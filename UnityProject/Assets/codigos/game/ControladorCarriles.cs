
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorCarriles : MonoBehaviour
{
    public Key tecla;
    public int puntosPorAcierto = 100;
    public float ventanaAcierto = 0.5f;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[tecla].wasPressedThisFrame)
        {
            IntentarAcertar();
        }
    }

    private void IntentarAcertar()
    {
        MovimientoNota[] notas =
            FindObjectsByType<MovimientoNota>(
                FindObjectsSortMode.None
            );

        MovimientoNota mejorNota = null;
        float menorDistancia = float.MaxValue;

        foreach (MovimientoNota nota in notas)
        {
            if (nota == null)
                continue;

            if (Mathf.Abs(
                nota.transform.position.x -
                transform.position.x
            ) > 0.3f)
                continue;

            float distancia = Mathf.Abs(
                nota.transform.position.y -
                transform.position.y
            );

            if (distancia <= ventanaAcierto &&
                distancia < menorDistancia)
            {
                menorDistancia = distancia;
                mejorNota = nota;
            }
        }

        
        if (mejorNota != null)
        {
            AdministradorPuntuacion.Instancia
                .RegistrarAcierto(puntosPorAcierto);

            Destroy(mejorNota.gameObject);

            Debug.Log("¡ACIERTO! +" + puntosPorAcierto);
        }
        else
        {
            Debug.Log("¡FALLO!");
        }
    }
}
