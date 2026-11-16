
using UnityEngine;

public class GeneradorNotas : MonoBehaviour
{
    public GameObject prefabNota;

    public float tiempoEntreNotas = 0.8f;
    public float posicionInicialY = 5.5f;

    private float temporizador;

    private float[] posicionesX = { -28.235f, -26.45599f, -24.713f, -22.97701f, -21.24001f };

    private void Update()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= tiempoEntreNotas)
        {
            CrearNota();
            temporizador = 0f;
        }
    }

    private void CrearNota()
    {
        int carril = Random.Range(0, posicionesX.Length);

        Vector3 posicion = new Vector3(
            posicionesX[carril],
            posicionInicialY,
            -0.2f
        );

        GameObject nuevaNota = Instantiate(
            prefabNota,
            posicion,
            Quaternion.identity
        );

        Color[] colores =
        {
            new Color(0.76f, 0.765f, 0.612f), // #C2C39C
            new Color(0.808f, 0.325f, 0.502f), // #CE5380
            new Color(0.439f, 0.533f, 0.678f), // #7088AD
            new Color(0.71f, 0.545f, 0.663f), // #B58BA9
            new Color(0.553f, 0.749f, 0.694f)  // #8DBFB1
        };

        nuevaNota.GetComponent<SpriteRenderer>().color =
            colores[carril];
    }
}
