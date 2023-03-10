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

    public Vector2 walkDirectionVector = Vector2.right;

    public GameObject bloodEffect;
    Animator hitAnmiator;

    public WalkableDirection _walkDirection = WalkableDirection.Right;
    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale *= new Vector2(-1, 1);
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
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
            FlipDirection();
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
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("??????????????????");
        }
    }

    public void OnHit(int damage, Vector2 attackback)
    {
        // LockVelocity = true;
        if(transform.localScale.x>0 && attackback.x>0){
            FlipDirection();
        }else if(transform.localScale.x<0 && attackback.x<0){
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
           FlipDirection();
    }
}
