using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{

    [SerializeField] public PickableType _pickableType;
    public Action<Pickable> OnPicked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (OnPicked != null)
            {
                OnPicked(this);
            }
            Debug.Log("Trigger Pickup: " + _pickableType);
        }
    }
}
