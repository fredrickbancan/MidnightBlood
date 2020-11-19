using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

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
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        transform.Translate(movement * Time.deltaTime * speed);


    }
}
