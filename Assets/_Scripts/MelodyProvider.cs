using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MelodyProvider
{
    public static int[] GenerateMelodyForScale(PerlinParameters perlinParameters, List<Note> notes, List<Note> scale)
    {
        // Generate a melody for perlin parameter
        int[] melody = GenerateMelody(perlinParameters);

        // TODO:
        // Truncate music tempo
        // Divide music (structuration)
        // Get harmony
        // StartCoroutine (GenerateHarmony())

        // Apply scale filter (convert melody notes based on passed scale)
        int[] melodyNotes = ApplyScaleToMelody(melody, notes, scale);

        return melodyNotes;
    }

    public static int[] GenerateMelody(PerlinParameters perlinParameters) {
        const int NOTE = 0;
        const int PAUSE = 1;

        int[,] noiseMap = GetNoiseMap(perlinParameters);

        int[] melody = new int[noiseMap.GetLength(0)];

        for (int i = 0; i < melody.Length; i++)
        {
            melody[i] = noiseMap[i, NOTE];
        }

        return melody;
    }

    private static int[,] GetNoiseMap(PerlinParameters perlinParameters)
    {
        int dimensions = perlinParameters.dimensions;
        int seed = perlinParameters.seed;
        int range = perlinParameters.range;
        int octaves = perlinParameters.octaves;
        float persistance = perlinParameters.persistance;
        float lacunarity = perlinParameters.lacunarity;

        return PerlinNoise.GenerateHeights(dimensions, dimensions, seed, range, octaves, persistance, lacunarity);
    }

    private static int[] ApplyScaleToMelody(int[] melody, List<Note> notes, List<Note> scale)
    {
        // Get notes for our melody based on scale
        Note[] melodyNotes = new Note[melody.Length];

        for (int i = 0; i < melody.Length; i++)
        {
            melodyNotes[i] = scale[melody[i]];
        }

        // Find index of melody on original Note list to play
        int[] result = new int[melody.Length];
        int j = 0;

        foreach (Note n in melodyNotes)
        {
            result[j] = System.Array.FindIndex(notes.ToArray(), note => note == n);
            j++;
        }

        return result;
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
