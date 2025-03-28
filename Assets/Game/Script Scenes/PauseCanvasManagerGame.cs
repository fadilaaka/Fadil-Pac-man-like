using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvasManagerGame : MonoBehaviour
{
    [SerializeField] private GameObject _pauseCanvasGame;
    public void ResumeGame()
    {
        _pauseCanvasGame.SetActive(false);
        Time.timeScale = 1;
        AudioListener.volume = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("GameMainMenu");
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}