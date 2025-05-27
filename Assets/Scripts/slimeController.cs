
using UnityEngine;
using System.Collections;

public class slimeController : MonoBehaviour
{
    // Start is called before the first frame update

    public float health = 3.0f;

    [Header("Movement Controlls")]
    public float moveSpeed;
    public float idleTime;
    bool moveLeft = false;
    float distanceToMove;
    float distanceAlreadyMoved;
    float timeInIdleAlready = 0;
    public float minWalkDistance;
    public float maxWalkDistance;
    public GameObject groundChecker;

    [Header("Attack")]
    public float attackMoveForce;
    public float delayBeforeAttack = .8f;
    public float damage;
    public float cd;
    public float nockBackForce = 1f;
    bool canAttack = true;
    public bool canDoDamage = true;

    [Header("Player Detection")]
    public float viewDistance;
    public float attackDistance;
    public LayerMask playerMask;
    public LayerMask groundMask;
    public NewHeroController hero;

    Rigidbody2D rb;
    public event System.Action onDeath;
    bool isDead = false;

    private enum slimeState
    {
        idle = 0,
        idleMove = 1,
        move = 2,
        busy = 3
    }

    slimeState currentState = slimeState.idle;
    private Animator slimeAnimator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        slimeAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case slimeState.idle:
                idleUpdate();
                break;
            case slimeState.idleMove:
                idleMoveUpdate();
                break;
            case slimeState.move:
                moveUpdate();
                break;
            case slimeState.busy:
                break;
            default:
                break;
        }
        slimeAnimator.SetInteger("state", (int)currentState);
    }

    public GameObject gameController;
    public void takeDamage(float damage)
    {
        Debug.Log("take damage");
        health -= damage;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            slimeAnimator.SetBool("die", true);
            Debug.Log("I am dead!");
            onDeath?.Invoke();
            playDieAnimation();
            
            //for firebase test
           /* if (gameController && gameController.GetComponent<FirebaseDatabaseController>())
            {
                FirebaseDatabaseController controller = gameController.GetComponent<FirebaseDatabaseController>();
                controller.addScore(1);
            }*/
        }
        else
        {
            if (!isDead)
            {
                slimeAnimator.Play("hurt");
            }
        }
    }

    public void playDieAnimation()
    {
        slimeAnimator.Play("die");
    }

    public void destroySelf()
    {
        Destroy(this.gameObject);
    }

    private void idleUpdate()
    {
        rb.velocity = Vector2.zero;
        timeInIdleAlready += Time.deltaTime;
        if (timeInIdleAlready > idleTime)
        {
            moveLeft = Random.Range(0, 2) == 0? false : true;
            if (moveLeft)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else
                transform.rotation = Quaternion.Euler(0, 180, 0);
            distanceToMove = Random.Range(minWalkDistance, maxWalkDistance);
            distanceAlreadyMoved = 0;
            currentState = slimeState.idleMove;
        }

        if (canAttack)
        {
            SetHeroController();
            if (hero != null)
            {
                if (Vector2.Distance(transform.position, hero.transform.position) < viewDistance)
                {
                    Vector2 direction = (hero.transform.position - transform.position).normalized;
                    RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction, viewDistance, playerMask);
                    Debug.DrawRay(transform.position, direction * viewDistance, Color.red);
                    if (raycastHit.collider != null)
                    {
                        if (raycastHit.collider.tag == "Player")
                        {
                            currentState = slimeState.move;
                        }
                    }
                }
            }
        }
    }

    private void idleMoveUpdate()
    {
        if (Physics2D.Raycast(groundChecker.transform.position, Vector2.down, 1f,groundMask).collider == null)
        {
            moveLeft = !moveLeft;
            if (moveLeft)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else
                transform.rotation = Quaternion.Euler(0, 180, 0);
            distanceAlreadyMoved = 0;
        }

        if (moveLeft)
        {
            rb.velocity = (Vector2.left * moveSpeed);
            distanceAlreadyMoved -= rb.velocity.x * Time.deltaTime;
        }
        else
        {
            rb.velocity = (Vector2.right * moveSpeed);
            distanceAlreadyMoved += rb.velocity.x * Time.deltaTime;
        }

        if (distanceAlreadyMoved > distanceToMove)
        {
            timeInIdleAlready = 0;
            currentState = slimeState.idle;
        }


        SetHeroController();
        //This effects when to start chasing player. It needs to be ipudated to work on multiple players
        if (hero != null)
        {
            if (Vector2.Distance(transform.position, hero.transform.position) < viewDistance)
            {
                Vector2 direction = (hero.transform.position - transform.position).normalized;
                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction, viewDistance, playerMask);
                Debug.DrawRay(transform.position, direction * viewDistance, Color.red);
                if (raycastHit.collider == null)
                {
                    currentState = slimeState.move;
                }
                else if (raycastHit.collider.tag == "Player")
                {
                    currentState = slimeState.move;
                }
            }
        }  
    }

    private void moveUpdate()
    {
        SetHeroController();

        

        if (hero.transform.position.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            rb.velocity = (Vector2.right * moveSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.velocity = (Vector2.left * moveSpeed);
        }

        if (Vector2.Distance(transform.position, hero.transform.position) > viewDistance)
        {
            currentState = slimeState.idle;
            timeInIdleAlready = 0;
            return;
        }
        else
        {
            Vector2 direction = (hero.transform.position - transform.position).normalized;
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction, viewDistance, playerMask);
            Debug.DrawRay(transform.position, direction * viewDistance, Color.red);
            if (raycastHit.collider == null)
            {
                currentState = slimeState.idle;
                timeInIdleAlready = 0;
                return;
            }
            else if (raycastHit.collider.tag != "Player")
            {
                currentState = slimeState.idle;
                timeInIdleAlready = 0;
                return;
            }

            //These things need updating for when there is multiple players
            if (Vector2.Distance(transform.position, hero.transform.position) < attackDistance)
            {
                currentState = slimeState.busy;
                StartCoroutine(Attack());
            }
        }

    }
 
    IEnumerator Attack()
    {
        float playerPositionAtStart = hero.transform.position.x;

        slimeAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(delayBeforeAttack);

        //This is useed so that the slime can only hurt the player 1 time per attack
        canDoDamage = true;

        yield return new WaitForSeconds(0.1f);
        if (playerPositionAtStart > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            rb.AddRelativeForce(Vector2.right * attackMoveForce, ForceMode2D.Impulse);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rb.AddRelativeForce(Vector2.left * attackMoveForce, ForceMode2D.Impulse);
        }
        StartCoroutine(StopDamage());
        yield return new WaitForSeconds(cd);
        currentState = slimeState.move;
    }

    IEnumerator StopDamage()
    {
        yield return new WaitForSeconds(.5f);
        canDoDamage = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDoDamage)
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            NewHeroController thisHero = collision.GetComponent<NewHeroController>();
            if (playerHealth)
            {
                playerHealth.TakeDamage(damage);
                if (transform.position.x > hero.transform.position.x)
                {
                    thisHero?.AddForce(Vector2.left * nockBackForce);
                }
                else
                {
                    thisHero?.AddForce(Vector2.right * nockBackForce);
                }
                canDoDamage = false;
            }
        }
        
    }

    private void SetHeroController()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewDistance, playerMask);
        foreach (Collider2D thisPlayer in hits)
        {
            NewHeroController thisPlayerController = thisPlayer.GetComponent<NewHeroController>();
            if (thisPlayerController == null)
                continue;

            if (thisPlayerController == hero)
                continue;

            if (hero == null)
            {
                hero = thisPlayerController;
                continue;
            }

            if (Vector2.Distance(transform.position, thisPlayerController.transform.position) < Vector2.Distance(transform.position, hero.transform.position))
            {
                hero = thisPlayerController;
            }

        }
    }
}
