using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<Pickable> _pickableList = new List<Pickable>();
    private int _coinCount = 0;
    private List<Vector3> positions = new List<Vector3>();
    [SerializeField] private Player _player;
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
            _coinCount--;
            if (_coinCount <= 0)
            {
                Debug.Log("Win");
            }
        }

        if (pickable._pickableType == PickableType.PowerUp)
        {
            _player?.PickPowerUp();
        }
    }
}
