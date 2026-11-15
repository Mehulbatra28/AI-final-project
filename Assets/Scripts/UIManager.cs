using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnPlayButtonSelected()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1.0f;
    }
    public void OnPlayLevel1ButtonSelected()
    {
        SceneManager.LoadScene("Game 1");
        Time.timeScale = 1.0f;
    }
    public void OnPlayLevel2ButtonSelected()
    {
        SceneManager.LoadScene("Game 2");
        Time.timeScale = 1.0f;
    }

    public void OnExitSelected()
    {
        Application.Quit();
    }
  
}
