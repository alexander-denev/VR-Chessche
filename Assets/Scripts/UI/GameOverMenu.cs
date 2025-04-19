using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
	public static GameOverMenu Instance { set; get; }

    [SerializeField] private GameObject gameOverMenuUI;

	private void Start()
    {
        Instance = this;
    }

    public void OpenMenu()
    {
        gameOverMenuUI.SetActive(true);
    	Time.timeScale = 0f;
    }
}
