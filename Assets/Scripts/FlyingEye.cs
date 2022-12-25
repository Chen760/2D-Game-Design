using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D),  typeof(Damageable))]
public class FlyingEye : MonoBehaviour
{
    public float walkSpeed = 3f;
    private float waypointReachedDistance = 0.1f;
    public DetectionZone attackZone;
    public List<Transform> waypoints;
    int waypointNum = 0;
    Transform nextWaypoint;
    Rigidbody2D rb;
    Animator animator;
    Damageable damageable;

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
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }
    // Start is called before the first frame update
    void Start()
    {
        // nextWaypoint = waypoints[waypointNum];
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

    }
    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }else{
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0,rb.velocity.y);
        }

    }

    private void Flight()
    {
        // Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        // rb.velocity = directionToWaypoint * walkSpeed;
        // UpdateDirection();

        // if (distance <= waypointReachedDistance)
        // {
        //     waypointNum++;
        //     if (waypointNum >= waypoints.Count)
        //     {
        //         waypointNum = 0;
        //     }
        //     nextWaypoint = waypoints[waypointNum];
        // }


    }

    private void UpdateDirection()
    {
        Vector3 locScal = transform.localScale;
        if(transform.localScale.x>0){
            if(rb.velocity.x<0){
                transform.localScale = new Vector3(-1*locScal.x,locScal.y,locScal.z);
            }

        }else{
            if(rb.velocity.x>=0){
                transform.localScale = new Vector3(-1*locScal.x,locScal.y,locScal.z);
            }

        }
    }

    public void OnHit(int damage, Vector2 attackback)
    {
        // LockVelocity = true;
        rb.velocity = new Vector2(attackback.x, rb.velocity.y + attackback.y);
    }

}
