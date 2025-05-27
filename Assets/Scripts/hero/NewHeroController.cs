using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;

public enum HeroState
{
    Idle,
    Run,
    Jump,
    Fall,
    Crouch,
    Slide,
    WallSlide,
    WallJump,
    Attack,
}

public class NewHeroController : MonoBehaviour
{
    Rigidbody2D _rb;
    BoxCollider2D _bc;
    
    [Header(("Physics"))]
    private PhysicsMaterial2D _phyMat;
    public float playerFriction = 0.2f;

    Animator _anim;
    SpriteRenderer _sp;

    [Header("Multiplayer")] public PhotonView view;
    
    [Header("Wall Check and Ground Check")] 
    public Transform wallCheckTransform;
    public LayerMask wallLayerMask;
    public float wallCheckRadius = 0.5f;
    public float wallSlideSpeedLimit = 2.0f;

    [Header("Crouch and Slide")] 
    public float slideForce = 8.0f;
    public float linearDragNormal = 5;
    public float linearDragSlide = 3;

    public float slideToRunBound = 3.0f;
    public float slideToIdleBound = 2.0f;
	//His changes
    // public GameObject slidingrbHolder;
    // public GameObject crouchingrbHolder;
    // BoxCollider2D _slidingBC;
    // BoxCollider2D _crouchingBC;
	
	//my code
    public GameObject slidingrbHolder;
    public GameObject crouchingrbHolder;
    //public float slideSlowRate = 0.985f;
    //public float crouchMutiplier = 0.5f;
    //public float slideStopFraction = 0.5f;
    BoxCollider2D _slidingBC;
    BoxCollider2D _crouchingBC;


    bool _sliding = false;
    private bool _facingRight = true;

    [Header("Spawn Point")]
    public SpawnPointTracker spawnPointTracker;

    [Header(("Frame Rate Test"))] public int targetFrameRate = 60;
    
    public static NewHeroController instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _bc = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
		//he commented this out
         _slidingBC = slidingrbHolder.GetComponent<BoxCollider2D>();
         _crouchingBC = crouchingrbHolder.GetComponent<BoxCollider2D>();


        _anim = GetComponent<Animator>();
        _sp = GetComponent<SpriteRenderer>();
        _phyMat = _rb.sharedMaterial;
         

        if (!_phyMat)
        {
            Debug.LogError(("physics 2d material not found!"));
        }
        else
        {
            _phyMat.friction = playerFriction;
        }

        if (!_rb)
        {
            Debug.LogError("Rigidbody2d not found!");
        }

