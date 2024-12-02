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

    [SerializeField] Transform[] _patronPoints;

    void Awake()
    {
        _AIAgent = GetComponent<NavMeshAgent>();
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
        if(_AIAgent.remainingDistance < 0.5f)
        {
            SetRandomPatronPoint();
        }
    }

    void Chase()
    {


    }

    void SetRandomPatronPoint()
    {
        _AIAgent.destination = _patronPoints[Random.Range(0, _patronPoints.Length)].position;
    }

    /*void OnDrawGizmos()
    {
        foreach(Transform point in _patronPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawingSphere(point.position, 1);
        }
    }*/
}
