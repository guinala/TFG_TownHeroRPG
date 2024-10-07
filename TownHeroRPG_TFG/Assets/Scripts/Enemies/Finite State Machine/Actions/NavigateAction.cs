using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NavigateAction : Enemy_Action
{
    [Header("Config")]
    [SerializeField] private bool debug;
    [SerializeField] private bool randomMovement;
    [SerializeField] private bool limitMovement;
    [SerializeField] private bool tileMovement;
    private Vector2 areaMinBounds;
    private Vector2 areaMaxBounds;

    [Header("Valores")]
    [SerializeField] private float speed;
    [SerializeField] private Vector2 range;
    [SerializeField] private float minCheckDistance = 0.5f;
    
    [Header("Random Limit Area")]
    [SerializeField] private Vector2 minAreaSize;  
    [SerializeField] private Vector2 maxAreaSize;   

    [Header("Obstaculos")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float detectionRadius;

    private Enemy_FSM enemyFsm;
    private Animator animator;
    
    private Vector3 position;
    private Vector3 direction;
    private bool isPaused;
    private int remainingDirections;
    
    private readonly int directionX = Animator.StringToHash("MovementX");
    private readonly int directionY = Animator.StringToHash("MovementY");
    //private readonly int defeated = Animator.StringToHash("Defeated");
    private readonly int walking = Animator.StringToHash("Walking");

    private void Awake()
    {
        enemyFsm = GetComponent<Enemy_FSM>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetNewCount();
        GetNewDirection();
        animator.SetBool("Walking", true);
    }

    public override void ExecuteAction()
    {
        if (isPaused) return;
        
        direction = (position - transform.position).normalized;
        animator.SetFloat(directionX, direction.x);
        animator.SetFloat(directionY, direction.y);
        transform.Translate(direction * (speed * Time.deltaTime));
        if (CanGetNewDirection())
        {
            remainingDirections--;
            if (remainingDirections <= 0)
            {
                StartCoroutine(PauseMovement());
            }
            
            else
            {
                GetNewDirection();
            }
        }
    } 
    
    // Método que se llama en el editor cuando se cambia algún valor
    private void OnValidate()
    {
        SetRandomBounds();  // Generar los límites aleatorios en el editor
    }
    
    // Genera límites aleatorios para el área de movimiento
    private void SetRandomBounds()
    {
        // Generar un tamaño aleatorio para el ancho y alto del área
        float areaWidth = Random.Range(minAreaSize.x, maxAreaSize.x);
        float areaHeight = Random.Range(minAreaSize.y, maxAreaSize.y);

        // El enemigo debe estar en el centro del área, así que calculamos los límites en función de su posición
        Vector2 enemyPosition = transform.position;

        areaMinBounds.x = enemyPosition.x - areaWidth / 2f;
        areaMaxBounds.x = enemyPosition.x + areaWidth / 2f;

        areaMinBounds.y = enemyPosition.y - areaHeight / 2f;
        areaMaxBounds.y = enemyPosition.y + areaHeight / 2f;
    }
    
    private void GetNewDirection()
    {
        if (randomMovement)
        {
            position = transform.position + GetRandomDirection();
            if(limitMovement)
                ClampPositionToBounds();
        } 

        if (tileMovement)
        {
            //posicionMovimiento = enemigoFsm.RoomParent.GetAvailableTile();
        }
    }
    
    private Vector3 GetRandomDirection()
    {
        float randomX = Random.Range(-range.x, range.x);
        float randomY = Random.Range(-range.y, range.y);
        return new Vector3(randomX, randomY, 0f);
    }

    private bool CanGetNewDirection()
    {
        if (Vector3.Distance(transform.position, position) < minCheckDistance)
        {
            return true;
        }

        Collider2D[] results = new Collider2D[10];
        
        int collisions = Physics2D.OverlapCircleNonAlloc(transform.position,
            detectionRadius, results, obstacleMask);
        
        if (collisions > 0)
        {
            for (int i = 0; i < collisions; i++)
            {
                if (results[i] != null)
                {
                    Vector3 dirOposite = -direction;
                    transform.position += dirOposite * 0.1f;
                    return true;
                }
            }
        }

        return false;
    }

    private void SetNewCount()
    {
        remainingDirections = Random.Range(1, 5);
    }
    
    private IEnumerator PauseMovement()
    {
        isPaused = true;
        animator.SetBool("Walking", false);
        yield return new WaitForSeconds(2f);
        SetNewCount();
        isPaused = false;
        animator.SetBool("Walking", true);
        GetNewDirection();
    }

    private void ClampPositionToBounds()
    {
        position.x = Mathf.Clamp(position.x, areaMinBounds.x, areaMaxBounds.x);
        position.y = Mathf.Clamp(position.y, areaMinBounds.y, areaMaxBounds.y);
    }
    
    private void OnDrawGizmos()
    {
        if (debug == false) return;
        if (randomMovement)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, range * 2);
        }
        
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, position);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (limitMovement)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((Vector3)((areaMaxBounds + areaMinBounds) / 2), new Vector3(areaMaxBounds.x - areaMinBounds.x, areaMaxBounds.y - areaMinBounds.y, 0f));
        }
    }
}
