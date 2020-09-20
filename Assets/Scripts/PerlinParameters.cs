using UnityEngine;

[System.Serializable]
public class PerlinParameters
{
    public int dimensions = 8;
    public int seed = 20;
    public int octaves = 4;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 3f;

    [HideInInspector]
    public int range = 10 * 8;
}