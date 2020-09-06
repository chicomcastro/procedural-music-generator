using UnityEngine;

[System.Serializable]
public class PerlinParameters
{
    public int dimensions = 4;
    public int seed = 10;
    public int octaves = 1;
    [Range(0, 1)]
    public float persistance = 1f;
    public float lacunarity = 1f;

    [HideInInspector]
    public int range;
}