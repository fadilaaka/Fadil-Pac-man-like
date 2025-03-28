using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainMenu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("GameMainMenu Loaded. Ensuring audio is active.");
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }
}