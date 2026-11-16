
using UnityEngine;

public class MovimientoNota : MonoBehaviour
{
    public float velocidad = 4f;
    public float limiteInferior = -10f;

    private void Update()
    {
        transform.Translate(
            Vector3.down * velocidad * Time.deltaTime
        );

        if (transform.position.y <= limiteInferior)
        {
            if (AdministradorPuntuacion.Instancia != null)
            {
                AdministradorPuntuacion.Instancia
                    .RegistrarError();
            }

            Destroy(gameObject);
        }
    }
}
