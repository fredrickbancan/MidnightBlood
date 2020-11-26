using UnityEngine;
using UnityEngine.UI;

public class GameManagerAlpha : MonoBehaviour
{

    public GameObject authorityPrefab;
    public GameObject player;
    public GameObject uiCanvas;
    public static GameManagerAlpha instance = null;//singleton instance
    private bool authoritySpawned = false;
    // timer for level
    [SerializeField] private float levelTimer;
    [SerializeField] private Text levelTimerHUD;

    [SerializeField] private int energyRemaining;
    [SerializeField] private Text energyRemainingHUD;

    [SerializeField] private int playerScore;
    [SerializeField] private Text playerScoreHUD;

    [SerializeField] private int playerAttack;
    [SerializeField] private Text playerAttackHUD;

    [SerializeField] private bool villagersAlerted;
    [SerializeField] private bool gamePaused;


    void Start()
    {
       
    }

    void NewGame()
    { 
        playerScore = 0;
       

    }

    void NewLevel()
    {
        levelTimer = 5000.0f;
        energyRemaining = 100;
        villagersAlerted = false;

    }

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
