using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NPC : Character, Interactable {

	public enum NPCType {
		NORMIE
	}

	public enum NPCState {
		PASSIVE,                    // completing their scheduled tasks
		CURIOUS,    			 	// they know something is up, but don't know of the player
		SEARCHING,   				// they are aware of the player, but don't know location
		ALERTING,					// running to notify guards
		FLEEING,					// running off the map
		DOWN_UNTIED,
		DOWN_TIED,
		ATTACKING,
		ATTACK_READY                // standoff
	}

	public bool firstStateIteration {
		get { return timeInCurrentState == 0; }
	}

	public NPCType type;
	public NPCState currentState;
	public List<NPCTaskSource> taskSources;

	protected UnityEngine.AI.NavMeshAgent agent;
	protected List<Character> enemies = new List<Character>();

	public override void Start() {
		StartCoroutine(UpdateEvidenceInSight(.5f));
		StartCoroutine(UpdateEquippedPlayersInSight(.1f));

		base.Start();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	void Update() {
		if (!isAlive || GameManager.paused)
			return;

		LegAnimation();
		walking = agent.enabled && agent.velocity != Vector3.zero;
		Rotate();
		agent.speed = CalculateSpeed();

		// Deal with race conditions with resetting timers
		if (resetTimeFlag) {
			resetTimeFlag = false;
			timeInCurrentState = 0;
		}

		switch (currentState) {
			case NPCState.PASSIVE:
				StatePassive();
				break;
			case NPCState.CURIOUS:
				StateCurious();
				break;
			case NPCState.SEARCHING:
				StateSearching();
				break;
			case NPCState.ALERTING:
				StateAlerting();
				break;
			case NPCState.FLEEING:
				StateFleeing();
				break;
			case NPCState.DOWN_UNTIED:
				StateDownUntied();
				break;
			case NPCState.DOWN_TIED:
				StateDownTied();
				break;
			case NPCState.ATTACKING:
				StateAttacking();
				break;
		}

		timeInCurrentState += Time.deltaTime;
	}

	protected virtual void StatePassive() {}
	protected virtual void StateCurious() {}
	protected virtual void StateSearching() {}
	protected virtual void StateAlerting() {}
	protected virtual void StateFleeing() {}
	protected virtual void StateDownUntied() {}
	protected virtual void StateDownTied() {}
	protected virtual void StateAttacking() {}

	public override void Die(Vector3 location, Vector3 angle, Character attacker = null, DamageType type = DamageType.MELEE) {
		if (arms.CurrentFrame != 0 && Random.Range(0, 2) == 0 && currentState != NPCState.DOWN_TIED)
			arms.SetFrame(0);
		base.Die(location, angle, attacker, type);
	}

	private void LegAnimation() {
		Vector3 velocity = agent.velocity;
		velocity.y = 0f;
		if (velocity == Vector3.zero) {
			if (walk.isWalking) {
				walk.StandStill();
			}
		} else if (velocity.magnitude > 0f) {
			if (!walk.isWalking) {
				walk.StartWalk();
			}
		}
	}

	public WorldPoint[] pathToFollow;
	private int currentPointIndex;
	private float timeAtCurrentPoint;
	protected void FollowPath() {
		if (pathToFollow == null || pathToFollow.Length == 0)
			return;
		WorldPoint currentPoint = pathToFollow[currentPointIndex];
		bool atPoint = (currentPoint.transform.position - transform.position).magnitude < 1f;
		if (atPoint) {
			timeAtCurrentPoint++;
			if (currentPoint.faceDirection) {
				LookAt(transform.position + currentPoint.lookDirection);
			}
			if (timeAtCurrentPoint > currentPoint.timeToStayHere && currentPoint.timeToStayHere >= 0) {
				int oldIndex = currentPointIndex;
				currentPointIndex = (currentPointIndex + 1) % pathToFollow.Length;
				if (oldIndex != currentPointIndex)
					agent.SetDestination(currentPoint.transform.position);
			}
		}
		if (agent.destination != pathToFollow[currentPointIndex].transform.position)
			agent.SetDestination(pathToFollow[currentPointIndex].transform.position);
	}
	protected void StopFollowingPath() {

	}

	protected bool transitioningState;
	protected float timeInCurrentState;
	private NPCState stateToTransitionTo;
	private bool resetTimeFlag;
	public void TransitionState(NPCState newState, float time = 0f) {
		Debug.Log(name + " transitioning to " + newState);
		stateToTransitionTo = newState;
		transitioningState = true;
		if (time <= 0f) {
			CompleteTransition();
		} else {
			Invoke("CompleteTransition", time);
		}
	}
	private void CompleteTransition() {
		if (currentState != stateToTransitionTo) {
			resetTimeFlag = true;
		}
		currentState = stateToTransitionTo;
		transitioningState = false;
	}

	public void Interact(Character character) {
		if (character.zipties > 0 && (currentState == NPCState.DOWN_UNTIED)) {
			character.zipties--;
			TransitionState(NPCState.DOWN_TIED);
		}
	}
	public void Uninteract(Character character) {}

	public bool seesEvidence;
	public Vector3? evidencePoint;
	public IEnumerator UpdateEvidenceInSight(float timeStep) {
		while (isAlive) {
			evidencePoint = AddEquippedPlayersInSight();
			if (evidencePoint == null) {
				evidencePoint = CorpsesInSight();
			}
			seesEvidence = evidencePoint != null;
			yield return new WaitForSeconds(timeStep);
		}
	}

	// Returns the point of the closest enemy in sight
	private Dictionary<Player, float> enemyPlayersInSight = new Dictionary<Player, float>();
	private Vector3? AddEquippedPlayersInSight() {
		List<Player> seenPlayers = GameManager.players
				.Where(x => x.IsEquipped() && (CanSee(x.gameObject)))
				.OrderBy(x => (x.transform.position - transform.position).magnitude)
				.ToList();
		foreach (Player pc in seenPlayers) {
			if (!enemyPlayersInSight.ContainsKey(pc))
				enemyPlayersInSight.Add(pc, Time.time);
		}
		if (seenPlayers.Count > 0)
			return seenPlayers[0].transform.position;
		return null;
	}
	private Vector3? CorpsesInSight() {
		foreach (NPC c in GameManager.spawnedNPCs) {
			bool isEvidence = !c.isAlive;
			isEvidence |= c.currentState == Civilian.NPCState.ALERTING;
			isEvidence |= c.currentState == Civilian.NPCState.DOWN_TIED;
			isEvidence |= c.currentState == Civilian.NPCState.DOWN_UNTIED;
			if (isEvidence && CanSee(c.gameObject)) {
				return c.transform.position;
			}
		}
		return null;
	}

	private static float TIME_IN_SIGHT_BEFORE_ATTACK = .6f;
	protected void LookForEvidence() {
		foreach (Player pc in enemyPlayersInSight.Keys) {
			if (Time.time - enemyPlayersInSight[pc] > TIME_IN_SIGHT_BEFORE_ATTACK) {
				Alert(Reaction.AGGRO, evidencePoint.Value);
			}
		}
	}
	private IEnumerator UpdateEquippedPlayersInSight(float timeStep) {
		Player[] pcs = enemyPlayersInSight.Keys.ToArray();
		while (isAlive) {
			foreach (Player pc in pcs) {
				if (!CanSee(pc.gameObject)) {
					enemyPlayersInSight.Remove(pc);
				}
			}
			yield return new WaitForSeconds(timeStep);
		}
	}

	public Player ClosestEnemyPlayerInSight() {
		Player playerScript = null;
		foreach (Player pc in GameManager.players) {
			if (!pc.IsEquipped() || !CanSee(pc.gameObject) || !pc.isAlive)
				continue;

			if (playerScript == null || ((transform.position - pc.transform.position).magnitude <
									     (transform.position - playerScript.transform.position).magnitude))
				playerScript = pc;
		}
		return playerScript;
	}

	public bool ClearShot(GameObject target, float dist = 20f) {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, dist, sightLayers))
			return hit.collider.transform.root.gameObject == target.transform.root.gameObject;

		return false;
	}

	public override void Alert(Reaction importance, Vector3 position) {}

	///////////////////// SAVE STATE FUNCTIONS /////////////////////

	public NPCSaveData SaveData() {
		NPCSaveData data = (NPCSaveData) base.SaveData(SaveGame.currentGame.savedCharacters[guid]);
		data.type = type;
		data.name = name;
		data.rotation = new SerializableVector3(transform.rotation.eulerAngles);
		data.state = currentState;
		data.taskSources = taskSources;
		return data;
	}

	public void LoadSaveData(NPCSaveData data) {
		base.LoadSaveData(data);
		type = data.type;
		name = data.name;
		if (!isAlive)
			SetDeathPhysics();
		transform.rotation = Quaternion.Euler(data.rotation.val);
		taskSources = data.taskSources;
		TransitionState(data.state);
	}

	[System.Serializable]
	public class NPCSaveData : CharacterSaveData {
		public NPCType type;
		public string name;
		public SerializableVector3 rotation = new SerializableVector3(new Vector3(0, Random.Range(0, 360), 0));
		public NPCState state = NPCState.PASSIVE;
		public List<NPCTaskSource> taskSources = new List<NPCTaskSource>();
		private float timeOfLastSimulation = SaveGame.currentGame.time.worldTime;

		public NPCSaveData(NPCType type, bool female = false, string lastName = "") {
			this.type = type;
			this.female = female;
			name = NameGen.CharacterName(female, lastName);
		}

		public void Simulate(float newWorldTime) {
			List<NPCTask> tasks = taskSources
					.Select(x => x.GetTask(guid))
					.Where(x => x != null)
					.ToList();
			if (tasks.Count == 0) {
				return;
			}
			int maxPriority = tasks.Max(x => x.priority);
			NPCTask task = taskSources
					.Select(x => x.GetTask(guid))
					.Where(x => x != null && x.priority == maxPriority)
					.First();
			SimulateTask(task);
			timeOfLastSimulation = newWorldTime;
		}

		private void SimulateTask(NPCTask task) {
			// TODO
		}
	}
}
