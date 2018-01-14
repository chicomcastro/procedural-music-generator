using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour {

	// Our list of sounds
	public AudioClip[] audioSamples;
	public List<Note> notes = new List<Note>();
	private List<Note> scale = new List<Note>();
	private float time;

	private VisualConstructor vs;

	const int NOTE = 0;
	const int PAUSE = 1;

	public int[,] noiseMap;
	
	public float bpm = 120f;
	public int dimensions = 4;
	public int size = 20;
	public int seed = 10;
	public int octaves = 1;
	[Range(0, 1)]
	public float persistance = 1f;
	public float lacunarity = 1f;

	public int signature = 4;

	public Text bpmInput, dimensionInput, seedInput, octaveInput, lacunarityInput, sizeInput;
	public Slider persistanceInput;
	public Toggle loopToggle;

	// Our reference to an audio manager
	public static AudioManager instance;

	public string[] scaleNotes = new string[] { "C", "D", "E", "G", "A" };

	private void Awake() {

		// Is instance is empty, fill it up with this gameobject and others like him will be destroyed
		if (instance == null)
			instance = this;
		else {
			Destroy(this.gameObject);
			return;
		}

		// And we dont wanna destroy this game object on load scene
		DontDestroyOnLoad(gameObject);

		notes = Mapper.MapAudioClips(audioSamples);

		scale = Mapper.GetScale(notes, scaleNotes);

		vs = FindObjectOfType<VisualConstructor>();
	}
	
	public void GenerateMelody () // When Play button is hitted, this method is called
	{
		noiseMap = PerlinNoise.GenerateHeights(dimensions, dimensions, seed, scale.Count-1, octaves, persistance, lacunarity);
		
		int[] aux = new int[noiseMap.GetLength(0)];

		for (int i = 0; i < aux.Length; i++)
		{
			aux[i] = noiseMap[i, NOTE];
		}

		// Truncate music tempo
		// Divide music (structuration)
		// Get harmony
		// StartCoroutine (GenerateHarmony())

		int[] melodyHeights = ConvertNoiseMapIntoScaleInfo(aux);
		
		vs.ApplyMelody(melodyHeights);
	}

	private void Stop()
	{
		FindObjectOfType<CameraFollower>().InitializeCamera();
	}

	private int[] ConvertNoiseMapIntoScaleInfo(int[] aux)
	{
		Note[] melody = new Note[aux.Length];

		for (int i = 0; i < aux.Length; i++)
		{
			melody[i] = scale[aux[i]];
		}

		int[] result = new int[aux.Length];
		int j = 0;

		foreach (Note n in melody)
		{
			result[j] = notes.FindIndex(note => note == n);
			j++;
		}

		return result;
	}

	void OnValidate()
	{
		if (bpm < 60)
		{
			bpm = 60;
		}
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 0)
		{
			octaves = 0;
		}
		if (signature <= 0)
		{
			signature = 1;
		}
	}
	
	#region Encapsuling stuff
	public void SetBPM()
	{
		int result;
		if (int.TryParse(bpmInput.text, out result))
		{
			bpm = result;
		}
	}

	public void SetSeed()
	{
		int result;
		if (int.TryParse(seedInput.text, out result))
		{
			seed = result;
		}
	}

	public void SetOctave()
	{
		int result;
		if (int.TryParse(octaveInput.text, out result))
		{
			octaves = result;
		}
	}

	public void SetLacunarity()
	{
		int result;
		if (int.TryParse(lacunarityInput.text, out result))
		{
			lacunarity = result;
		}
	}

	public void SetPersistance()
	{
		persistance = persistanceInput.value;
	}

	public void SetDimensions()
	{
		int result;
		if (int.TryParse(dimensionInput.text, out result))
		{
			dimensions = result;
		}
	}

	public void SetSize()
	{
		int result;
		if (int.TryParse(sizeInput.text, out result))
		{
			size = result;
		}
	}

	public int GetNotesRange()
	{
		return audioSamples.Length;
	}

	public int GetMusicLength()
	{
		return dimensions * dimensions;
	}

	public int GetMusicSize()
	{
		return dimensions;
	}

	public int GetMusicArmature()
	{
		return signature;
	}
	#endregion
}
/* Prioridades:
 * - Colocar câmera para seguir OK
 * - Melhorar UI OK
 * - Colocar animação OK
 * - Gerar Perlin mais sofisticado OK
 * - Os tempos não estão legais OK
 * */

/* - Ver se restringir as oitavas da melodia fica melhor
 *		- EDIT: estou começando a achar que a melodia já está boa
 *		- Acho que só acrescentar a harmonia e a percussão já vai ficar top <<< THIS
 *		
 *	* COISAS PRA UMA VERSÃO PLUS
 * - Passar um filtro no noiseMap
 * - Definir trechos da música (intro, verso, refrão, etc)
 * - Fazer teste de probabilidade no noiseMap
 * - Melhorar as pausas (tentar)
 * - Definir harmonia (progressão de acordes com base no noiseMap)
 * - Definir trigger de eventos de sincopatia
 * - Dar feedback visual OK
 * - Permitir output MIDI
 * - Controle de bateria/modalidade ao vivo
 * */

