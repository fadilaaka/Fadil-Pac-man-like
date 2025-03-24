using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableScript : MonoBehaviour
{

    [SerializeField] public PickableTypeScript _pickableType;
    public Action<PickableScript> OnPicked;

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
