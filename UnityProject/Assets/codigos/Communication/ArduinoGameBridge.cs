using UnityEngine;
using System;
using System.Collections.Generic;

public class ArduinoGameBridge : MonoBehaviour
{
    [Header("Comunicacion")]
    [SerializeField] private SerialManager serialManager;

    private readonly string[] fretNames =
    {
        "GREEN",
        "RED",
        "YELLOW",
        "BLUE",
        "ORANGE"
    };

    private readonly bool[] fretStates = new bool[5];

    private bool previousStrumState = false;

    // Se ejecuta cuando se presiona la palanca.
    // Entrega los colores que están presionados.
    public event Action<string[]> OnPhysicalStrum;

    private void Start()
    {
        if (serialManager == null)
        {
            serialManager = FindFirstObjectByType<SerialManager>();
        }

        if (serialManager != null)
        {
            serialManager.OnMessageReceived += ProcessArduinoMessage;
        }
        else
        {
            Debug.LogWarning("No se encontró SerialManager en la escena.");
        }
    }

    private void ProcessArduinoMessage(string message)
    {
        string[] data = message.Split(',');

        if (data.Length == 0)
            return;

        switch (data[0])
        {
            case "READY":
                Debug.Log("Arduino está listo.");
                break;

            case "FRET":
                ProcessFret(data);
                break;

            case "STRUM":
                ProcessStrum(data);
                break;

            case "STAR":
                ProcessStarPower(data);
                break;

            case "MPU":
                ProcessMPU(data);
                break;

            case "STREAK":
                Debug.Log("Racha informada por Arduino: " +
                          (data.Length > 1 ? data[1] : "0"));
                break;
        }
    }

    private void ProcessFret(string[] data)
    {
        if (data.Length < 3)
            return;

        int index = Array.IndexOf(fretNames, data[1]);

        if (index < 0)
            return;

        fretStates[index] = data[2] == "1";
    }

    private void ProcessStrum(string[] data)
    {
        if (data.Length < 2)
            return;

        bool currentState = data[1] == "1";

        // Solo reaccionamos al momento de presionar.
        if (currentState && !previousStrumState)
        {
            List<string> pressedFrets = new List<string>();

            for (int i = 0; i < fretStates.Length; i++)
            {
                if (fretStates[i])
                {
                    pressedFrets.Add(fretNames[i]);
                }
            }

            Debug.Log("Palanca activada. Botones presionados: " +
                      string.Join(", ", pressedFrets));

            OnPhysicalStrum?.Invoke(pressedFrets.ToArray());
        }

        previousStrumState = currentState;
    }

    private void ProcessStarPower(string[] data)
    {
        if (data.Length < 2)
            return;

        bool active = data[1] == "1";

        if (AdministradorPuntuacion.Instancia != null)
        {
            AdministradorPuntuacion.Instancia
                .EstablecerMultiplicadorGiroscopio(active);
        }
    }

    private void ProcessMPU(string[] data)
    {
        if (data.Length < 2)
            return;

        bool detected = data[1] == "1";

        Debug.Log(detected
            ? "Giroscopio detectado."
            : "Giroscopio no detectado.");
    }

    private void OnDestroy()
    {
        if (serialManager != null)
        {
            serialManager.OnMessageReceived -= ProcessArduinoMessage;
        }
    }
}