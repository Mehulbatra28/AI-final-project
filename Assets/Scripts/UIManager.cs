using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
   public void OnPlayButtonSelected()
    {
        SceneManager.LoadScene("Game");
            Time.timeScale=1.0f;
    }
    public void OnExitSelected()
    {
        Application.Quit();
    }
  
}
