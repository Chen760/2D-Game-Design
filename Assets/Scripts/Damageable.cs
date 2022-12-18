using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//可破坏的
public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChange;
    Animator animator;
    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            healthChange?.Invoke(_health,MaxHealth);
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    public bool isInvincible = false;

    private float timeSinceHit = 0;
    public float invincibilityTimer = 0.25f;

    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log(value);
        }
    }

    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set { animator.SetBool(AnimationStrings.lockVelocity, value); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTimer)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }

        //  Hit(10);

    }

    public bool Hit(int damage, Vector2 attackback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hit);
            LockVelocity = true;
            damageableHit?.Invoke(damage, attackback);
            CharaterEvents.characterDamaged.Invoke(gameObject,damage);

            return true;
        }
        return false;

    }

    public bool Heal(int heal){
        if(IsAlive && Health < MaxHealth){
            int maxHeal = Mathf.Max(MaxHealth - Health,0);
            int actualHeal = Mathf.Min(maxHeal,heal);
            Health += actualHeal;
            CharaterEvents.characterHealed(gameObject,actualHeal);
            return true;
        }

        return false;
    }

}
