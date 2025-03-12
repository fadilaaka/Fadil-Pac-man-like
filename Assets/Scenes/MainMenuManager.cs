using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void exit()
    {
        Application.Quit();
    }
}
