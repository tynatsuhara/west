using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour {

	public Vector3 jumpForce;
	public float jumpGravityDelay;

	private Rigidbody rb;

	public void Start() {
		rb = GetComponent<Rigidbody>();
	}

	public void Jump() {
		StartCoroutine(JumpCoroutine());
	}

	private IEnumerator JumpCoroutine() {
		rb.useGravity = false;
		rb.AddRelativeForce(jumpForce, ForceMode.Force);
		if (jumpGravityDelay > 0)
			yield return new WaitForSeconds(jumpGravityDelay);
		rb.useGravity = true;
	}
}
