using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class Knight : MonoBehaviour
{
    public float walkSpeed = 3f;
    private float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone groundZone;
    Rigidbody2D rb;
    TouchingDirection touchingDirection;
    Animator animator;
    Damageable damageable;
    public enum WalkableDirection { Right, Left }

    public WalkableDirection _walkDirection;
    public Vector2 walkDirectionVector = Vector2.right;

    public GameObject bloodEffect;
    Animator hitAnmiator;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            // Debug.Log(value);
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == WalkableDirection.Right)
                {
                    Debug.LogError("向右走");
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    Debug.LogError("向左走");
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;

        }
    }

    public bool _hasTarget = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            {
                _hasTarget = value;
                animator.SetBool(AnimationStrings.hasTarget, value);
            }
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        hitAnmiator= transform.GetChild(0).GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if(AttackCooldown>0){
            AttackCooldown -= Time.deltaTime;
        }

    }
    private void FixedUpdate()
    {
        if (touchingDirection.IsOnWall && touchingDirection.IsGrounded)
        {
            // Debug.Log(touchingDirection.IsOnWall);
            // Debug.LogError(touchingDirection.IsOnWall);
            Debug.Log("碰墙转向");
            FlipDirection();
            Debug.Log("碰墙转向");
        }
        if (!damageable.LockVelocity)
        {
            if (CanMove && touchingDirection.IsGrounded)
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }


    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            Debug.LogError("左转");
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            Debug.LogError("右转");
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("当前朝向错误");
        }
    }

    public void OnHit(int damage, Vector2 attackback)
    {
        // LockVelocity = true;
        if(transform.localScale.x>0 && attackback.x>0){
            Debug.Log("受击转向");
            FlipDirection();
        }else if(transform.localScale.x<0 && attackback.x<0){
            Debug.Log("受击转向");
            FlipDirection();
        }
        hitAnmiator.SetTrigger(AnimationStrings.hit);
        // Debug.Log(attackback);
        // rb.velocity = new Vector2(0, 0);
        rb.AddForce(new Vector2(attackback.x, rb.velocity.y + attackback.y), ForceMode2D.Impulse);
        Instantiate(bloodEffect,transform.position,Quaternion.identity);
    }

    public void OnNoGroundDetected()
    {
        // LockVelocity = true;
        if(touchingDirection.IsGrounded)
           Debug.Log("边缘转向");
           FlipDirection();
    }
}
