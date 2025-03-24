using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseCanvas;
    public void Resume()
    {
        _pauseCanvas.SetActive(false);
        Time.timeScale = 1;
        AudioListener.volume = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}