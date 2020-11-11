using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Input/FPS Input")]

public class FPSInput : MonoBehaviour {

    public float walkSpeed = 4.0f;
    public float runSpeed = 8.0f;
    public float gravity = -9.8f;

    private CharacterController _charController;

	void Start () {
        _charController = GetComponent<CharacterController>(); 
	}
	
	// Update is called once per frame
	void Update () {

        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            speed = runSpeed;
        }

        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = gravity;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        _charController.Move(movement);
	}
}
