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
