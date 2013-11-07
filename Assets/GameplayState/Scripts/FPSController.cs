using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {

    public float Speed = 6.0f;
    public float JumpSpeed = 8.0f;
    public float Gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded = false;

    void FixedUpdate()
    {
        if (isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= Speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = JumpSpeed;
            }
        }

        moveDirection.y -= Gravity * Time.deltaTime;

        CharacterController controller = GetComponent(typeof(CharacterController)) as CharacterController;
        CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
        isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
    }
}
