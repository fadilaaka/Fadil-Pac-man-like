using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private float _speed;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _powerUpDuration;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private int _health;
    [SerializeField] private RectTransform _healthBarContainer;
    [SerializeField] private GameObject _healthPrefab;
    private List<GameObject> _healthUnits = new List<GameObject>();
    [SerializeField] private TMP_Text _healthText;
    public AudioSource _punchAudio;
    private Coroutine _powerUpCoroutine;
    public Action OnPowerUpStart;
    public Action OnPowerUpStop;
    private bool _isPowerUpActive;
    [SerializeField] private GameObject _pauseCanvas;

    private void UpdateUI()
    {
        foreach (var unit in _healthUnits)
        {
            Destroy(unit);
        }
        _healthUnits.Clear();

        for (int i = 0; i < _health; i++)
        {
            GameObject newHealth = Instantiate(_healthPrefab, _healthBarContainer);
            newHealth.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 80, 0);
            _healthUnits.Add(newHealth);
            Animator animator = newHealth.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("Idle", 0, 0);
            }
        }
    }

    public void Dead()
    {
        _health--;
        if (_healthUnits.Count > 0)
        {
            GameObject lastHealth = _healthUnits[_healthUnits.Count - 1];
            Animator animator = lastHealth.GetComponent<Animator>();
            if (animator != null)
            {
                animator.ResetTrigger("HitTrigger");
                animator.SetTrigger("HitTrigger");
                StartCoroutine(DestroyAfterAnimation(animator, lastHealth));
            }
            else
            {
                Destroy(lastHealth);
            }
            _healthUnits.RemoveAt(_healthUnits.Count - 1);
        }
        if (_health > 0)
        {
            transform.position = _respawnPoint.position;
        }
        else
        {
            _health = 0;
            SceneManager.LoadScene("LoseScene");
        }
        UpdateUI();
    }

    private IEnumerator DestroyAfterAnimation(Animator animator, GameObject healthUnit)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(healthUnit);
    }

    private IEnumerator StartPowerUp()
    {
        _isPowerUpActive = true;
        if (OnPowerUpStart != null)
        {
            OnPowerUpStart();
            Debug.Log("Start Power Up");
        }
        yield return new WaitForSeconds(_powerUpDuration);
        _isPowerUpActive = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (_isPowerUpActive)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                _punchAudio.Play();
                collision.gameObject.GetComponent<Enemy>().Dead();
            }
        }
    }

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (_pauseCanvas.activeSelf) return;

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

    private void TogglePause()
    {
        if (!_pauseCanvas.activeSelf)
        {
            _pauseCanvas.SetActive(true);
            AudioListener.volume = 0f;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}