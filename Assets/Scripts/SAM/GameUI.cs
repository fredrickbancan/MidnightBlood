using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    // Instance
    public static GameUI instance = null;

    private Canvas pauseMenu;
    private Canvas mainMenu;
    private Canvas gameOverMenu;

    bool started = false;
    bool inMenu = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            started = true;
            inMenu = true;
        }
        Debug.Log("Instance");
    }

    private void Start()
    {
        GetCanvas();
        if (started)
        {
            mainMenu.enabled = true;
            inMenu = true;
        }
        Debug.Log("Start");
    }

    void Update()
    {
        // If escape key is pressed, invert active state of UI canvas
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            if (pauseMenu)
            {
                pauseMenu.enabled = !pauseMenu.enabled;
            }
        }
    }

    public void LoadGame()
    {
        Time.timeScale = 1;
        mainMenu.enabled = false;
        inMenu = false;
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverMenu.enabled = true;
        inMenu = true;
    }
    public void RestartLevel(string sceneName)
    {
        Time.timeScale = 1;
        Debug.Log("Restart");
        SceneManager.LoadScene(sceneName);
    }
    public void Exit()
    {
        Application.Quit();
    }
    void GetCanvas()
    {
        mainMenu = GameObject.Find("Main Menu Canvas").GetComponent<Canvas>();
        mainMenu.enabled = false;
        pauseMenu = GameObject.Find("Pause Menu Canvas").GetComponent<Canvas>();
        pauseMenu.enabled = false;
        gameOverMenu = GameObject.Find("Game Over Canvas").GetComponent<Canvas>();
        gameOverMenu.enabled = false;
    }
}