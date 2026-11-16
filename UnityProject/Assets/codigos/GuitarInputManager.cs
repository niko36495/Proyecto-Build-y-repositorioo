
using UnityEngine;
using System;

public class GuitarInputManager : MonoBehaviour
{
    public SerialManager serialManager;
    public PauseManager pauseManager;

    public event Action<string> OnNotePressed;

    void OnEnable()
    {
        if (serialManager != null)
            serialManager.OnMessageReceived += ProcessMessage;
    }

    void OnDisable()
    {
        if (serialManager != null)
            serialManager.OnMessageReceived -= ProcessMessage;
    }

    void Update()
    {
        // Controles de teclado para pruebas

        if (Input.GetKeyDown(KeyCode.A))
            PressNote("GREEN");

        if (Input.GetKeyDown(KeyCode.S))
            PressNote("RED");

        if (Input.GetKeyDown(KeyCode.D))
            PressNote("YELLOW");

        if (Input.GetKeyDown(KeyCode.F))
            PressNote("BLUE");

        if (Input.GetKeyDown(KeyCode.G))
            PressNote("ORANGE");

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void ProcessMessage(string message)
    {
        message = message.Trim().ToUpper();

        if (message.StartsWith("NOTE:"))
        {
            string color = message.Substring(5);

            PressNote(color);
        }
        else if (message == "PAUSE")
        {
            TogglePause();
        }
    }

    public void PressNote(string color)
    {
        switch (color)
        {
            case "GREEN":
            case "RED":
            case "YELLOW":
            case "BLUE":
            case "ORANGE":

                Debug.Log("NOTA PRESIONADA: " + color);

                OnNotePressed?.Invoke(color);
                break;

            default:
                Debug.LogWarning("Nota desconocida: " + color);
                break;
        }
    }

    public void TogglePause()
    {
        if (pauseManager != null)
            pauseManager.TogglePause();
    }
}
