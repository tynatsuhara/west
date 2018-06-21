﻿using UnityEngine;
using System.Collections;

/*
	TODO get rid of this
 */
public class Civilian : NPC {

	public bool hasGun;  // can enter attacking states

	public override void Start() {
		base.Start();
		hasGun = sidearmId > 0 || weaponId > 0;
	}

	//=================== STATE FUNCTIONS ===================//

	// CivilianState.PASSIVE
	private bool checkDrawWeaponInvoked = false;
	// protected override void StatePassive() {


		// LoseLookTarget();
		// BraveCitizenCheck();
		// LookForEvidence();
	// }

	private void CheckDrawWeapon() {
		// if (!SeenByAnyPlayers()) {
		// 	TransitionState(NPCState.ATTACKING, 0f);
		// } else {
		// 	checkDrawWeaponInvoked = false;
		// }
	}
	private void BraveCitizenCheck() {
		// if (SeenByAnyPlayers())
		// 	return;
		
		// Player playerScript = ClosestEnemyPlayerInSight();
		// if (playerScript == null)
		// 	return;

		// if (hasGun && !checkDrawWeaponInvoked && playerScript.IsEquipped() && playerScript.isAlive) {
		// 	// switch to attacking
		// 	bool canSeePlayer = currentState == NPCState.PASSIVE 
		// 			? CanSee(playerScript.gameObject) 
		// 			: ClearShot(playerScript.gameObject);
		// 	if (canSeePlayer && !playerScript.CanSee(gameObject)) {
		// 		Invoke("CheckDrawWeapon", Random.Range(.3f, 3f));
		// 		checkDrawWeaponInvoked = true;				
		// 	}
		// }
	}

	private bool SeenByAnyPlayers() {
		foreach (Player pc in GameManager.players) {
			if (pc.CanSee(gameObject))
				return true;
		}
		return false;
	}

	// any states that can come after CivilianState.HELD_HOSTAGE_UNTIED should call this,
	// since the civilian will be laying on the ground
	private void ResetRB() {
		if (rb.constraints == RigidbodyConstraints.None) {
			rb.rotation = Quaternion.Euler(new Vector3(0, rb.rotation.y, 0));
			rb.position = new Vector3(rb.position.x, 1.1f, rb.position.z);
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;			
			arms.SetFrame(0);
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}

	// CivilianState.ALERTING
	// protected override void StateAlerting() {
	// 	if (firstStateIteration)
	// 		Debug.Log("ALERT ALERT OH SHIT");
	// }

	// // CivilianState.FLEEING
	// protected override void StateFleeing() {
	// 	if (firstStateIteration) {
	// 		ResetRB();
	// 		arms.SetFrame(1);
	// 		UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	// 		Vector3 destPos = Random.insideUnitCircle * 30;				
	// 		destPos.z = destPos.y;
	// 		destPos.y = transform.position.y;
	// 		agent.destination = destPos;
	// 	}
	// }

	// // CivilianState.ATTACKING
	// protected override void StateAttacking() {
	// 	ResetRB();
	// 	DrawWeapon();
	// 	Player pc = ClosestEnemyPlayerInSight();
	// 	if (pc != null) {
	// 		LookAt(pc.transform);
	// 		if (CanSee(pc.gameObject, fov:40f)) {
	// 			Shoot();
	// 		}
	// 	}
	// }

	// // CivilianState.HELD_HOSTAGE_UNTIED
	// protected override void StateDownUntied() {
	// 	LoseLookTarget();						

	// 	if (firstStateIteration) {
	// 		arms.SetFrame(1);  // hands up
	// 		rb.constraints = RigidbodyConstraints.None;
	// 		GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
	// 		Vector3 rot = rb.rotation.eulerAngles;
	// 		rot.x += 32f;
	// 		rb.MoveRotation(Quaternion.Euler(rot));
	// 	} else {
	// 		BraveCitizenCheck();
	// 	}
	// }

	// // CivilianState.HELD_HOSTAGE_TIED
	// protected override void StateDownTied() {
	// 	if (arms.CurrentFrame != 2) {
	// 		arms.SetFrame(2);
	// 	}
	// }


	// public override void Alert(Reaction importance, Vector3 position) {
	// 	if (!isAlive||
	// 		currentState == NPCState.ATTACKING ||
	// 		currentState == NPCState.DOWN_UNTIED ||
	// 		currentState == NPCState.DOWN_TIED)
	// 		return;

	// 	if (currentState == NPCState.ALERTING) {
	// 		TransitionState(NPCState.DOWN_UNTIED);
	// 		return;
	// 	}

	// 	if (importance == Reaction.SUSPICIOUS && currentState == NPCState.PASSIVE) {
	// 		// random chance that they actually care
	// 		if (Random.Range(0, 2) == 0) {
	// 			TransitionState(NPCState.ALERTING, Random.Range(.3f, 1f));
	// 		}
	// 	} else if (importance == Reaction.AGGRO) {
	// 		Invoke("SpeechAlerted", Random.Range(.3f, 1f));
	// 		if (Random.Range(0, 2) == 0) {
	// 			TransitionState(NPCState.FLEEING, Random.Range(.3f, 1f));
	// 		} else {
	// 			TransitionState(NPCState.DOWN_UNTIED, Random.Range(.3f, 1f));
	// 		}
	// 	}
	// }

	// private void SpeechAlerted() {
	// 	if (!speech.currentlyDisplaying && Random.Range(0, 3) == 0) {
	// 		speech.SayRandom(Speech.CIVILIAN_ALERTED, showFlash: true);
	// 	}
	// }
}
