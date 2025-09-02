using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private bool _canDamage = true;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController hit = other.GetComponent<PlayerController>();
        if (hit != null) {
            if (_canDamage) {
                hit.Damage(10);
                _canDamage = false;
                StartCoroutine(ResetDamage());
            }
        }
    }
    IEnumerator ResetDamage() {
        yield return new WaitForSeconds(1.0f);
        _canDamage = true;
    }
}
