using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
   public void OnPlayButtonSelected()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnExitSelected()
    {
        Application.Quit();
    }
}