        if (Application.isEditor)
        {
            //on PC, use hardware input (A,D for horizontal move, Space for jump)
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
        }
        else 
        {
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Touch);
        }
         // CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
   
        if (spawnPointTracker != null)
        {
            transform.position = spawnPointTracker.UseSpawnPoint?  spawnPointTracker.LastSpawnPoint: transform.position;
        }

        view = GetComponent<PhotonView>();

        // if (view != null && !view.IsMine)
        // {
        //     Color temp = _sp.color;
        //     temp.r = 233.0f;
        //     temp.g = 255.0f;
        //     temp.b = 249.0f;
        //     temp.a = 148.0f;
        //     _sp.color = temp;
        // }
    }

    public float horAcc = 20.0f;
    public float horAccAir = 2.0f;
    public float horMaxSpeed = 10.0f;


    public int jumpCount = 2;

    public HeroState currentState = HeroState.Idle;

    public HeroState lastState = HeroState.Idle;

    //public for debug
    public float _horizontal = 0.0f;
    public float _vertical = 0.0f;
    private bool _clickJump = false;
    private bool _clickAttack = false;
    private bool _crouching = false;



    void updateSpeedLimit()
    {
        switch (currentState)
        {
            case HeroState.Run:
                RunSpeedLimit();
                break;
            // case HeroState.Crouch:
            // case HeroState.Slide:
            //     CrouchSpeedLimit();
            //     break;
                
        }
    }

    void Update()
    {
        if (view!=null && !view.IsMine)
        {
            return;
        }
        Debug.DrawRay(transform.position, Vector3.down*0.1f, Color.white);
        UpdateInput();
        if (currentState != HeroState.WallSlide)
        {
            UpdateFacing();
        }

        _rb.drag = currentState == HeroState.Slide ? linearDragSlide : linearDragNormal;

        switch (currentState)
        {
            case HeroState.Idle:
            case HeroState.Run:
                UpdateIdle();
                RunSpeedLimit();
                break;
            case HeroState.Jump:
            case HeroState.Fall:
                UpdateInAir();
                break;
            case HeroState.WallSlide:
                UpdateWallSlide();
                break;
            case HeroState.WallJump:
                UpdateWallJump();
                break;
            case HeroState.Crouch:
            
                UpdateCrouch();
                // CrouchSpeedLimit();
                break;
            case HeroState.Slide:
                UpdateSlide();
                break;
            case HeroState.Attack:
                UpdateAttack();
                RunSpeedLimit();
                break;
                
            default:
                break;
        }
        lastState = currentState;
        UpdateAnim();
        UpdateCollider();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void UpdateInput()
    {
        float tempHor = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        if (tempHor > 0.99f)
        {
            _horizontal = 1; //do this here so we only need to check once;
        }
        else if (tempHor < -0.99f)
        {
            _horizontal = -1;
        }
        else
        {
            _horizontal = 0.0f;
        }
        
        _clickJump = CrossPlatformInputManager.GetButtonDown("Jump");
        _clickAttack = CrossPlatformInputManager.GetButtonDown("Fire1");
        _vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
        _crouching = (_vertical < 0);
    }

    private void UpdateFacing()
    {
        float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");

        if (horizontal > 0)
        {
            _facingRight = true;
        } else if (horizontal < 0)
        {
            _facingRight = false;
        }

        if (_facingRight)
        {
            gameObject.transform.rotation = Quaternion.identity;
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void UpdateIdle()
    {
        UpdateFacing();
        
        bool vertical = Mathf.Abs(_rb.velocity.y) > 0.1;                     //do this here so we only need to check once
        
        
        //jump check
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            PlayerJump();
            return;
        }
        
        if (CrossPlatformInputManager.GetButtonDown("Vertical"))
        {
            // crouch check 
            switch (currentState)
            {
                case HeroState.Idle:
                    currentState = HeroState.Crouch;
                    return;
                case HeroState.Run:
                    //add slide force
                    StartSlide();
                    currentState = HeroState.Slide;
                    return;
            }
        }
        
        //fall check
        if(_rb.velocity.y < -2.1f)
        {
            currentState = HeroState.Fall;
            return;
        }
        
        //left/right move check
        if (Mathf.Abs(_horizontal) > 0)
        {
            if (_horizontal < 0)
            {
                _rb.AddForce(Vector2.left * horAcc * Time.deltaTime, ForceMode2D.Impulse);
                currentState = HeroState.Run;
            }
            else if (_horizontal > 0)
            {
                _rb.AddForce(Vector2.right * horAcc * Time.deltaTime, ForceMode2D.Impulse);
                currentState = HeroState.Run;
            }
        }
        else 
        {
            currentState = HeroState.Idle;
        }
    }
    
    private void UpdateAttack()
    {
        string tempname = _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (!tempname.StartsWith("attack"))
        {
            //TODO: restore state based on animation clip name
            lastState = currentState;
            currentState = HeroState.Idle;
        }
        if (Mathf.Abs(_horizontal) > 0)
        {
            if (_horizontal < 0)
            {
                _rb.AddForce(Vector2.left * horAcc * Time.deltaTime, ForceMode2D.Impulse);
            }
            else if (_horizontal > 0)
            {
                _rb.AddForce(Vector2.right * horAcc * Time.deltaTime, ForceMode2D.Impulse);
            }
        }
    }

    public float jumpForce = 10.0f;
    public float wallJumpForce = 10.0f;
    public Vector2 wallJumpDir;

    private void UpdateCrouch()//crouching or sliding
    {
        // //Check if should slide or not
        // if (Mathf.Abs(_rb.velocity.x) > horMaxSpeed * (crouchMutiplier) * slideStopFraction)
        // {
        //     currentState = HeroState.Slide;
        //     _sliding = true;
        // }
        // else
        // {
        //     //Movement for crouching
        //     // if (_horizontal < 0)
        //     //     _rb.AddForce(Vector2.left * horAcc * crouchMutiplier * Time.deltaTime, ForceMode2D.Impulse);
        //     // else if (_horizontal > 0)
        //     // {
        //     //     _rb.AddForce(Vector2.right * horAcc * crouchMutiplier * Time.deltaTime, ForceMode2D.Impulse);
        //     // }
        //     UpdateFacing();
        //     Debug.Log("by update crouch");
        //     currentState = HeroState.Crouch;
        //     _sliding = false;
        // }

        if (CrossPlatformInputManager.GetButtonDown("Horizontal"))
        {
            StartSlide();
        }
        if (!_crouching)
        {
            currentState = HeroState.Idle;
        }
    }

    private void StartSlide()
    {
        _rb.velocity = new Vector2(0.0f, _rb.velocity.y);
        _rb.AddForce(new Vector2(_horizontal * slideForce, 0.0f), ForceMode2D.Impulse);
        currentState = HeroState.Slide;
    }

    private void UpdateSlide()
    {
        // press crouch and not pressing left/right: enter crouch
        if (Mathf.Abs(_horizontal) < 0.01f)
        {
            if (Mathf.Abs(_rb.velocity.x) < 0.1f)
            {
                if (_crouching)
                {
                    currentState = HeroState.Crouch;
                }
                else
                {
                    currentState = HeroState.Idle;
                }
            }
        }
        
        // press left/right: enter run
        if (Mathf.Abs(_horizontal) > 0.1f)
        {
            if (Mathf.Abs(_rb.velocity.x) < slideToRunBound)
            {
                currentState = HeroState.Run;
            }
        }
    }

    void RunSpeedLimit()
    {
        if (currentState != HeroState.Run && currentState != HeroState.Idle && currentState != HeroState.Attack)
        {
            return;
        }
        //speedLimit
        if (Mathf.Abs(_rb.velocity.x) > horMaxSpeed) //Currently running
        {
            _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * horMaxSpeed, _rb.velocity.y);
        }
    }
	
    /*void CrouchSpeedLimit()
    {
        //crouch speed limit
        if (Mathf.Abs(_rb.velocity.x) > horMaxSpeed * crouchMutiplier && currentState == HeroState.Slide) //Slow my speed while sliding
        {
            if (Mathf.Abs(_rb.velocity.x) > horMaxSpeed * crouchMutiplier * slideStopFraction)
            {
                if (_rb.velocity.x > 0)
                    _rb.AddForce(Vector2.left * slideSlowRate * Time.deltaTime, ForceMode2D.Impulse);
                else if (_rb.velocity.x < 0)
                {
                    _rb.AddForce(Vector2.right * slideSlowRate * Time.deltaTime, ForceMode2D.Impulse);
                }
            }
            else                                                                                            //Limit while crouching
            {
                _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * (horMaxSpeed * crouchMutiplier), _rb.velocity.y);
                currentState = HeroState.Crouch;
                _sliding = false;
                
            }
        }
        else if (Mathf.Abs(_rb.velocity.x) > horMaxSpeed * crouchMutiplier) //Limit while crouching
        {
            _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * (horMaxSpeed * crouchMutiplier), _rb.velocity.y);
        }
    }*/

    private void PlayerJump()
    {
        if (jumpCount <= 0) return;
        jumpCount--;

        if (TouchingWall())
        {
            // FlipGameObject();
            currentState = HeroState.WallSlide;
        }
        else
        {
            //normal jump
            currentState = HeroState.Jump;
            //rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }
        
        ClearVerticalVelocity();
        _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void PlayerWallJump()
    {
        if (jumpCount <= 0) return;
        //unlike normal jump, wall jump will consume all jump count 
        jumpCount = 0;
        currentState = HeroState.WallJump;
        
        //clear force
        ClearVerticalVelocity();
        
        Vector2 tempForce = wallJumpForce * wallJumpDir;
        tempForce.x *= transform.right.x;
        _rb.AddForce(tempForce, ForceMode2D.Impulse);
        FlipGameObject();
    }

    private float _tempHorSpeed = 0.0f;
    private void UpdateInAir()
    {
        UpdateFacing();
        
        if (Mathf.Abs(_rb.velocity.y) < 0.1f && jumpCount == 2)
        {
            currentState = HeroState.Idle;
        } else if (TouchingWall())
        {
            currentState = HeroState.WallSlide;
        } else if (_rb.velocity.y <= 0)
        {
            currentState = HeroState.Fall;
        }

        if (_clickJump)
        {
            PlayerJump();
            return;
        }

        if (currentState == HeroState.Jump || currentState == HeroState.Fall)
        {
            if (Mathf.Abs(_horizontal) > 0)
            {
                _rb.AddForce(new Vector2(horAccAir * _horizontal * Time.deltaTime, 0.0f), ForceMode2D.Impulse);    
            }

            if (Mathf.Abs(_rb.velocity.x) > horMaxSpeed)
            {
                _rb.velocity = new Vector2(Mathf.Sign(_rb.velocity.x) * horMaxSpeed, _rb.velocity.y);
            }
        }

        if (currentState == HeroState.Fall || currentState == HeroState.Jump || currentState == HeroState.WallJump)
        {
            _tempHorSpeed = _rb.velocity.x;
        }
    }

    private void UpdateWallSlide()
    {
        if (!TouchingWall())
        {
            if (jumpCount < 2)
            {
                currentState = HeroState.Fall;
                return;
            }
        }
        
        jumpCount = Mathf.Max(1, jumpCount);
        
        //fall speed limit
        if (_rb.velocity.y < -wallSlideSpeedLimit)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, -wallSlideSpeedLimit);
        }
        
        if (_clickJump) 
        {
            PlayerWallJump();
            return;
        }

        if (jumpCount == 2 && Mathf.Abs(_rb.velocity.y) < 0.1f)
        {
            currentState = HeroState.Idle;
        }
    }

    private void UpdateWallJump()
    {
        if (TouchingWall())
        {
            currentState = HeroState.WallSlide;
        }
        else
        {
            if (_rb.velocity.y < -0.1f)
            {
                currentState = HeroState.Fall;
            }
        }
    }

    private bool TouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckTransform.position, wallCheckRadius, wallLayerMask);
    }

    private bool TouchingGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down * 0.1f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    private void FlipGameObject()
    {
        gameObject.transform.rotation = (gameObject.transform.rotation == Quaternion.identity ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
        this._facingRight = !this._facingRight;
    }

    private void ClearVerticalVelocity()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0.0f);
        _rb.angularVelocity = 0.0f;
    }


    private void UpdateAnim()
    {
        // if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
        // {
        //     //sp.flipX = true;
        //     gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        //     //cameraTracker.transform.rotation = Quaternion.identity;
        // }
        // else if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
        // {
        //     gameObject.transform.rotation = Quaternion.identity;
        //     //cameraTracker.transform.rotation = Quaternion.identity;
        //     //sp.flipX = false;
        // }
        switch (currentState)
        {
            case HeroState.Idle:
                _anim.SetInteger("state", 0);
                break;
            case HeroState.Run:
                _anim.SetInteger("state", 1);
                break;
            case HeroState.Jump:
            case HeroState.WallJump:
                _anim.SetInteger("state", 2);
                break;
            case HeroState.Fall:
                _anim.SetInteger("state", 3);
                break;
            case HeroState.Crouch:
                _anim.SetInteger("state", 4);
                break;
            case HeroState.Slide:
                _anim.SetInteger("state", 5);
                break;
            case HeroState.WallSlide:
                _anim.SetInteger("state", 6);
                break;
            default:
                _anim.SetInteger("state", 0);
                break;
        }
    }

    private void UpdateCollider()
    {
        Debug.DrawRay(transform.position, Vector3.down * 0.1f);
        switch (currentState)
        {
			//he commented the lsiding and courchingBC out
            case HeroState.Crouch:
                _bc.enabled = false;
                _slidingBC.enabled = false;
                _crouchingBC.enabled = true;
                break;
            case HeroState.Slide:
                _bc.enabled = false;
                _slidingBC.enabled = true;
                _crouchingBC.enabled = false;
                break;
            default:
                _bc.enabled = true;
                _slidingBC.enabled = false;
                _crouchingBC.enabled = false;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down * 0.1f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    jumpCount = Mathf.Max(2, jumpCount);
                    // StartCoroutine(restoreSpeed());
                }
            }
        }
        
        if ((wallLayerMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (TouchingWall())
            {
                jumpCount = Mathf.Max(1, jumpCount);
            }
        }
    }

    public void enterAttackState()
    {
        lastState = currentState;
        currentState = HeroState.Attack;
    }

    IEnumerator restoreSpeed()
    {
        yield return 0;
        _rb.velocity = new Vector2(_tempHorSpeed, _rb.velocity.y);
        Debug.Log("current Speed: " + _tempHorSpeed.ToString());
    }
    

    public void AddForce(Vector2 force)
    {
        _rb.AddForce(force,ForceMode2D.Impulse);
    }

    public void SetNewSpawnPoint(SpawnPoint spawnPoint)
    {
        spawnPointTracker.SetSpawnPoint(spawnPoint);
    }
}
