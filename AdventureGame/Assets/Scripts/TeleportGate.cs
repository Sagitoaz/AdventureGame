using UnityEngine;

public class TeleportGate : MonoBehaviour
{
    [SerializeField] private Transform _targetPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.SetStartPosition(_targetPosition);
            player.ReturnToLastPosition();
        }
    }
}
