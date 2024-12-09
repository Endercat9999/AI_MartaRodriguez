using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public enum EnemyState 
    {

        Patrolling,

        Chasing,

        Searching

    }

    public EnemyState currentState;

    private NavMeshAgent _AIAgent;

    private Transform _playerTransform;

    [SerializeField] Transform[] _patronPoints;

    [SerializeField] float _visionRange = 20;

    [SerializeField] float _visionAngle = 120;

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

            case EnemyState.Searching:
                Search();
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

    void Search()
    {

    }

    bool OnRange()
    {

        Vector3 directionToPlayer = _playerTransform.position - transform.position; 
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if(distanceToPlayer < _visionRange)
        {
            if(angleToPlayer < _visionAngle * 0.5f)
            {
                return true; 
            }
            else
            {
                return false; 
            }
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

        Gizmos.color = Color.yellow;

        Vector3 fovLine1 = Quaternion.AngleAxis(_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;

        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
