using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;

    [Header("Movement")]
    private Rigidbody _rigidbody;
    [SerializeField] private Transform orientation;
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
    [SerializeField] private GameObject _pauseCanvasGame;
    private Vector3 movementDirection;
    float horizontalInput;
    float verticalInput;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundFriction = 5f;
    public LayerMask whatIsGround;
    bool grounded;

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
            newHealth.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 50, 0);
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
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }
    private void MovePlayer()
    {
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        _rigidbody.AddForce(movementDirection.normalized * _speed * 10f, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _speed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _speed;
            _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (_pauseCanvasGame.activeSelf) return;

        _playerAnimator.SetFloat("Velocity", _rigidbody.linearVelocity.magnitude);

        grounded = Physics.CheckSphere(groundCheck.position, groundFriction, whatIsGround);

        MyInput();
        SpeedControl();

        if (grounded)
            _rigidbody.linearDamping = groundFriction;
        else
            _rigidbody.linearDamping = 0;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void TogglePause()
    {
        if (!_pauseCanvasGame.activeSelf)
        {
            _pauseCanvasGame.SetActive(true);
            AudioListener.volume = 0f;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}