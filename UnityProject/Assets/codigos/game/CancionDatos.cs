
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "NuevaCancion",
    menuName = "Guitar Hero/Cancion"
)]
public class CancionDatos : ScriptableObject
{
    [Header("Informacion")]
    public string nombreCancion;

    [Header("Audio")]
    public AudioClip audio;

    [Header("Secuencia de notas")]
    public List<NotaProgramada> notas =
        new List<NotaProgramada>();
}
