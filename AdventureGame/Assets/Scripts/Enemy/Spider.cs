using Unity.Cinemachine;
using UnityEngine;

public class Spider : Enemy, IDamageable
{
    public int Health {get; set;}
    public override void Init()
    {
        base.Init();
        Health = base.health;
    }
    public override void Movement()
    {
        base.Movement();
        Vector3 direction = player.transform.position - transform.position;
        float distanceToPlayer = direction.magnitude;
        if (distanceToPlayer < attackRange)
        {
            anim.SetBool("InCombat", true);
        }
        if (direction.x > 0 && anim.GetBool("InCombat")) {
            sprite.flipX = false;
        } else if (direction.x < 0 && anim.GetBool("InCombat")) {
            sprite.flipX = true;
        }
    }
    public void Damage(int dmg) {
        Health -= dmg;
        anim.SetTrigger("Hit");
        anim.SetBool("InCombat", true);
        if (Health < 1) {
            Destroy(this.gameObject);
        }
    }
}
