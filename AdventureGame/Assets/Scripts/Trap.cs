using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private bool _canDamage = true;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController hit = collision.gameObject.GetComponent<PlayerController>();
        if (hit != null) {
            if (_canDamage) {
                hit.Damage(10);
                _canDamage = false;
                hit.ReturnToLastPosition();
                StartCoroutine(ResetDamage());
            }
        }
    }
    IEnumerator ResetDamage() {
        yield return new WaitForSeconds(1.0f);
        _canDamage = true;
    }
}
