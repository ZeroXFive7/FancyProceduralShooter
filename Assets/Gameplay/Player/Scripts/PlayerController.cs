using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerController : MonoBehaviour
{
	public float WalkSpeed = 10.0f;
	public float MaxDeltaVelocity = 10.0f;

	public float Gravity = 10.0f;
	public bool CanJump = true;
	public float JumpHeight = 2.0f;
	
	private float lerpSpeed = 10.0f;
	private bool isGrounded = false;
	private Vector3 up = Vector3.up;
	
	private float jumpSpeed
	{
		get
		{
			return Mathf.Sqrt(2 * JumpHeight * Gravity);	
		}
	}
	
	void Awake()
	{
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
	}
	
	void FixedUpdate()
	{
		rigidbody.AddForce(-up * Gravity * rigidbody.mass);
	}
	
	void Update()
	{
		if (isGrounded && Input.GetButtonUp("Jump"))
		{
			rigidbody.velocity += up * jumpSpeed;	
		}
		
		Vector3 targetUp = Vector3.Normalize(-transform.position);
		up = Vector3.Lerp(up, targetUp, lerpSpeed  * Time.deltaTime);
		
		Vector3 forward = Vector3.Cross(transform.right, up);
		
		Quaternion targetRotation = Quaternion.LookRotation(forward, up);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
		
		transform.Translate(Input.GetAxis("Horizontal") * WalkSpeed * Time.deltaTime, 0.0f, Input.GetAxis("Vertical") * WalkSpeed * Time.deltaTime);
		isGrounded = false;
	}
	
	void OnCollisionStay()
	{
		isGrounded = true;
	}
	
	public void SetOrientation(Vector3 newUp, Vector3 newRight)
	{
		Vector3 forward = Vector3.Cross(newRight, newUp);
		transform.rotation = Quaternion.LookRotation(forward, newUp);
	}
}
