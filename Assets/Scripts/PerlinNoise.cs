using UnityEngine;
using System.IO;

public static class PerlinNoise {

	const int NOTE = 0;
	const int PAUSE = 1;

	public static int[,] GenerateHeights(int width, int length, float scale, int range, int octaves, float persistance, float lacunarity)
	{
		return GenerateHeights(width, length, scale, range, octaves, persistance, lacunarity, new Vector2(0f, 0f));
	}

	public static int[,] GenerateHeights(int width, int length, float scale, int range, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		System.Random prng = new System.Random(Mathf.RoundToInt(scale));
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}

		if (scale % (range) != 0)
			scale = scale % (range);  // Pentatonic has 5 notes
		if (scale == 1)
			scale = range;

		//if (scale % width == 0)
		//{
		//	width++;
		//}

		//if (scale % length == 0)
		//{
		//	length++;
		//}

		// Create a new one
		float[,] heights = new float[width * length, 2];
		int[,] result = new int[width * length, 2];

		// For analysis purpose
		float[,] heights2 = new float[width * length, 2];

		// For normalization purposes
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		// And go pixel by pixel setting them up
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < length; y++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{
					float sampleX = x / scale * frequency + octaveOffsets[i].x;
					float sampleY = y / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);// * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight)
				{
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}
				
				heights[length * x + y, NOTE] = noiseHeight;
				heights[length * x + y, PAUSE] = noiseHeight; // TIME SIGNATURE = 4/4
				
				heights2[length * x + y, NOTE] = noiseHeight;
			}
		}

		for (int y = 0; y < length; y++)
		{
			for (int x = 0; x < width; x++)
			{
				heights[length * x + y, NOTE] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights[length * x + y, NOTE]);
				result[length * x + y, NOTE] = Mathf.RoundToInt(heights[length * x + y, NOTE] * range);
				heights[length * x + y, PAUSE] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights[length * x + y, PAUSE]);
				result[length * x + y, PAUSE] = Mathf.RoundToInt(heights[length * x + y, PAUSE] * 4);
				heights2[length * x + y, NOTE] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights2[length * x + y, NOTE]);
			}
		}

		// if (Application.isEditor)
		// {
		// 	// Create a txt with noise data
		// 	TxtStuff(heights, "rounded");
		// 	TxtStuff(heights2, "bruto");
		// }

		return result;
	}

	private static void TxtStuff(int[,] heights, string name)
	{
		if (Application.isEditor)
		{
			string path = @"c:\temp\MyTest_" + name + ".txt";

			if (!File.Exists(path))
			{
				// Create a file to write to.
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.Write(name + "= [");
					for (int y = 0; y < heights.GetLength(0); y++)
					{
						// We wanna fill them with something from perlin noise, then let call this function
						sw.Write(heights[y, NOTE].ToString() + " ");
					}
					sw.Write("]");
				}
			}
		}
	}

	private static void TxtStuff(float[,] heights, string name)
	{
		if (Application.isEditor)
		{
			string path = @"c:\temp\MyTest_" + name + ".txt";

			if (!File.Exists(path))
			{
				// Create a file to write to.
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.Write(name + "= [");
					for (int y = 0; y < heights.GetLength(0); y++)
					{
						// We wanna fill them with something from perlin noise, then let call this function
						sw.Write(heights[y, NOTE].ToString() + " ");
					}
					sw.Write("]");
				}
			}
		}
	}
}
