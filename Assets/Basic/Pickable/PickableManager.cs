using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickableManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<Pickable> _pickableList = new List<Pickable>();
    private int _coinCount = 0;
    private List<Vector3> positions = new List<Vector3>();
    [SerializeField] private Player _player;
    [SerializeField] private ScoreManager _scoreManager;
    public AudioSource _coinAudio;
    public AudioSource _powerUpAudio;
    void Start()
    {
        InitPickableList();
    }

    private void InitPickableList()
    {
        Pickable[] pickableObjects = GameObject.FindObjectsByType<Pickable>(FindObjectsSortMode.None);
        for (int i = 0; i < pickableObjects.Length; i++)
        {
            _pickableList.Add(pickableObjects[i]);
            pickableObjects[i].OnPicked += OnPickablePicked;
            if (pickableObjects[i].name.Contains("Coin"))
            {
                _coinCount++;
            }

            positions.Add(pickableObjects[i].transform.position);
        }
        _scoreManager.SetMaxScore(_coinCount);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 temp = positions[i];
            int randomIndex = Random.Range(i, positions.Count);
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }
        for (int i = 0; i < pickableObjects.Length; i++)
        {
            pickableObjects[i].transform.position = positions[i];
        }
    }

    private void OnPickablePicked(Pickable pickable)
    {
        _pickableList.Remove(pickable);
        Destroy(pickable.gameObject);
        if (pickable.name.Contains("Coin"))
        {
            _coinAudio.Play();
            _coinCount--;
            if (_coinCount <= 0)
            {
                // Debug.Log("Win");
                SceneManager.LoadScene("WinScene");
            }
            if (_scoreManager != null)
            {
                _scoreManager.AddScore(1);
            }
        }

        if (pickable._pickableType == PickableType.PowerUp)
        {
            _powerUpAudio.Play();
            _player?.PickPowerUp();
        }
    }

    private void Update()
    {
        for (int i = 0; i < _pickableList.Count; i++)
        {
            float delay = i * 0.3f;
            float timeWithDelay = Time.time + delay;

            Vector3 position = _pickableList[i].transform.position;
            position.y = Mathf.PingPong(timeWithDelay * 0.5f, 1.2f - 0.6f) + 0.6f;
            _pickableList[i].transform.position = position;

            float rotationSpeed = 100f;
            _pickableList[i].transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}
