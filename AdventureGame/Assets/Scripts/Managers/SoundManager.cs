using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    public AudioClip _normalAttack;
    public AudioClip _heavyAttack;
    public AudioClip _playerUltimate;
    public AudioClip _jump;
    public AudioClip _playerHit;
    public AudioClip _run;
    public AudioClip[] _attackType;
    public void PlayClip(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }
    public void PlayLoop(AudioClip clip)
    {
        if (_sfxSource.clip == clip && _sfxSource.isPlaying) return;
        _sfxSource.clip = clip;
        _sfxSource.loop = true;
        _sfxSource.Play();
    }
}