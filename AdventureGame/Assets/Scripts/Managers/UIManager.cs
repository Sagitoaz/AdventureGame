using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image HPBar;
    [SerializeField] private Image MPBar;
    [SerializeField] private GameObject MainMenu;
    public void UpdateHealthBar(float fillAmount)
    {
        HPBar.fillAmount = fillAmount;
    }
    public void UpdateManaBar(float fillAmount)
    {
        MPBar.fillAmount = fillAmount;
    }
    public void OnClickStartButton()
    {
        GameManager.Instance.SetGameSpeed(1f);
        MainMenu.SetActive(false);
        SoundManager.Instance.gameObject.SetActive(true);
    }
    public void OnClickQuitButton()
    {
        Application.Quit();
    }
}
