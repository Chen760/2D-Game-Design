using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//攻击
public class Attack : MonoBehaviour
{
    Collider2D attackCollider;
    public int attackDamage = 10;
    // public float attackMove = 0f;
    public Vector2 attackback = Vector2.zero;

    [Header("打击感")]
    public float shakeTime;
    public int Pause;
    public float shakeAmount;
    public CamerShaker shaker;


    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            AttackSense.Instance.HitPause(Pause);
            if(shaker != null)
                shaker.RequestShake(shakeAmount,shakeTime);
            // AttackSense.Instance.CameraShake(shakeTime, Strength);
            Vector2 deliverAttackback = transform.parent.localScale.x > 0 ? attackback : new Vector2(-attackback.x, attackback.y);
            bool gotHit = damageable.Hit(attackDamage, deliverAttackback);
            if (gotHit)
                Debug.Log(collision.name + "hit for" + attackDamage);
        }
    }
}
