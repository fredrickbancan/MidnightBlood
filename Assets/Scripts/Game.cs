using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    // SerialisedFields can be accessed via the editor, but are still private
    // Usually, private variables cannot be see by the editor
    [SerializeField]
    private Canvas pauseMenu;

    // Load scene via sceneName string
    // Used for restarting level, too
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Exit()
    {
        Application.Quit();
    }

    void Update()
    {
        // If escape key is pressed, invert active state of UI canvas
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }
    }
}