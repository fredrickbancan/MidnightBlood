﻿using UnityEngine;

/// <summary>
/// This class is for non-movement related player logic (since this is made by Fred and movement scripts belong to Andrew)
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float attackDistance = 3.0F;

    void Update()
    {
        /*Testing when player left clicks if they are in range of a villager and are in the correct position to kill them*/

        RaycastHit info;
        Ray meleRay = new Ray(player.transform.position, player.transform.forward);
        if (Physics.Raycast(meleRay, out info, attackDistance) && info.collider.gameObject.tag == "Villager")
        {
            if (PlayerIsInKillOrientation(info.collider.gameObject))
            {
                GameManager.instance.OnPlayerEnterKillPosition();
                if (Input.GetMouseButtonDown(0))
                {
                    GameManager.instance.OnPlayerKillVillager(info.collider.gameObject);
                }
            }
            else
            {
                GameManager.instance.OnPlayerExitKillPosition();
            }
        }
        else
        {
            GameManager.instance.OnPlayerExitKillPosition();
        }
    }

    private bool PlayerIsInKillOrientation(GameObject villager)
    {
        return Vector3.Dot(player.transform.forward, villager.transform.forward) > 0.0F;
    }
}
