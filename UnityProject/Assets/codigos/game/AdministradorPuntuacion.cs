using UnityEngine;
using TMPro;
using System;

public class AdministradorPuntuacion : MonoBehaviour
{
    public static AdministradorPuntuacion Instancia;

    [Header("Estadisticas")]
    public int puntuacion = 0;
    public int aciertos = 0;
    public int errores = 0;
    public int rachaActual = 0;
    public int mejorRacha = 0;

    [Header("Configuracion de racha y LEDs")]
    public int rachaParaActivarLEDs = 10;
    public bool ledsRachaActivos = false;

    [Header("Multiplicador de puntuacion")]
    public bool multiplicadorGiroscopioActivo = false;

    [Header("Textos de pantalla")]
    public TMP_Text textoPuntuacion;
    public TMP_Text textoAciertos;
    public TMP_Text textoErrores;
    public TMP_Text textoRacha;

    private SerialManager serialManager;


    // Este evento permitirá conectar los LEDs cuando tengamos
    // el protocolo serial definitivo de Arduino.
    public event Action<bool> OnEstadoLEDsRachaChanged;

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        ActualizarInterfaz();
    }

    public void RegistrarAcierto(int puntos)
    {
        int puntosFinales = multiplicadorGiroscopioActivo
            ? puntos * 2
            : puntos;

        puntuacion += puntosFinales;
        aciertos++;
        rachaActual++;

        if (rachaActual > mejorRacha)
        {
            mejorRacha = rachaActual;
        }

        EnviarComandoArduino("HIT");
        ActualizarEstadoLEDs();
        ActualizarInterfaz();

        Debug.Log("Acierto registrado. Puntos obtenidos: " + puntosFinales);
    }

    private void EnviarComandoArduino(string command)
    {
        if (serialManager == null)
        {
            serialManager = FindFirstObjectByType<SerialManager>();
        }

        if (serialManager != null && serialManager.IsConnected)
        {
            serialManager.SendMessageToArduino(command);
        }
    }

    public void RegistrarError()
    {
        errores++;
        rachaActual = 0;

        EnviarComandoArduino("MISS");
        ActualizarEstadoLEDs();
        ActualizarInterfaz();
    }

    private void ActualizarEstadoLEDs()
    {
        bool nuevoEstado = rachaActual >= rachaParaActivarLEDs;

        if (nuevoEstado == ledsRachaActivos)
        {
            return;
        }

        ledsRachaActivos = nuevoEstado;

        OnEstadoLEDsRachaChanged?.Invoke(ledsRachaActivos);

        if (ledsRachaActivos)
        {
            Debug.Log("Racha de 10 aciertos: solicitar encendido de los dos LEDs.");
        }
        else
        {
            Debug.Log("Racha perdida: solicitar apagado de los dos LEDs.");
        }
    }

    // Este método se conectará al mensaje real del giroscopio.
    public void EstablecerMultiplicadorGiroscopio(bool activo)
    {
        multiplicadorGiroscopioActivo = activo;

        Debug.Log(
            activo
            ? "Multiplicador x2 activado por el giroscopio."
            : "Multiplicador x2 desactivado."
        );
    }

    private void ActualizarInterfaz()
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = "PUNTOS: " + puntuacion;

        if (textoAciertos != null)
            textoAciertos.text = "ACIERTOS: " + aciertos;

        if (textoErrores != null)
            textoErrores.text = "ERRORES: " + errores;

        if (textoRacha != null)
            textoRacha.text = "RACHA: " + rachaActual;
    }
}