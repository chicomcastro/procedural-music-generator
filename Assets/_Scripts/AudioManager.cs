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
	public int size = 4;
	public int seed = 10;
	public int octaves = 1;
	[Range(0, 1)]
	public float persistance = 1f;
	public float lacunarity = 1f;

	public Text BPMinput, SEEDinput;

	// Our reference to an audio manager
	public static AudioManager instance;

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

		scale = Mapper.GetScale(notes, new string[] { "C", "D", "E", "G", "A" });

		vs = FindObjectOfType<VisualConstructor>();
	}

	private void Start()
	{
		BPMinput.text = bpm.ToString();
		SEEDinput.text = seed.ToString();
	}

	public void GenerateMelody () // When Play button is hitted, this method is called
	{
		noiseMap = PerlinNoise.GenerateHeights(size, size, seed, scale.Count-1, octaves, persistance, lacunarity);
		
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

	public void Stop()
	{
		FindObjectOfType<CameraFollower>().InitializeCamera();
	}

	// public void Pause()

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
	}
	
	#region Encapsuling stuff
	public void SetBPM()
	{
		int result;
		if (int.TryParse(BPMinput.text, out result))
		{
			bpm = result;
		}
	}

	public void SetSeed()
	{
		int result;
		if (int.TryParse(SEEDinput.text, out result))
		{
			seed = result;
		}
	}

	public int GetNotesRange()
	{
		return audioSamples.Length;
	}

	public int GetMusicLength()
	{
		return size * size;
	}

	public int GetMusicSize()
	{
		return size;
	}
	#endregion
}
/* Prioridades:
 * - Colocar câmera para seguir
 * - Melhorar UI
 * - Colocar animação
 * - Gerar Perlin mais sofisticado
 * - Os tempos não estão legais
 * */

/* - Ver se restringir as oitavas da melodia fica melhor
 *		- EDIT: estou começando a achar que a melodia já está boa
 *		- Acho que só acrescentar a harmonia e a percussão já vai ficar top
 * - Passar um filtro no noiseMap
 * - Definir trechos da música (intro, verso, refrão, etc)
 * - Fazer teste de probabilidade no noiseMap
 * - Melhorar as pausas (tentar)
 * - Definir harmonia (progressão de acordes com base no noiseMap)
 * - Definir trigger de eventos de sincopatia
 * - Dar feedback visual
 * - Permitir output MIDI
 * - Controle de bateria/modalidade ao vivo
 * */

