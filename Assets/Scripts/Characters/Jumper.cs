using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour {

	public Vector3 jumpForce;
	public float jumpGravityDelay;
	public float landingDirtRadius;

	public bool isJumping {
		// considered "jumping" as long as upwards velocity is non-zero(ish)
		get { return Mathf.Abs(rb.velocity.y) > .001f; }
	}

	private Rigidbody rb;

	public void Start() {
		rb = GetComponent<Rigidbody>();
	}

	public void Jump(Vector3? force = null) {
		if (force == null)
			force = jumpForce;
		StartCoroutine(JumpCoroutine(force.Value));
	}

	private IEnumerator JumpCoroutine(Vector3 force) {
		rb.useGravity = false;
		rb.AddRelativeForce(force, ForceMode.Force);
		yield return new WaitForSeconds(jumpGravityDelay);
		rb.useGravity = true;
	}
}
