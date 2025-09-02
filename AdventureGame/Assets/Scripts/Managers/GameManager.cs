using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float _gameSpeed = 1f;
    private void Start()
    {
        SetGameSpeed(0f);
        SoundManager.Instance.gameObject.SetActive(false);
    }
    public void SetGameSpeed(float newSpeed)
    {
        _gameSpeed = newSpeed;
        Time.timeScale = _gameSpeed;
    }
    public float GetGameSpeed()
    {
        return _gameSpeed;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
