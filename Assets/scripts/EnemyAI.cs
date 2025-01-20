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
        Searching,
        Waiting,
        Attacking
    }

    public EnemyState currentState;

    private NavMeshAgent _AIAgent;

    private Transform _playerTransform;
    // Cosas de patrulla: Asegúrate de tener 5 puntos en el array en el Inspector
    [SerializeField] Transform[] _patrolPoints;
    private int _currentPatrolIndex = 0;  // Índice para seguir los puntos de patrullaje secuencialmente

    [SerializeField] Vector2 _patrolAreaSize = new Vector2(5, 5);
    [SerializeField] Transform _patrolAreaCenter;

    // Cosas de detección
    [SerializeField] float _visionRange = 20;
    [SerializeField] float _visionAngle = 120;

    private Vector3 _PlayerLastPosition;
    // Cosas de búsqueda
    float _searchTimer;
    float _searchWaitTime = 15;
    float _serchRadius = 10;

    // Cosas de espera
    private float _waitTimer = 0f;
    private float _waitTime = 5f;  // Tiempo de espera en segundos

    void Awake()
    {
        _AIAgent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetNextPatrolPoint();  // Establecemos el primer punto de patrullaje
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

            case EnemyState.Waiting:
                Wait();
                break;

            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (OnRange())
        {
            currentState = EnemyState.Chasing;
        }

        // Si el enemigo ha llegado al punto de patrullaje
        if (_AIAgent.remainingDistance < 0.5f && !_AIAgent.pathPending)
        {
            // Cambiar al estado Waiting cuando llegue al punto
            currentState = EnemyState.Waiting;
            _waitTimer = 0f;  // Reiniciar el temporizador de espera
        }
    }

    void Chase()
    {
        if (!OnRange())
        {
            currentState = EnemyState.Searching;
        }

        _AIAgent.destination = _playerTransform.position;
    }

    void Search()
    {
        if (OnRange())
        {
            currentState = EnemyState.Chasing;
        }

        _searchTimer += Time.deltaTime;

        if (_searchTimer < _searchWaitTime)
        {
            if (_AIAgent.remainingDistance < 0.5f)
            {
                Vector3 randomPoint;
                if (RandomSearchPoints(_PlayerLastPosition, _serchRadius, out randomPoint))
                {
                    _AIAgent.destination = randomPoint;
                }
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
            _searchTimer = 0;
        }
    }

    void Wait()
    {
        // Esperar 5 segundos antes de volver a patrullar
        _waitTimer += Time.deltaTime;  // Incrementamos el temporizador

        if (_waitTimer >= _waitTime)  // Si el temporizador alcanza 5 segundos
        {
            currentState = EnemyState.Patrolling;
            SetNextPatrolPoint();  // Establecer el siguiente punto de patrullaje
        }
    }

    void Attack()
    {
        // Implementar la lógica de ataque
    }

    bool RandomSearchPoints(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    bool OnRange()
    {
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (_playerTransform.position == _PlayerLastPosition)
        {
            return true;
        }

        if (distanceToPlayer > _visionRange)
        {
            return false;
        }

        if (angleToPlayer > _visionAngle * 0.5f)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                _PlayerLastPosition = _playerTransform.position;
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    // Método para establecer el siguiente punto de patrullaje en orden secuencial
    void SetNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;

        // Establecer el destino al siguiente punto de patrullaje
        _AIAgent.destination = _patrolPoints[_currentPatrolIndex].position;

        // Incrementa el índice del punto de patrullaje
        _currentPatrolIndex++;

        // Si ha llegado al último punto, vuelve al primero
        if (_currentPatrolIndex >= _patrolPoints.Length)
        {
            _currentPatrolIndex = 0;
        }
    }

    void OnDrawGizmos()
    {
        // Gizmos para el área de patrullaje
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolAreaCenter.position, new Vector3(_patrolAreaSize.x, 1, _patrolAreaSize.y));

        // Gizmos para el rango de visión
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _visionRange);

        // Gizmos para el campo de visión
        Gizmos.color = Color.yellow;
        Vector3 fovLine1 = Quaternion.AngleAxis(_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;

        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}





/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public enum EnemyState 
    {

        Patrolling,

        Chasing,

        Searching,

        Waiting,

        Attacking

    }

    public EnemyState currentState;

    private NavMeshAgent _AIAgent;

    private Transform _playerTransform;
    //cosas patrulla
    [SerializeField] Transform[] _patrolPoints;


    [SerializeField] Vector2 _patrolAreaSize = new Vector2(5, 5);
    [SerializeField] Transform _patrolAreaCenter;
    private int _currentPatrolIndex = 0;

    //cosas detecion
    [SerializeField] float _visionRange = 20;

    [SerializeField] float _visionAngle = 120;

    private Vector3 _PlayerLastPosition; 
    // cosas busqueda
    float _searchTimer;
    float _searchWaitTime =15;
    float _serchRadius = 10;

    // Cosas de espera
    private float _waitTimer = 0f;
    private float _waitTime = 5f;  


    void Awake()
    {
        _AIAgent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }



    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetNextPatrolPoint();
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

            case EnemyState.Waiting:
                Wait();
            break;

            case EnemyState.Attacking:
                Attack();
            break;
        }
    }

    void Patrol()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing; 
        }

        //Si el enemigo ha llegado al punto de patrullaje
        if (_AIAgent.remainingDistance < 0.5f)
        {
            currentState = EnemyState.Waiting;  // Cambiar al estado Waiting
            _waitTimer = 0f;  // Reiniciar el temporizador de espera

        }
    }


    
    

    void Chase()
    {
        if(!OnRange())
        {
            currentState = EnemyState.Searching;
        }
        

        _AIAgent.destination = _playerTransform.position;


    }

    void Search()
    {
        if(OnRange())
        {
            currentState = EnemyState.Chasing; 
        }

        _searchTimer += Time.deltaTime;

        if(_searchTimer < _searchWaitTime)
        {
    
            if(_AIAgent.remainingDistance < 0.5f)
            {
                Vector3 randomPoint;
                if(RandomSearchPoints(_PlayerLastPosition, _serchRadius, out randomPoint))
                {
                    _AIAgent.destination = randomPoint;
                }
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
            _searchTimer = 0;
        }
    }

    void Wait()
    {
        // Esperar 5 segundos antes de volver a patrullar
        _waitTimer += Time.deltaTime;  // Incrementamos el temporizador

        if (_waitTimer >= _waitTime)  // Si el temporizador alcanza 5 segundos
        {
            currentState = EnemyState.Patrolling;
            SetNextPatrolPoint();  // Establecer el siguiente punto de patrullaje
        }

    }

    void Attack()
    {

    }


    void SetNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;

        
        _AIAgent.destination = _patrolPoints[_currentPatrolIndex].position;

        
        _currentPatrolIndex++;

        
        if (_currentPatrolIndex >= _patrolPoints.Length)
        {
            _currentPatrolIndex = 0;
        }
    }


    bool RandomSearchPoints(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius; 
       
       NavMeshHit hit;
       if(NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
       {
            point = hit.position;
            return true;
       }

       point = Vector3.zero;
       return false; 
    }

    bool OnRange()
    {

        Vector3 directionToPlayer = _playerTransform.position - transform.position; 
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if(_playerTransform.position == _PlayerLastPosition)
        {
            return true;
        }

        if(distanceToPlayer > _visionRange)
        {
            return false;
        }

        if(angleToPlayer > _visionAngle * 0.5f)
        {
            return false;
        }


        RaycastHit hit;
        if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if(hit.collider.CompareTag("Player"))
            {
                _PlayerLastPosition = _playerTransform.position;

                return true;
            }
            else
            {
                return false;
            }

        }

        return true;
        
    }

    

    void OnDrawGizmos()
    {
        /*foreach(Transform point in _patronPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_patrolAreaCenter.position, new Vector3(_patrolAreaSize.x, 1, _patrolAreaSize.y));


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _visionRange);

        Gizmos.color = Color.yellow;

        Vector3 fovLine1 = Quaternion.AngleAxis(_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-_visionAngle * 0.5f, transform.up) * transform.forward * _visionRange;

        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}*/
