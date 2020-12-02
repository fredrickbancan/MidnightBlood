using UnityEngine;

public class PlayerTriggerScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Respawn")
        {
            GameManagerAlpha.instance.OnPlayerEnterChurchTrigger();
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Respawn")
        {
            GameManagerAlpha.instance.OnPlayerExitChurchTrigger();
        }
    }
}
