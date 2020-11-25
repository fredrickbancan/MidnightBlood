using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public static GameManager instance = null;//singleton instance
    private bool authoritySpawned = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsPlayerBloody()
    {
        return true;
    }

    public Vector3 GetPlayerPos()
    {
        return player.transform.position;
    }

    public Vector3 GetPlayerEuler()
    {
        return player.transform.eulerAngles;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void OnVillagerEscape(Vector3 eventPos)
    {
        //spawn authority and set global value
        authoritySpawned = true;
        Debug.Log("Villager escaped and authority spawned at " + eventPos.ToString());
    }

    /// <summary>
    /// For telling if the authority is in the world and chasing player, useful for U.I Notifications.
    /// </summary>
    public bool IsAuthoritySpawned()
    {
        return authoritySpawned;
    }
}
