
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Panel de pausa")]
    public GameObject pausePanel;

    public bool IsPaused { get; private set; }

    void Start()
    {
        IsPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        Time.timeScale = IsPaused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(IsPaused);

        Debug.Log(IsPaused ? "JUEGO PAUSADO" : "JUEGO REANUDADO");
    }

    public void ResumeGame()
    {
        if (IsPaused)
            TogglePause();
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Regresando al menú principal");

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
