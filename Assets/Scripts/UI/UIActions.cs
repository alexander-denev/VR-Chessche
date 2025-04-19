using UnityEngine;
using UnityEngine.SceneManagement;

public class UIActions : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit Game!!");
        Application.Quit();
    }

    public void RestartGame()
    {
        //BoardManager.Instance.EndGame();
        SceneManager.LoadScene(0);
    }
}
