using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float _gameSpeed = 1f;
    public void SetGameSpeed(float newSpeed)
    {
        _gameSpeed = newSpeed;
        Time.timeScale = _gameSpeed;
    }
}
