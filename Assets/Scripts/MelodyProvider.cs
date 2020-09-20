using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MelodyProvider
{
    public static void PrintMelodyData(int[] melody) {
        string a = "";
        foreach (int note in melody)
        {
            a += ", " + note.ToString();
        }
        Debug.Log(a);
    }

    public static int[] GenerateMelody(PerlinParameters perlinParameters, MelodyParameters melodyParameters=null) {
		if (melodyParameters == null) {
			melodyParameters = new MelodyParameters();
		}
		Debug.Log("Generating melody with parameters: " + melodyParameters.ToString());

        const int NOTE = 0;
        const int PAUSE = 1;

        int[,] noiseMap = GetNoiseMap(perlinParameters);

        int[] melody = new int[noiseMap.GetLength(0)];

        for (int i = 0; i < melody.Length; i++)
        {
            melody[i] = noiseMap[i, NOTE] + melodyParameters.octave * 12 + melodyParameters.tone;
        }

        return melody;
    }

    private static int[,] GetNoiseMap(PerlinParameters perlinParameters)
    {
        int width = perlinParameters.width;
        int length = perlinParameters.length;
        int seed = perlinParameters.seed;
        int range = perlinParameters.range;
        int octaves = perlinParameters.octaves;
        float persistance = perlinParameters.persistance;
        float lacunarity = perlinParameters.lacunarity;

        return PerlinNoise.GenerateHeights(width, length, seed, range, octaves, persistance, lacunarity);
    }

    private static void MergeMelodies() {
        /* Merge audioclips
		// This part is not necessary right now, but it's good to keep this peace of code for future
		List<AudioClip> clipsForMerge = new List<AudioClip>();
		foreach (int i in melodyHeights)
		{
			clipsForMerge.Add(notes[i].clip);
		}

		AudioClip mergedClip = Combine(clipsForMerge.ToArray());
		AudioSource aS = gameObject.AddComponent<AudioSource>();
		aS.clip = mergedClip;
		aS.Play();
		*/
    }

    /* Combine audio clips method
	private static AudioClip Combine(params AudioClip[] clips)
	{
		if (clips == null || clips.Length == 0)
			return null;

		int length = 0;
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i] == null)
				continue;

			length += clips[i].samples * clips[i].channels;
		}

		float[] data = new float[length];
		length = 0;
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i] == null)
				continue;

			float[] buffer = new float[clips[i].samples * clips[i].channels];
			clips[i].GetData(buffer, 0);
			//System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
			buffer.CopyTo(data, length);
			length += buffer.Length;
		}

		if (length == 0)
			return null;

		AudioClip result = AudioClip.Create("Combine", length / 2, 2, 44100, false, false);
		result.SetData(data, 0);

		return result;
	}
	*/
}