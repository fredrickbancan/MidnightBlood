using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerAlpha : MonoBehaviour
{
    private static GameManagerAlpha privateInstance;
    public static GameManagerAlpha instance
    {
        get
        {
            if (privateInstance == null)
            {
                privateInstance = FindObjectOfType<GameManagerAlpha>();
                if (privateInstance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManagerAlpha).Name;
                    privateInstance = obj.AddComponent<GameManagerAlpha>();
                }
            }
            return privateInstance;
        }
    }

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

    void Awake()
    {
        Debug.LogError("Awake");
        GetComponents();
        if (privateInstance == null)
        {
            Debug.LogError("Instance is null");
            privateInstance = this as GameManagerAlpha;
            DontDestroyOnLoad(gameObject);
            inMenu = true;
            paused = true;
            mainMenu.SetActive(true);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
        }
        else if (this != privateInstance)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.LogError("Start");

        Cursor.lockState = CursorLockMode.Confined;

        GetComponents();
        /*SetupMainMenuButtons();
        SetupPauseMenuButtons();
        SetuptGameOverMenuButtons();*/
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndGame();
        }
        //Debug.Log(paused);
        // If escape key is pressed, invert active state of UI canvas component
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            if (pauseMenu)
            {
                if (paused)
                {
                    Debug.Log("Paused");
                    ResumeGame();
                }
                else
                {
                    Debug.Log("Unpaused");
                    PauseGame();
                }
            }
        }
    }

    /// <summary>
    /// To be called each update when the player is in the correct position to kill a villager (I.E, Behind the vilager, in range and pointing towards them)
    /// This function is useful for triggering a display of U.I components that tell the player they can kill a villager with mouse click.
    /// </summary>
    public void OnPlayerEnterKillPosition()
    {
        //Debug.Log("Player is in Kill Position");
    }

    /// <summary>
    /// To be called each update when the player is NOT in the correct position to kill a villager (I.E, Behind the vilager, in range and pointing towards them)
    /// </summary>
    public void OnPlayerExitKillPosition()
    {
        //Debug.Log("Player is Not in Kill Position");
    }

    public void OnPlayerKillVillager(GameObject villager)
    {
        //Call ui and camera changes here
        //change scores and energy
        //Debug.Log("Player killed villager at: " + villager.transform.position.ToString());
        Destroy(villager);
    }

    public void OnPlayerCaptured()
    {
        //Debug.Log("Player captured by authority A.I");
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
    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
        inMenu = false;
        paused = false;
        SceneManager.LoadScene("MainStageAlpha");
    }
    public void PauseGame()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        paused = false;
    }
    public void ReturnToMenu()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        inMenu = true;
        paused = true;
        Cursor.visible = true;
    }
    public void EndGame()
    {
        GetComponents();
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        inMenu = true;
        paused = true;
        Cursor.visible = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    void GetComponents()
    {
        mainMenu = GameObject.Find("UI").transform.GetChild(0).gameObject;
        gameOverMenu = GameObject.Find("UI").transform.GetChild(1).gameObject;
        pauseMenu = GameObject.Find("UI").transform.GetChild(2).gameObject;

        /*mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);*/
    }
    /*void SetupMainMenuButtons()
    {
        Button button = GameObject.Find("Start Button").GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        Button buttonTwo = GameObject.Find("Quit Button").GetComponent<Button>();
        buttonTwo.onClick.AddListener(QuitGame);
    }
    void SetuptGameOverMenuButtons()
    {
        Button button = GameObject.Find("Play Button").GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        button = GameObject.Find("Exit Button").GetComponent<Button>();
        button.onClick.AddListener(ReturnToMenu);
    }
    void SetupPauseMenuButtons()
    {
        Button button = GameObject.Find("Resume Button").GetComponent<Button>();
        button.onClick.AddListener(ResumeGame);
        button = GameObject.Find("Restart Button").GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        button = GameObject.Find("Menu Button").GetComponent<Button>();
        button.onClick.AddListener(ReturnToMenu);
    }*/
}