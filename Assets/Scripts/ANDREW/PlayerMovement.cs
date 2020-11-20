using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 8.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // variables for x- and z- axis movement.
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // create a vector 3 that updates each frame. only going to affect front/back and side to side movement. y-axis (height) is not changed as we are not jumping.
        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;

        // calls the Move() on the character controller 'controller' which is the resultant magnitude of Vector3 multiplied by speed by deltaTime.
        controller.Move(move * speed * Time.deltaTime);
                
    }
}
