using UnityEngine;

public class BossHitBoxAtkController : MonoBehaviour
{
    private int damage;

    public void SetDamage(int _damage) {
        damage = _damage;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.tag == "Player")
        {
            //Debug.Log(collision.tag + " " + damage);
            gameObject.GetComponentInParent<BossController>().SetHitPlayer();
            if (gameObject.GetComponentInParent<BossController>().BerserkerMode)
            {
                damage *= 2;
                
                 
            }
            gameObject.GetComponentInParent<BossController>().PlayerController.Damage(damage);
        }
    }
}
