using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameUIManager : MonoBehaviour
{
    public TMP_Text WinText;
    public GameObject Panel;
    public static GameUIManager instance;
    public TMP_Text RocksText;
    public void Awake()
    {
        instance = this;
    }
    public void RocksCount(int count)
    {
        RocksText.SetText("Rocks:"+count);
    }
    public void ORestartButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
        public void ShowWinScreen()
        {

        
               Panel.SetActive(true);
            WinText.SetText("YOU WIN");
        Debug.Log("you win");
        Time.timeScale = 0f;

    }

    public void ShowLosescreen() 
        {
            Panel.SetActive(true);
            WinText.SetText("YOU LOSE");
            Debug.Log("you lose");
             Time.timeScale = 0f;
        }
    
}
