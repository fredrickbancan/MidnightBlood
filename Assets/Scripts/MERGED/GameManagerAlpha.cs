using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerAlpha : MonoBehaviour
{
    public static GameManagerAlpha instance = null;//singleton instance

    public GameObject authorityPrefab;
    public GameObject player;

    // UI Canvases
    private GameObject pauseMenu;
    private GameObject mainMenu;
    private GameObject gameOverMenu;

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
    //[SerializeField] private bool gamePaused;

    private bool authoritySpawned = false;
    bool started = false;
    public bool restarted = false;
    public bool inMenu = false;
    public bool paused = false;
    public bool gameOver = false;

    void Awake()
    {
        Debug.Log("Awake");
        GetComponents();
        if (instance == null)
        {
            Debug.Log("Instance is null");
            instance = this;
            inMenu = true;
            paused = true;
            DontDestroyOnLoad(gameObject);
            
            mainMenu.SetActive(true);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
        }
    }
    void Start()
    {
        Debug.Log("Start");
    }
    void Update()
    {
        // If escape key is pressed, invert active state of UI canvas component
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            if (pauseMenu)
            {
                if (paused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                    paused = true;
                }
            }
        }
    }

    public void OnPlayerCaptured()
    {
        Debug.Log("Player captured by authority A.I");
        EndGame();
    }

    public void ReloadPlayer()
    {
        player = GameObject.Find("Player");
    }

    public bool IsPlayerBloody()
    {
        return true;
    }

    public Vector3 GetPlayerPos()
    {
        ReloadPlayer();
        return player.transform.position;
    }

    public Vector3 GetPlayerEuler()
    {
        ReloadPlayer();
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

    void NewLevel()
    {
        levelTimer = 5000.0f;
        energyRemaining = 100;
        villagersAlerted = false;
    }
    
    void NewGame()
    {
        playerScore = 0;
    }

    public void StartGame(string sceneName)
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
        inMenu = false;
        paused = false;
        //gameOver = false;
        SceneManager.LoadScene(sceneName);
    }

    public void PauseGame()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        paused = false;
    }
    public void ReturnToMenu()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        inMenu = true;
        paused = true;
        //gameOver = true;
        Cursor.visible = true;
    }
    public void EndGame()
    {
        GetComponents();
        //Time.timeScale = 0;
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        inMenu = true;
        paused = true;
        //gameOver = true;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void GetComponents()
    {
        mainMenu = GameObject.Find("_UI").transform.GetChild(0).gameObject;
        gameOverMenu = GameObject.Find("_UI").transform.GetChild(1).gameObject;
        pauseMenu = GameObject.Find("_UI").transform.GetChild(2).gameObject;
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }
}