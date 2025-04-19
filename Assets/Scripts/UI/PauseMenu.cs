using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] GameObject pauseMenuUI;

    [SerializeField] private InputActionReference pauseInputAction;

    private void OnEnable()
    {
        if (pauseInputAction != null)
        {
            Debug.Log("Pause Menu Input Action Enable");
            pauseInputAction.action.Enable();
            pauseInputAction.action.performed += TogglePause;
        }
    }

    private void OnDisable()
    {
        if (pauseInputAction != null)
        {
            pauseInputAction.action.performed -= TogglePause;
            pauseInputAction.action.Disable();
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
    	Time.timeScale = 1f;
    	GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
    	Time.timeScale = 0f;
    	GameIsPaused = true;
    }

    void TogglePause(InputAction.CallbackContext context)
    {
        Debug.Log("Toggle Pause Menu!");
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
