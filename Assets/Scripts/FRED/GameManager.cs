using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the authority prefab so it can be instantiated and spawned in world when required
    /// </summary>
    public GameObject authorityPrefab;
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

    /// <summary>
    /// To be called each update when the player is in the correct position to kill a villager (I.E, Behind the vilager, in range and pointing towards them)
    /// This function is useful for triggering a display of U.I components that tell the player they can kill a villager with mouse click.
    /// </summary>
    public void OnPlayerEnterKillPosition()
    {
        Debug.Log("Player is in Kill Position");
    }

    /// <summary>
    /// To be called each update when the player is NOT in the correct position to kill a villager (I.E, Behind the vilager, in range and pointing towards them)
    /// </summary>
    public void OnPlayerExitKillPosition()
    {
        Debug.Log("Player is Not in Kill Position");
    }

    public void OnPlayerKillVillager(GameObject villager)
    {
        Debug.Log("Player killed villager at: " + villager.transform.position.ToString());
        Destroy(villager);
    }

    public void OnPlayerCaptured()
    {
        Debug.Log("Player captured by authority A.I");
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
        //enable notification in player hud they are being chased
        authoritySpawned = true;
        Debug.Log("Villager escaped and authority spawned at " + eventPos.ToString());
        Instantiate(authorityPrefab, eventPos, Quaternion.identity);
    }

    /// <summary>
    /// For telling if the authority is in the world and chasing player, useful for U.I Notifications.
    /// </summary>
    public bool IsAuthoritySpawned()
    {
        return authoritySpawned;
    }
}
