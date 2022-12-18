// using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float airwalkSpeed = 5f;
    public float jumpImpulse = 3f;
    public float dashImpulse = 8f;
    public float rollImpulse = 8f;
    //划墙最大下落速度
    public float maxfallSpeed = 4f;
    public float attackImpulse = 1.5f;
    private bool isDash = false;
    private bool isRoll = false;
    public float counterWallImpulse = 20f;

    private float buffer_jump_counter = 0;    // 跳跃输入缓冲计数器
    private float buffer_jump_max = 0.1f;     // 跳跃输入缓冲最大值
    private float buffer_coyote_counter = 0;    // 跳跃土狼时间计数器
    private float buffer_coyote_max = 0.25f;       // 跳跃土狼时间最大值
    private bool hasJumpForce;
    private float timer;           // 计时器
    private float timer_max = 2f;  // 限定时间

    // private bool isAttack = false;

    UnityEngine.Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirection.IsOnWall)
                {
                    if (touchingDirection.IsGrounded)
                    {
                        return walkSpeed;
                    }
                    else
                    {
                        return airwalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
    }
    // Start is called before the first frame update
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField]
    private bool _isAttack = false;
    public bool IsAttack
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAttack);
        }
        private set
        {
            _isAttack = value;
            animator.SetBool(AnimationStrings.isAttack, value);
        }
    }
    [SerializeField]
    private bool _isBlock = false;
    public bool IsBlock
    {
        get
        {
            return _isBlock;
        }
        private set
        {
            _isBlock = value;
            animator.SetBool(AnimationStrings.isBlock, value);
        }
    }
    [SerializeField]
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get { return animator.GetBool(AnimationStrings.isAlive); }
    }

    Rigidbody2D rb;

    Animator animator;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
    }
    void Start()
    {
        buffer_coyote_counter = buffer_coyote_max;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForJump();

        if (touchingDirection.IsOnWall)
        {
            rb.gravityScale = 0.5f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        //土狼时间 脱离地面一定时间内可以跳跃
        if (buffer_coyote_counter < buffer_coyote_max)
        {
            buffer_coyote_counter += Time.deltaTime;
            if (!hasJumpForce && Input.GetButtonDown("Jump"))
            {
                hasJumpForce = true;
                buffer_coyote_counter = buffer_coyote_max;
                rb.AddForce(new Vector2(0f, jumpImpulse), ForceMode2D.Impulse);
            }
        }


        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
            }
            else
            {
                //缓冲跳跃
                if (buffer_jump_counter < buffer_jump_max)
                {
                    buffer_jump_counter += (1 * Time.deltaTime);
                    Debug.Log(touchingDirection.IsOnWall);
                    if (!hasJumpForce)
                    {
                        if (touchingDirection.IsGrounded)
                        {
                            hasJumpForce = true;
                            //具体施加跳跃力操作
                            rb.AddForce(new Vector2(0f, jumpImpulse), ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            // Debug.Log(IsAttack);
            if (isDash)
            {
                rb.velocity = new Vector2(transform.localScale.x * dashImpulse, rb.velocity.y);
            }
            else if (isRoll)
            {
                rb.velocity = new Vector2(transform.localScale.x * rollImpulse, rb.velocity.y);
            }
            else if (IsAttack && IsMoving)
            {
                rb.velocity = new Vector2(transform.localScale.x * attackImpulse, rb.velocity.y);
            }
            else if (touchingDirection.IsOnWall)
            {
                float fallVelocity = Mathf.Min(-rb.velocity.y, maxfallSpeed);
                rb.velocity = new Vector2(rb.velocity.x, -fallVelocity);
            }
            else
            {
                rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
            }
        }
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    private void CheckForJump()
    {
        if ((touchingDirection.IsGrounded || touchingDirection.IsOnWall) && rb.velocity.y < 0.05f && rb.velocity.y > -0.05f)
        {
            hasJumpForce = false;
            buffer_coyote_counter = 0;
        }
    }

    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }

    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void onDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Damageable damageable = GetComponent<Damageable>();
            animator.SetTrigger(AnimationStrings.dash);
            isDash = true;
            damageable.isInvincible = true;
        }

    }
    private void dashOver()
    {
        isDash = false;
    }

    public void onRoll(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Damageable damageable = GetComponent<Damageable>();
            animator.SetTrigger(AnimationStrings.roll);
            isRoll = true;
            damageable.isInvincible = true;
        }

    }
    private void rollOver()
    {
        isRoll = false;
    }

    public void onBlock(InputAction.CallbackContext context)
    {
        if (context.performed && IsAlive && touchingDirection.IsGrounded)
        {
            IsBlock = true;
        }
        else if (context.canceled)
        {
            IsBlock = false;
        }
    }

    public void onJump(InputAction.CallbackContext context)
    {

        //TODO Check if alive
        if (context.started && (touchingDirection.IsGrounded || touchingDirection.IsOnWall) && CanMove)
        {
            timer = timer_max;
            buffer_jump_counter = 0;
            animator.SetTrigger(AnimationStrings.jump);
            // rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }

    }


    public void onAttack(InputAction.CallbackContext context)
    {
        //TODO Check if alive
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attack);
            // isAttack = true;
        }

    }

    private void attackOver()
    {
        // Debug.Log(IsAttack);
        IsAttack = false;
    }



    public void onRangedAttack(InputAction.CallbackContext context)
    {
        //TODO Check if alive
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttack);
        }

    }

    public void OnHit(int damage, Vector2 attackback)
    {
        // LockVelocity = true;
        rb.velocity = new Vector2(attackback.x, rb.velocity.y + attackback.y);
    }


}
