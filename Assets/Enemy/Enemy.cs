using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator Animator;
    private BaseState _currentState;

    public void SwitchState(BaseState state)
    {
        _currentState.ExitState(this);
        _currentState = state;
        _currentState.EnterState(this);
    }

    public PatrolState PatrolState = new PatrolState();
    public ChaseState ChaseState = new ChaseState();
    public RetreatState RetreatState = new RetreatState();
    [SerializeField] public float ChaseDistance;
    [SerializeField] public Player Player;
    [SerializeField] public List<Transform> Waypoints = new List<Transform>();
    [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();
    [HideInInspector] public NavMeshAgent NavMeshAgent;
    public AudioSource _punchAudio;

    private void StartRetreating()
    {
        SwitchState(RetreatState);
    }

    private void StopRetreating()
    {
        SwitchState(PatrolState);
    }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _currentState = PatrolState;
        _currentState.EnterState(this);
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        NavMeshAgent.enabled = false;
        if (SpawnPoints.Count > 0)
        {
            Transform randomSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
            transform.position = randomSpawnPoint.position;
        }
        NavMeshAgent.enabled = true;
        if (Player != null)
        {
            Player.OnPowerUpStart += StartRetreating;
            Player.OnPowerUpStop += StopRetreating;
        }
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.UpdateState(this);
        }
    }

    private IEnumerator ApplyKnockbackAndFall(Vector3 direction, float speed)
    {
        float knockbackDuration = 0.5f;
        float rotationDuration = 1f;
        float knockbackElapsed = 0f;
        float rotationElapsed = 0f;

        Quaternion targetRotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        float rotationSpeed = 2f;

        while (knockbackElapsed < knockbackDuration || rotationElapsed < rotationDuration)
        {
            if (knockbackElapsed < knockbackDuration)
            {
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
                knockbackElapsed += Time.deltaTime;
            }

            if (rotationElapsed < rotationDuration)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                rotationElapsed += Time.deltaTime;
            }

            Vector3 newPosition = transform.position;
            newPosition.y = 0.5f;
            transform.position = newPosition;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
    public void Dead()
    {
        NavMeshAgent.enabled = false;
        Animator.SetTrigger("Die");
        Vector3 fallDirection = -transform.forward;
        float knockbackSpeed = 5f;
        StartCoroutine(ApplyKnockbackAndFall(fallDirection, knockbackSpeed));
        StartCoroutine(RespawnCoroutine());
    }
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(3f);
        if (SpawnPoints.Count > 0)
        {
            Transform randomSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
            transform.position = randomSpawnPoint.position;
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
        NavMeshAgent.enabled = true;
        SwitchState(PatrolState);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_currentState != RetreatState)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _punchAudio.Play();
                collision.gameObject.GetComponent<Player>().Dead();
            }
        }
    }
}