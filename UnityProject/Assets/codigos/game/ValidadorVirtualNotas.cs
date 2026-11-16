using UnityEngine;
using UnityEngine.Events;

public class ValidadorVirtualNotas : MonoBehaviour
{
    [Header("Configuracion del control virtual")]
    [SerializeField] private KeyCode teclaValidacion = KeyCode.Semicolon;

    [Header("Evento de validacion")]
    [SerializeField] private UnityEvent onValidarNota;

    private void Update()
    {
        if (Input.GetKeyDown(teclaValidacion))
        {
            ValidarNotaVirtual();
        }
    }

    public void ValidarNotaVirtual()
    {
        Debug.Log("Validacion virtual solicitada mediante la tecla Ñ.");

        onValidarNota?.Invoke();
    }
}