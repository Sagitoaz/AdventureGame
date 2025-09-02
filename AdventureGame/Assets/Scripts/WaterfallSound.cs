using UnityEngine;

public class WaterfallSound : MonoBehaviour
{
    [SerializeField] private AudioSource _waterfallAudioSource;
    [SerializeField] private Transform _player;
    void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.position);
        _waterfallAudioSource.volume = Mathf.Clamp01(1 / distance * 10);
    }
}
