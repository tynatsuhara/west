using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPC : Character, Interactable {

	private NPCData data;
	private NPCTask task;
	public CharacterIndicator characterIndicator;

	public bool firstStateIteration {
		get { return timeOnCurrentTask == 0; }
	}

	private float timeOnCurrentTask;

	protected UnityEngine.AI.NavMeshAgent agent;
	protected List<Character> enemies = new List<Character>();

	public void Awake() {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}

	public override void Start() {
		StartCoroutine(UpdateEvidenceInSight(.5f));
		StartCoroutine(UpdateEquippedPlayersInSight(.1f));

		base.Start();
	}

	void Update() {
		characterIndicator.UpdateQuestsToGive(data.questsToGive.Count > 0 && isAlive);

		if (!isAlive || GameManager.paused)
			return;

		LegAnimation();
		walking = agent.enabled && agent.velocity != Vector3.zero;
		Rotate();
		agent.speed = CalculateSpeed();

		timeOnCurrentTask += Time.deltaTime;

		NPCTask newTask = data.GetTask();
		if (newTask != task) {
			timeOnCurrentTask = 0;
			task = newTask;
		}

		ExecuteTask();
	}

	private Teleporter taskTeleporter;
	private void ExecuteTask() {
		System.Guid taskLocation = task.GetLocation().location;
		if (taskLocation != Map.CurrentLocation().guid) {
			GoToTeleporter(taskLocation);
			return;
		}
		
		if (task is NPCFollowTask) {
			ExecuteTask(task as NPCFollowTask);
		}
	}

	private void GoToTeleporter(System.Guid taskLocation) {
		List<System.Guid> path = data.SetPathFor(taskLocation);
		if (path.First() == data.location) {
			path.RemoveAt(0);
		}
		System.Guid nextStop = path.First();
		if (taskTeleporter == null || taskTeleporter.toId != nextStop) {
			taskTeleporter = GameObject.FindObjectsOfType<Teleporter>().Where(x => x.toId == nextStop).First();
		}
		GoToPosition(taskTeleporter.transform.position);
		if (Vector3.Distance(taskTeleporter.transform.position, transform.position) < 3f) {
			// taskTeleporter.Teleport(this);  // maybe?
			data.InitiateTravel(nextStop);
			Destroy(gameObject);
		}
	}

	private void ExecuteTask(NPCFollowTask followTask) {
		GoToPosition(followTask.GetLocation().position, followTask.distance);
	}

	private void GoToPosition(Vector3 pos, float dist = 0) {
		agent.destination = pos;
		agent.stoppingDistance = dist;
	}

	public override void Die(Vector3 location, Vector3 angle, Character attacker = null, DamageType type = DamageType.MELEE) {
		if (arms.CurrentFrame != 0 && Random.Range(0, 2) == 0/* && currentState != NPCState.DOWN_TIED*/)
			arms.SetFrame(0);
		data.health = health;
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
	
	public void Interact(Character character) {
		if (!isAlive)
			return;

		if (data.questsToGive.Count > 0 && character is Player) {
			Player p = character as Player;
			Quest q = data.questsToGive.First();
			p.playerUI.ShowDialogue(true);
			SaveGame.currentGame.quests.AddQuest(q);
		}
		
		// if (character.zipties > 0 && (currentState == NPCState.DOWN_UNTIED)) {
		// 	character.zipties--;
		// 	TransitionState(NPCState.DOWN_TIED);
		// }
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
			// isEvidence |= c.currentState == Civilian.NPCState.ALERTING;
			// isEvidence |= c.currentState == Civilian.NPCState.DOWN_TIED;
			// isEvidence |= c.currentState == Civilian.NPCState.DOWN_UNTIED;
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

	/*
	 * When a character is spawned, they're put at the location of the teleporter.
	 * We should fast forward their position to wherever they would be if we simulated position.
	 */
	public void FastForwardPosition() {
		Vector3 pos = data.GetTask().GetLocation().position;
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(pos, path);
		float pathLen = 0;
		for (int i = 1; i < path.corners.Length; i++) {
			pathLen += (path.corners[i] - path.corners[i-1]).magnitude;
		}

		float distTraveled = data.timeSimulatedInLocation * CalculateSpeed();
		if (distTraveled > pathLen) {
			transform.position = pos;
			return;
		}

		float dist = 0;			
		for (int i = 1; i < path.corners.Length; i++) {
			float lineLen = (path.corners[i] - path.corners[i-1]).magnitude;
			if (lineLen > distTraveled - dist) {
				float percentDownLine = Mathf.Clamp01((distTraveled - dist) / lineLen);
				transform.position = path.corners[i-1] + (path.corners[i] - path.corners[i-1]) * percentDownLine;
				return;
			}
		}
	}

	///////////////////// SAVE STATE FUNCTIONS /////////////////////

	public NPCData SaveData() {
		data = (NPCData) base.SaveData(data);
		data.name = name;
		data.rotation = new SerializableVector3(transform.rotation.eulerAngles);
		return data;
	}

	public void LoadSaveData(NPCData data) {
		this.data = data;
		base.LoadSaveData(data);
		name = data.name;
		if (!isAlive)
			SetDeathPhysics();
		transform.rotation = Quaternion.Euler(data.rotation.val);
 	}
}
