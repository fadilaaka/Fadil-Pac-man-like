using TMPro;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GameObject itemButton;
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance;
    void Start()
    {
        mainCamera = Camera.main;
        if (itemButton != null)
        {
            Canvas canvas = itemButton.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
    }
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }

        float distance = Vector3.Distance(player.position, transform.position);
        if (itemButton != null)
        {
            Canvas canvas = itemButton.GetComponent<Canvas>();
            if (canvas != null)
            {
                if (distance <= interactionDistance)
                {
                    canvas.enabled = true;
                }
                else
                {
                    canvas.enabled = false;
                }
            }
        }
    }
}
