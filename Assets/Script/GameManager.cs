using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameController : Singleton<GameController>
{
    public void PlayAgain()
    {
        // Tải lại Scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
