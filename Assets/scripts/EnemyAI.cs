using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public enum EnemyState 
    {

        Patrolling,

        Chasing

    }

    public EnemyState currentState;

    private NavMeshAgent _AIAgent;

    private Transform _playerTransform;

    [SerializeField] Transform[] _patronPoints;

    [SerializeField] float _visionRange = 20;

    void Awake()
    {
        _AIAgent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }



    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetRandomPatronPoint();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
            break;

            case EnemyState.Chasing:
                Chase();
            break;
        }
    }

    void Patrol()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing; 
        }

        if(_AIAgent.remainingDistance < 0.5f)
        {
            SetRandomPatronPoint();
        }
    }

    void Chase()
    {
        if(!OnRange())
        {
            currentState = EnemyState.Patrolling;
        }

        _AIAgent.destination = _playerTransform.position;


    }

    bool OnRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if(distanceToPlayer < _visionRange)
        {
            return true;

        }
        else
        {
            return false; 
        }
    }

    void SetRandomPatronPoint()
    {
        _AIAgent.destination = _patronPoints[Random.Range(0, _patronPoints.Length)].position;
    }


    void OnDrawGizmos()
    {
        foreach(Transform point in _patronPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _visionRange);
    }
}
