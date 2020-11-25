using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collison : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            // is behind NPC then dead
            this.gameObject.SetActive(false);
            Debug.Log("Touched");


        }
        // otherwise NPC runs away
       
    }


}
