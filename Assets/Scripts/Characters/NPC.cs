﻿using UnityEngine;
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

	protected UnityEngine.AI.NavMeshAgent agent {
		get { return transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>(); }
	}
	protected List<Character> enemies = new List<Character>();

	void Update() {
		data.health = lt.health;	

		if (!isAlive || GameManager.paused)
			return;

		characterIndicator.UpdateDialogueIndicator(data.dialogues.Count > 0 ? data.dialogues.Values[0] : null);
		walking = agent.enabled && agent.velocity.magnitude > .1f;
		LegAnimation();
		agent.speed = CalculateSpeed();
		Rotate();

		ExecuteTask();
	}

	void LateUpdate() {
		characterIndicator.UpdateVerticalPositioning(speech.currentlyDisplaying);
	}

	private Teleporter taskTeleporter;
	private void ExecuteTask() {
		timeOnCurrentTask += Time.deltaTime;
		NPCTask newTask = data.GetTask();
		if (newTask != task) {
			timeOnCurrentTask = 0;
			task = newTask;
		}
		if (task == null) {
			return;
		}
		System.Guid taskLocation = task.GetLocation().location;
		if (taskLocation != Map.CurrentLocation().guid) {
			GoToTeleporter(taskLocation);
			return;
		}
		task.Execute(this);
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
		GoToPosition(taskTeleporter.transform.position, horseUsage: taskTeleporter.permitHorses ? HorseUsage.IF_POSSIBLE : HorseUsage.NONE);
		if (Vector3.Distance(taskTeleporter.transform.position, transform.position) < 3f) {
			// taskTeleporter.Teleport(this);  // maybe?
			data.InitiateTravel(nextStop);
			GameManager.spawnedNPCs.Remove(this);
			Destroy(gameObject);
		}
	}

	public enum HorseUsage {
		NONE,        // Don't ride horse (interiors)
		OPTIONAL,    // Ride horse if it is more convenient (distance threshold)
		IF_POSSIBLE  // If they have a horse, ride it (teleporters to other locations)
	}
	private float DISTANCE_FROM_HORSE_TO_MOUNT = 2.5f;
	private float DISTANCE_FROM_TARGET_TO_GET_HORSE = 20f;

	public void GoToPosition(Vector3 pos, float dist = 0, HorseUsage horseUsage = HorseUsage.OPTIONAL) {
		if (!ridingHorse && mount != null && (horseUsage == HorseUsage.IF_POSSIBLE || (horseUsage == HorseUsage.OPTIONAL && (transform.position - pos).magnitude >= DISTANCE_FROM_TARGET_TO_GET_HORSE))) {
			if ((mount.transform.position - transform.position).magnitude < DISTANCE_FROM_HORSE_TO_MOUNT) {
				mount.Mount(this);
			} else {
				mount.Call();
				agent.destination = mount.transform.position;
				agent.stoppingDistance = 0;
			}
		} else {
			agent.destination = pos;
			agent.stoppingDistance = dist;	
			if (!hasLookTarget) {
				LookAt(mount.transform.position + mount.transform.forward);
			}
		}
	}

	public override void Die(Vector3 location, Vector3 angle, Character attacker = null, DamageType type = DamageType.MELEE) {
		if (arms.CurrentFrame != 0 && Random.Range(0, 2) == 0/* && currentState != NPCState.DOWN_TIED*/)
			arms.SetFrame(0);
		data.dialogues.Clear();
		characterIndicator.UpdateDialogueIndicator(null);
		base.Die(location, angle, attacker, type);
	}

	private void LegAnimation() {
		if (!walking && walk.isWalking) {
			walk.StandStill(true);
		} else if (walking && !walk.isWalking) {
			walk.StartWalk();
		}
	}
	
	private Player playerInteractingWith;
	public void Interact(Character character, string action) {
		if (!isAlive)
			return;

		if (data.dialogues.Count > 0 && character is Player && playerInteractingWith == null) {
			playerInteractingWith = character as Player;
			playerInteractingWith.playerUI.ShowDialogue(data.dialogues.First().Value, this);
		}
		
		// if (character.zipties > 0 && (currentState == NPCState.DOWN_UNTIED)) {
		// 	character.zipties--;
		// 	TransitionState(NPCState.DOWN_TIED);
		// }
	}
	public void Uninteract(Character character) {}

	public InteractAction[] GetActions(Character character) {
		if (!isAlive) {
			return new InteractAction[] {
				new InteractAction("Drag", (transform.position - character.transform.position).magnitude < 2f && character.CanSee(gameObject))
			};
		} else if (data.dialogues.Count > 0) {
			return new InteractAction[] {
				new InteractAction("Talk", (transform.position - character.transform.position).magnitude < 2f && character.CanSee(gameObject))
			};
		}
		return new InteractAction[0];
	}

	public void CancelDialogue() {
		playerInteractingWith = null;
	}

	public void FinishDialogue(bool removeDialogue) {
		bool startNextDialogue = false;
		if (removeDialogue) {
			data.dialogues.RemoveAt(0);
		}
		if (startNextDialogue && data.dialogues.Count > 0) {
			playerInteractingWith.playerUI.ShowDialogue(data.dialogues.First().Value, this);
		} else {
			playerInteractingWith.playerUI.HideDialogue();
			playerInteractingWith = null;
		}
	}

	// alerter - the character who triggered the alert (e.g. an attacker for an ATTACK stimulus)
	public void Alert(Stimulus s, Vector3 location, Character alerter) {
		(data.taskSources[NPCData.DYNAMIC_AI] as DynamicBehavior).React(s, location, alerter);
	}

	public Weapon CurrentWeapon() {
		// TODO get the most useful weapon for the current situation
		return currentGun;
	}

	// TODO: rewrite this shit so it isn't garbage

	/*public bool seesEvidence;
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
	}*/

	public bool ClearShot(GameObject target, float dist = 20f) {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, dist, sightLayers))
			return hit.collider.transform.root.gameObject == target.transform.root.gameObject;

		return false;
	}

	/*
	 * When a character is spawned, they're put at the location of the teleporter.
	 * We should fast forward their position to wherever they would be if we simulated position.
	 */
	public void FastForwardPosition() {
		NPCTask task = data.GetTask();
		if (task == null) {
			return;
		}
		Vector3 pos = task.GetLocation().position;
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

	protected override Interactable GetInteractable() {
		return null;
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
