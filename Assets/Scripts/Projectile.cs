using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//远程攻击
public class Projectile : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 moveSpeed = new Vector2(3f,0);
    public Vector2 attackback = new Vector2(0,0);

    Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(moveSpeed.x * transform.lossyScale.x,moveSpeed.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
         Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliverAttackback = transform.localScale.x > 0 ? attackback : new Vector2(-attackback.x, attackback.y);
            bool gotHit = damageable.Hit(attackDamage, deliverAttackback);
            if (gotHit)
                Debug.Log(collision.name + "hit for" + attackDamage);
                Destroy(gameObject);
        }
    }
}
