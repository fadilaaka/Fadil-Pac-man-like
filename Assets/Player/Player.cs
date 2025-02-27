using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private float _speed;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _powerUpDuration;
    private Coroutine _powerUpCoroutine;
    public Action OnPowerUpStart;
    public Action OnPowerUpStop;
    private IEnumerator StartPowerUp()
    {
        if (OnPowerUpStart != null)
        {
            OnPowerUpStart();
            Debug.Log("Start Power Up");
        }
        yield return new WaitForSeconds(_powerUpDuration);
        if (OnPowerUpStop != null)
        {
            OnPowerUpStop();
            Debug.Log("Stop Power Up");
        }
    }
    public void PickPowerUp()
    {
        Debug.Log("Pick Power Up");
        if (_powerUpCoroutine != null)
        {
            StopCoroutine(_powerUpCoroutine);
        }

        _powerUpCoroutine = StartCoroutine(StartPowerUp());
    }
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 horizontalDirection = horizontal * _camera.transform.right;
        Vector3 verticalDirection = vertical * _camera.transform.forward;
        verticalDirection.y = 0;
        horizontalDirection.y = 0;

        Vector3 movementDirection = horizontalDirection + verticalDirection;
        if (movementDirection.magnitude > 1)
        {
            movementDirection.Normalize();
        }

        _rigidbody.linearVelocity = _speed * Time.deltaTime * movementDirection;
    }
}
