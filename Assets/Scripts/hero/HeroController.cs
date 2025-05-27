using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class HeroController : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D bc;

    public Transform cameraTracker;
    Animator anim;
    SpriteRenderer sp;
    public Transform wallCheckTransform;
    public LayerMask wallLayerMask;
    public float wallCheckRadius = 0.5f;
    public float wallSlideSpeedLimit = 2.0f;

    [Header("Crouch and Slide")]
    public GameObject slidingrbHolder;
    public GameObject crouchingrbHolder;
    public float slideSlowRate = 0.985f;
    public float crouchMutiplier = 0.5f;
    public float slideStopFraction = 0.5f;
    BoxCollider2D slidingBC;
    BoxCollider2D crouchingBC;
    bool sliding = false;
    

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        slidingBC = slidingrbHolder.GetComponent<BoxCollider2D>();
        crouchingBC = crouchingrbHolder.GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();

        if(!rb)
        {
            Debug.LogError("Rigidbody2d not found!");
        }

        if(Application.isEditor)
        {
            //on PC, use hardware input (A,D for horizontal move, Space for jump)
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
        } else
        {
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Touch);
        }
        
        // CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Touch);

    }

    public float horAcc = 20.0f;
    public float horAccAir = 2.0f;
    public float horMaxSpeed = 10.0f;
    

    int jumpCount = 2;

    private enum heroState
    {
        idle,
        run,
        jump,
        fall,
        crouch,
        slide,
        wallSlide,
    }

    private heroState currentState = heroState.idle;

    // Update is called once per frame
    void Update()
    {
        updateMovement();
        updateAnim();
        updateCollider();
    }

    public float jumpForce = 10.0f;
    public float wallJumpForce = 10.0f;
    public Vector2 wallJumpDir;



    private void playerJump()
    {
        if (jumpCount <= 0) return;

        jumpCount--;

        if(currentState==heroState.wallSlide || isTouchingWall())
        {
            Vector2 NormalizedDir = wallJumpDir.normalized;
            //wall jump
            rb.velocity = new Vector2(wallJumpForce * NormalizedDir.x, wallJumpForce * NormalizedDir.y);
            if(gameObject.transform.rotation != Quaternion.identity)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }

            //rb.AddForce(new Vector3(wallJumpForce * dir.x, wallJumpForce * dir.y, 0), ForceMode2D.Impulse);
            flipGameObject();
        } else
        {
            //normal jump
            rb.velocity = new Vector2(rb.velocity.x, jumpForce + Mathf.Max(rb.velocity.y, 0.0f));
            //rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }
    }

    private void updateMovement()
    {
        currentState = heroState.idle;

        bool crouching = CrossPlatformInputManager.GetButton("Vertical");
        bool vertical = Mathf.Abs(rb.velocity.y) > 0.1;
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

        if (CrossPlatformInputManager.GetAxis("Horizontal") < 0 && (!crouching || vertical))
        { 
            if (currentState == heroState.idle || currentState == heroState.run)
            {
                rb.AddForce(Vector2.left * horAcc * Time.deltaTime, ForceMode2D.Impulse);
            } else if (currentState == heroState.jump || currentState == heroState.fall)
            {
                rb.AddForce(Vector2.left * horAccAir * Time.deltaTime, ForceMode2D.Impulse);
            }
            
            currentState = heroState.run;
        } else if (CrossPlatformInputManager.GetAxis("Horizontal") > 0 && (!crouching || vertical))
        {
            rb.AddForce(Vector2.right * horAcc * Time.deltaTime, ForceMode2D.Impulse);
            currentState = heroState.run;
        }

        if (crouching && (horizontal > 0 || horizontal < 0) && !vertical)
        {
            //should be moving while crouched
            if (horizontal < 0)
                rb.AddForce(Vector2.left * horAcc * crouchMutiplier * Time.deltaTime, ForceMode2D.Impulse);
            else
                rb.AddForce(Vector2.right * horAcc * crouchMutiplier * Time.deltaTime, ForceMode2D.Impulse);

            if (Mathf.Abs(rb.velocity.x) > horMaxSpeed * (crouchMutiplier) * slideStopFraction)
            {
                currentState = heroState.slide;
                sliding = true;
            }
            else
            {
                currentState = heroState.crouch;
                sliding = false;
            }
            
        }
        else
        {
            sliding = false;
        }

        //speedLimit
        if (Mathf.Abs(rb.velocity.x) > horMaxSpeed && currentState != heroState.crouch && currentState != heroState.slide) //Currently running
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * horMaxSpeed, rb.velocity.y);
        }
        else if (Mathf.Abs(rb.velocity.x) > horMaxSpeed * crouchMutiplier && currentState == heroState.slide) //Slow my speed while sliding
        {
            if (Mathf.Abs(rb.velocity.x) > horMaxSpeed * crouchMutiplier * slideStopFraction)
            {
                if (rb.velocity.x > 0)
                    rb.AddForce(Vector2.left * slideSlowRate * Time.deltaTime,ForceMode2D.Impulse);
                else
                    rb.AddForce(Vector2.right * slideSlowRate * Time.deltaTime, ForceMode2D.Impulse);
            }
            else                                                                                            //Limit while crouching
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * (horMaxSpeed * crouchMutiplier), rb.velocity.y);
                currentState = heroState.crouch;
                sliding = false;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > horMaxSpeed * crouchMutiplier && currentState == heroState.crouch) //Limit while crouching
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * (horMaxSpeed * crouchMutiplier), rb.velocity.y);
        }
       

        if (rb.velocity.y > 0.01f)
        {
            currentState = heroState.jump;
        }
        else if (rb.velocity.y < -0.01f)
        {
            currentState = heroState.fall;
        }

        //wall sliding check
        if(currentState == heroState.fall
            && isTouchingWall()
            //&& (CrossPlatformInputManager.GetAxis("Horizontal")!=0)
            )
        {
            currentState = heroState.wallSlide;
            //refill jump count to 1
            if(jumpCount <=0)
            {
                jumpCount = 1;
            }
            if(Mathf.Abs(rb.velocity.y) > wallSlideSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeedLimit);
            }
        }

        //jump check
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            playerJump();
        }
    }

    private bool isTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckTransform.position, wallCheckRadius, wallLayerMask);
    }

    private void flipGameObject()
    {
        gameObject.transform.rotation = (gameObject.transform.rotation == Quaternion.identity ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
    }


    private void updateAnim()
    {
        if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
        {
            //sp.flipX = true;
            gameObject.transform.rotation = Quaternion.Euler(0,180,0);
            //cameraTracker.transform.rotation = Quaternion.identity;
        }
        else if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
        {
            gameObject.transform.rotation = Quaternion.identity;
            //cameraTracker.transform.rotation = Quaternion.identity;
            //sp.flipX = false;
        }
        switch (currentState)
        {
            case heroState.idle:
                anim.SetInteger("state", 0);
                break;
            case heroState.run:
                anim.SetInteger("state", 1);
                break;
            case heroState.jump:
                anim.SetInteger("state", 2);
                break;
            case heroState.fall:
                anim.SetInteger("state", 3);
                break;
            case heroState.crouch:
                anim.SetInteger("state", 4);
                break;
            case heroState.slide:
                anim.SetInteger("state", 5);
                break;
            case heroState.wallSlide:
                anim.SetInteger("state", 6);
                break;
            default:
                anim.SetInteger("state", 0);
                break;
        }
    }

    private void updateCollider()
    {
        switch (currentState)
        {
            case heroState.crouch:
                bc.enabled = false;
                slidingBC.enabled = false;
                crouchingBC.enabled = true;
                break;
            case heroState.slide:
                bc.enabled = false;
                slidingBC.enabled = true;
                crouchingBC.enabled = false;
                break;
            default:
                bc.enabled = true;
                slidingBC.enabled = false;
                crouchingBC.enabled = false;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down * 0.7f);
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider.gameObject.tag == "Ground")
                {
                    jumpCount = 2;
                }
            }
        }else if ((wallLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            if(isTouchingWall())
            {
                jumpCount = 1;
            }
        }
    }
}
