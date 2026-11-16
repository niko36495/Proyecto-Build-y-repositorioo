
using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;
using TMPro;

public class SerialManager : MonoBehaviour
{
    [Header("Configuracion serial")]
    [SerializeField] private int baudRate = 115200;

    [Header("Interfaz")]
    [SerializeField] private TMP_Dropdown portDropdown;
    [SerializeField] private TMP_Text connectionStatus;

    private SerialPort serialPort;
    private Thread readThread;
    private volatile bool isReading;

    private readonly ConcurrentQueue<string> receivedMessages =
        new ConcurrentQueue<string>();

    private readonly object writeLock = new object();

    public event Action<string> OnMessageReceived;

    private void Start()
    {
        RefreshPorts();
        UpdateStatus("Desconectado");
    }

    private void Update()
    {
        while (receivedMessages.TryDequeue(out string message))
        {
            OnMessageReceived?.Invoke(message);
            Debug.Log("Arduino -> Unity: " + message);
        }
    }

    public void RefreshPorts()
    {
        if (portDropdown == null)
        {
            Debug.LogWarning("No se ha asignado PortDropdown.");
            return;
        }

        string selectedPort = portDropdown.value >= 0 &&
                              portDropdown.options.Count > 0
            ? portDropdown.options[portDropdown.value].text
            : "";

        portDropdown.ClearOptions();

        string[] ports = SerialPort.GetPortNames();

        Array.Sort(ports);

        if (ports.Length == 0)
        {
            portDropdown.AddOptions(
                new System.Collections.Generic.List<string>
                {
                    "No hay puertos disponibles"
                });

            UpdateStatus("No se encontraron puertos COM");
            return;
        }

        portDropdown.AddOptions(
            new System.Collections.Generic.List<string>(ports)
        );

        int selectedIndex = Array.IndexOf(ports, selectedPort);

        portDropdown.value = selectedIndex >= 0 ? selectedIndex : 0;
        portDropdown.RefreshShownValue();

        UpdateStatus("Selecciona el puerto de Arduino");
    }

    public void Connect()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            UpdateStatus("Ya existe una conexion");
            return;
        }

        if (portDropdown == null || portDropdown.options.Count == 0)
        {
            UpdateStatus("No hay puertos disponibles");
            return;
        }

        string selectedPort =
            portDropdown.options[portDropdown.value].text;

        if (!selectedPort.StartsWith("COM"))
        {
            UpdateStatus("Selecciona un puerto COM valido");
            return;
        }

        try
        {
            serialPort = new SerialPort(selectedPort, baudRate);

            serialPort.ReadTimeout = 200;
            serialPort.WriteTimeout = 500;
            serialPort.NewLine = "\n";
            serialPort.DtrEnable = true;

            serialPort.Open();

            isReading = true;

            readThread = new Thread(ReadSerial);
            readThread.IsBackground = true;
            readThread.Start();

            UpdateStatus("Conectado a " + selectedPort);
            Debug.Log("Conexion establecida con " + selectedPort);
        }
        catch (Exception e)
        {
            UpdateStatus("Error de conexion");
            Debug.LogError("Error serial: " + e.Message);

            Disconnect();
        }
    }

    private void ReadSerial()
    {
        while (isReading && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string message = serialPort.ReadLine().Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    receivedMessages.Enqueue(message);
                }
            }
            catch (TimeoutException)
            {
                // No llego un mensaje durante este intervalo.
            }
            catch (Exception e)
            {
                if (isReading)
                {
                    Debug.LogWarning("Lectura serial interrumpida: " + e.Message);
                }

                break;
            }
        }
    }

    public void SendMessageToArduino(string message)
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            Debug.LogWarning("Arduino no esta conectado.");
            return;
        }

        try
        {
            lock (writeLock)
            {
                serialPort.WriteLine(message);
            }

            Debug.Log("Unity -> Arduino: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al enviar mensaje: " + e.Message);
            UpdateStatus("Error enviando datos");
        }
    }

    public bool IsConnected
    {
        get { return serialPort != null && serialPort.IsOpen; }
    }

    public void Disconnect()
    {
        isReading = false;

        try
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error al cerrar puerto: " + e.Message);
        }

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(500);
        }

        readThread = null;

        if (serialPort != null)
        {
            serialPort.Dispose();
            serialPort = null;
        }

        UpdateStatus("Desconectado");
    }

    private void UpdateStatus(string message)
    {
        if (connectionStatus != null)
        {
            connectionStatus.text = message;
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
