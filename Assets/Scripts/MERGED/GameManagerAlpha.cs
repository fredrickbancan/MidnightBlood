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

    [SerializeField] private GameObject authorityPrefab;
    [SerializeField] private GameObject player;

    // UI Canvases
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public GameObject gameOverMenu;
    public GameObject hudMenu;
    public Slider hudTimer;
    public Slider hudEnergy;

    // timer for level
    [SerializeField] private float levelTime;
    [SerializeField] private float energyDepletionSpeed;
    [SerializeField] private float energyKillRewardAmount;
    [SerializeField] private float newNightEnergyMultiplier;
    private float remainingTime;
    private float energyLevel = 1.0F;

    private int killCount;
    private Text killCountText;
    private bool gameStarted;

    private bool authoritySpawned = false;
    public bool inMenu = false;
    public bool paused = false;
    public bool playerInChurchTrigger = false;
    public bool gameOver = false;
    public bool churchBellRang = false;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip callForHelpSound;
    [SerializeField] AudioClip callToPlayerSound;
    [SerializeField] AudioClip screamFromAttackSound;
    [SerializeField] AudioClip playerDiesSound;
    [SerializeField] AudioClip churchBellSound;
    [SerializeField] int bellTimer = 20;

    void Awake()
    {
        if (privateInstance == null)
        {
            GetMenus();
            SetupMainMenuButtons();
            SetupPauseMenuButtons();
            SetuptGameOverMenuButtons();

            privateInstance = this as GameManagerAlpha;
            DontDestroyOnLoad(gameObject);
            inMenu = true;
            paused = true;
            
            mainMenu.SetActive(true);
            gameOverMenu.SetActive(false);
            pauseMenu.SetActive(false);
            hudMenu.SetActive(false);
        }
        else if (this != privateInstance)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    void Update()
    {
        if (gameStarted)
        {
            remainingTime -= Time.deltaTime;
            killCountText.text = killCount.ToString();
            GameObject g;
            if ( (g = GameObject.Find("Slider")))
            {
                hudTimer = g.GetComponent<Slider>();
                
                hudTimer.value = 1 - remainingTime / levelTime;
            }

            if (((int) remainingTime == bellTimer) && !churchBellRang)
            {
                audioSource = player.GetComponent<AudioSource>();
                audioSource.PlayOneShot(churchBellSound);

            }


            if (remainingTime <= 0)
            {
                if (!playerInChurchTrigger)
                {
                    gameOver = true;
                }
                EndGame();

            }

            energyLevel -= energyDepletionSpeed * Time.deltaTime;
            if(energyLevel <= 0)
            {
                gameOver = true;
                EndGame();
            }
            if ((g = GameObject.Find("EnergySlider")))
            {
                hudEnergy = g.GetComponent<Slider>();

                hudEnergy.value = 1 - energyLevel;
            }
        }
        // Temporary button for testing purpuses
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndGame();
        }
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
                }
            }
        }
    }

    public void OnPlayerEnterChurchTrigger()
    {
        playerInChurchTrigger = true;
        GameObject.Find("ChurchText").GetComponent<Text>().color = new Color(1, 0.43f, 0.43f, 1);
    }

    public void OnPlayerExitChurchTrigger()
    {
        playerInChurchTrigger = false;
        GameObject.Find("ChurchText").GetComponent<Text>().color = new Color(1, 0.43f, 0.43f, 0);
    }

    /// <summary>
    /// To be called each update when the player is in the correct position to kill a villager (I.E, Behind the vilager, in range and pointing towards them)
    /// This function is useful for triggering a display of U.I components that tell the player they can kill a villager with mouse click.
    /// </summary>
    public void OnPlayerEnterKillPosition()
    {
        //Debug.Log("Player is in Kill Position");
        GameObject.Find("CanKillText").GetComponent<Text>().color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// To be called each update when the player is NOT in the correct position to kill a villager (I.E, Behind the vilager, in range and pointing towards them)
    /// </summary>
    public void OnPlayerExitKillPosition()
    {
        //Debug.Log("Player is Not in Kill Position");
        GameObject text;
        if ((text = GameObject.Find("CanKillText")))
        {
            text.GetComponent<Text>().color = new Color(1, 1, 1, 0.25f);
        }
    }

    public void OnPlayerKillVillager(GameObject villager)
    {
        audioSource = player.GetComponent<AudioSource>();
        audioSource.PlayOneShot(screamFromAttackSound);

        //Call ui and camera changes here
        //change scores and energy
        //Debug.Log("Player killed villager at: " + villager.transform.position.ToString());
        Destroy(villager);
        killCount++;
        killCountText.text = killCount.ToString();
        energyLevel += energyKillRewardAmount;
        if (energyLevel > 1.0F) energyLevel = 1.0F;
    }

    public void OnPlayerCaptured()
    {
        audioSource = player.GetComponent<AudioSource>();
        audioSource.PlayOneShot(playerDiesSound);
        //Debug.Log("Player captured by authority A.I");
        gameOver = true;
        EndGame();
        Cursor.visible = true;
        GameObject[] authorities = GameObject.FindGameObjectsWithTag("BigMan");
        foreach (GameObject npc in authorities)
        {
            Destroy(npc);
        }
    }
    
    public bool IsPlayerBloody()
    {
        return true;
    }

    public void ReloadPlayer()
    {
        player = GameObject.Find("Player");
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

        audioSource = player.GetComponent<AudioSource>();
        audioSource.PlayOneShot(callForHelpSound);

        //spawn authority and set global value
        GameObject.Find("ChasedText").GetComponent<Text>().color = new Color(1, 0.43f, 0.43f, 1);
        Debug.Log("Villager escaped and authority spawned at " + eventPos.ToString());

        Instantiate(authorityPrefab, eventPos, Quaternion.identity);
    }

    /// <summary>
    /// For telling if the authority is in the world and chasing player, useful for U.I Notifications.
    /// </summary>
    public bool IsAuthoritySpawned()
    {

        audioSource = player.GetComponent<AudioSource>();
        audioSource.PlayOneShot(callToPlayerSound);
        return authoritySpawned;
    }
    public void StartGame()
    {
        //GetMenus();
        remainingTime = levelTime;
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        hudMenu.SetActive(false);
        Cursor.visible = false;
        inMenu = false;
        paused = false;
        gameStarted = true;
        playerInChurchTrigger = false;
        Time.timeScale = 1;
        if (gameOver)
        {
            energyLevel = 1.0F;
            killCount = 0;
            gameOver = false;
        }
        else 
        {
            energyLevel *= newNightEnergyMultiplier;
        }
        GameManagerHelper.restarted = true;
        SceneManager.LoadScene("Gold");
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        paused = true;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        paused = false;
    }
    public void ReturnToMenu()
    {
        if(gameOver)
        {
            QuitGame();
            return;
        }
        Time.timeScale = 0;
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        hudMenu.SetActive(false);
        remainingTime = levelTime;
        killCount = 0;
        energyLevel = 1.0F;
        inMenu = true;
        paused = true;
        Cursor.visible = true;
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        hudMenu.SetActive(false);
        inMenu = true;
        paused = true;
        Cursor.visible = true;
        if(!gameOver)
        {
            GameObject.Find("GameOverText").GetComponent<Text>().text = "You survived the night";
            GameObject.Find("ReplayButton").GetComponentInChildren<Text>().text = "Continue";
            GameObject.Find("ExitButton").GetComponentInChildren<Text>().text = "Main Menu";
        }
        else
        {
            GameObject.Find("GameOverText").GetComponent<Text>().text = "Game Over";
            GameObject.Find("ReplayButton").GetComponentInChildren<Text>().text = "Play Again";
            GameObject.Find("ExitButton").GetComponentInChildren<Text>().text = "Quit";
        }
        GameObject.Find("KillsTextCounter").GetComponent<Text>().text = killCount.ToString();
        GameObject.Find("TimeTextCounter").GetComponent<Text>().text = ((int)(remainingTime / 60.0F)).ToString() + ":" + ((int)(remainingTime % 60)).ToString();
        GameObject.Find("ScoreTextCounter").GetComponent<Text>().text = ((int)(killCount + killCount * remainingTime)).ToString();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void GetMenus()
    {
        mainMenu = GameObject.Find("UI").transform.GetChild(0).gameObject;
        gameOverMenu = GameObject.Find("UI").transform.GetChild(1).gameObject;
        pauseMenu = GameObject.Find("UI").transform.GetChild(2).gameObject;
        hudMenu = GameObject.Find("UI").transform.GetChild(3).gameObject;
        killCountText = GameObject.Find("CounterText").GetComponent<Text>();
    }
    void SetupMainMenuButtons()
    {
        Button button = GameObject.Find("StartButton").GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        button = GameObject.Find("QuitButton").GetComponent<Button>();
        button.onClick.AddListener(QuitGame);
    }
    void SetuptGameOverMenuButtons()
    {
        Button button = GameObject.Find("ReplayButton").GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        button = GameObject.Find("ExitButton").GetComponent<Button>();
        button.onClick.AddListener(ReturnToMenu);
    }
    void SetupPauseMenuButtons()
    {
        Button button = GameObject.Find("ResumeButton").GetComponent<Button>();
        button.onClick.AddListener(ResumeGame);
        button = GameObject.Find("RestartButton").GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        button = GameObject.Find("MenuButton").GetComponent<Button>();
        button.onClick.AddListener(ReturnToMenu);
    }


}