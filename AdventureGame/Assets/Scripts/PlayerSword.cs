using System.Collections;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    private bool _canDamage = true;
    [SerializeField] PlayerController player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable enemy = other.GetComponent<IDamageable>();
        if (enemy != null)
        {
            if (_canDamage)
            {
                enemy.Damage(player.GetCurrentAttackDamage());
                _canDamage = false;
                StartCoroutine(ResetDamage());
            }
        }
    }

    IEnumerator ResetDamage()
    {
        yield return new WaitForSeconds(1.0f);
        _canDamage = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _canDamage = true;
    }
}
