
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NotaProgramada
{
    public float tiempo;

    [Range(0, 4)]
    public int carril;
}

[System.Serializable]
public class CancionProgramada
{
    public string nombre;

    public AudioClip audio;

    public List<NotaProgramada> notas =
        new List<NotaProgramada>();
}

public class SincronizadorMusical : MonoBehaviour
{
    [Header("Configuracion")]
    public AudioSource reproductor;
    public GameObject prefabNota;

    [Header("Posiciones")]
    public float posicionInicialY = 5.5f;
    public float posicionAciertoY = -3.5f;

    [Header("Lista de canciones")]
    public List<CancionProgramada> canciones =
        new List<CancionProgramada>();

    private int indiceCancionActual = 0;
    private int indiceNota = 0;

    private double tiempoInicioCancion;
    private float tiempoRecorrido;

    private bool finalizado = false;

    private float[] posicionesX =
    {
        -2f, -1f, 0f, 1f, 2f
    };

    private Color[] colores =
    {
        Color.green,
        Color.red,
        Color.yellow,
        Color.blue,
        new Color(1f, 0.45f, 0f)
    };

    private void Start()
    {
        if (reproductor == null || prefabNota == null)
        {
            Debug.LogError(
                "Falta asignar el AudioSource o el Prefab Nota."
            );

            enabled = false;
            return;
        }

        MovimientoNota movimiento =
            prefabNota.GetComponent<MovimientoNota>();

        if (movimiento == null || movimiento.velocidad <= 0)
        {
            Debug.LogError(
                "El Prefab Nota necesita MovimientoNota."
            );

            enabled = false;
            return;
        }

        if (canciones.Count == 0)
        {
            Debug.LogError("No hay canciones configuradas.");
            enabled = false;
            return;
        }

        tiempoRecorrido =
            (posicionInicialY - posicionAciertoY) /
            movimiento.velocidad;

        reproductor.playOnAwake = false;
        reproductor.spatialBlend = 0f;

        IniciarCancion(0, AudioSettings.dspTime + 0.2);
    }

    private void Update()
    {
        if (finalizado)
            return;

        CancionProgramada cancion =
            canciones[indiceCancionActual];

        float tiempoCancion =
            (float)(AudioSettings.dspTime - tiempoInicioCancion);

        while (indiceNota < cancion.notas.Count &&
               tiempoCancion >=
               cancion.notas[indiceNota].tiempo - tiempoRecorrido)
        {
            CrearNota(cancion.notas[indiceNota]);
            indiceNota++;
        }

        if (tiempoCancion >= cancion.audio.length)
        {
            IniciarCancion(
                indiceCancionActual + 1,
                AudioSettings.dspTime + 0.1
            );
        }
    }

    private void IniciarCancion(
        int nuevoIndice,
        double tiempoInicio
    )
    {
        if (nuevoIndice >= canciones.Count)
        {
            FinalizarJuego();
            return;
        }

        CancionProgramada cancion =
            canciones[nuevoIndice];

        if (cancion.audio == null)
        {
            Debug.LogError(
                "La canción " + cancion.nombre +
                " no tiene un archivo de audio."
            );

            FinalizarJuego();
            return;
        }

        for (int i = 0; i < cancion.notas.Count - 1; i++)
        {
            if (cancion.notas[i].tiempo >
                cancion.notas[i + 1].tiempo)
            {
                Debug.LogError(
                    "Las notas de " + cancion.nombre +
                    " no están ordenadas por tiempo."
                );

                FinalizarJuego();
                return;
            }
        }

        MovimientoNota[] notasAnteriores =
            FindObjectsByType<MovimientoNota>(
                FindObjectsSortMode.None
            );

        foreach (MovimientoNota nota in notasAnteriores)
        {
            Destroy(nota.gameObject);
        }

        indiceCancionActual = nuevoIndice;
        indiceNota = 0;

        reproductor.Stop();
        reproductor.clip = cancion.audio;

        tiempoInicioCancion = tiempoInicio;

        reproductor.PlayScheduled(tiempoInicioCancion);

        Debug.Log(
            "Reproduciendo canción " +
            (indiceCancionActual + 1) +
            ": " + cancion.nombre
        );
    }

    private void CrearNota(NotaProgramada nota)
    {
        if (nota.carril < 0 || nota.carril >= 5)
            return;

        Vector3 posicion = new Vector3(
            posicionesX[nota.carril],
            posicionInicialY,
            -0.2f
        );

        GameObject nuevaNota = Instantiate(
            prefabNota,
            posicion,
            Quaternion.identity
        );

        SpriteRenderer imagen =
            nuevaNota.GetComponent<SpriteRenderer>();

        if (imagen != null)
        {
            imagen.color = colores[nota.carril];
        }
    }

    private void FinalizarJuego()
    {
        finalizado = true;
        reproductor.Stop();

        Debug.Log("¡FELICIDADES! Has completado las 5 canciones.");
    }
}
