using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator Animator { get; private set; }
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
        _currentState = PatrolState;
        _currentState.EnterState(this);
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (SpawnPoints.Count > 0)
        {
            Transform randomSpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
            transform.position = randomSpawnPoint.position;
        }
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
}
