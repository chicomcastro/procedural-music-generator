using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace PMM.Demo
{
    public class AudioManager : MonoBehaviour
    {
        #region Not important for live performance :)
        [Header("User interface")]
        public UserInterface publicReferences = new UserInterface();

        [Space]
        [Header("Our list of sounds")]
        public AudioClip[] audioSamples;
        public List<Note> notes = new List<Note>();
        public List<Note> scale = new List<Note>();
        private float time;

        private VisualConstructor visualConstructor;
        #endregion

        [Space]
        [Header("Melody parameters")]
        public MusicParameters melodyParameters;
        public PerlinParameters perlinParameters;

        // Our reference to an audio manager
        public static AudioManager instance;

        public string[] scaleNotes;
        public int[] scaleIntervals = new int[5] { 1, 2, 3, 5, 6 };

        public List<int[]> melodies = new List<int[]>();

        int currentTempo = 1;

        private void Awake()
        {
            // Is instance is empty, fill it up with this gameobject and others like him will be destroyed
            if (instance == null)
                instance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }

            // And we dont wanna destroy this game object on load scene
            DontDestroyOnLoad(gameObject);

            // Map all our samples to notes
            notes = Mapper.MapAudioClips(audioSamples);
            scaleNotes = Mapper.GetNotesNamesFromIntervals("C", scaleIntervals);

            // Filter all our registered notes (from samples) to only notes in our scale
            scale = Mapper.GetScale(notes, scaleNotes);
            perlinParameters.range = scale.Count - 1;  // unique dinamic value (depends on samples and scale)

            visualConstructor = FindObjectOfType<VisualConstructor>();
        }

        private void Start()
        {
            SetupParameters();
        }

        public void GenerateMelody()
        {
            int[] melody = Mapper.GetNotesFromMelody(
                MelodyProvider.GenerateMelody(perlinParameters),
                notes,
                scale
            );

            // Visual feedback
            if (visualConstructor != null)
            {
                visualConstructor.ApplyMelody(melody);
                return;
            }

            melodies.Add(melody);
        }

        private void PlayMelody(int index = 0)
        {
            if (index < 0 || index > melodies.Count - 1)
            {
                index = 0;
            }

            int[] melody = melodies[index];

            // Sound feedback
            StartCoroutine(PlayMusic(melody));
        }

        IEnumerator PlayMusic(int[] melody)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            yield return new WaitUntil(() => currentTempo == 1);

            while (true)
            {
                currentTempo = 1;
                for (int i = 0; i < melodyParameters.size * melodyParameters.signature; i++)
                {
                    audioSource.clip = notes[melody[i]].clip;

                    // Fazer lógica de não tocar caso seja repetido

                    audioSource.Play();
                    yield return new WaitForSeconds(60f / melodyParameters.bpm);

                    currentTempo++;
                }
            }
        }

        void OnValidate()
        {
            // Music
            if (melodyParameters.bpm < 60)
            {
                melodyParameters.bpm = 60;
            }
            if (melodyParameters.signature < 2)
            {
                melodyParameters.signature = 2;
            }
            // Perlin
            if (perlinParameters.lacunarity < 1)
            {
                perlinParameters.lacunarity = 1;
            }
            if (perlinParameters.octaves < 1)
            {
                perlinParameters.octaves = 1;
            }
        }

        #region Encapsuling stuff
        private void SetupParameters()
        {
            publicReferences.bpmInput.transform.parent.gameObject.GetComponent<InputField>().text = melodyParameters.bpm.ToString();
            publicReferences.dimensionInput.transform.parent.gameObject.GetComponent<InputField>().text = perlinParameters.dimensions.ToString();
            publicReferences.lacunarityInput.transform.parent.gameObject.GetComponent<InputField>().text = perlinParameters.lacunarity.ToString();
            publicReferences.octaveInput.transform.parent.gameObject.GetComponent<InputField>().text = perlinParameters.octaves.ToString();
            publicReferences.persistanceInput.value = perlinParameters.persistance;
            publicReferences.seedInput.transform.parent.gameObject.GetComponent<InputField>().text = perlinParameters.seed.ToString();
            publicReferences.sizeInput.transform.parent.gameObject.GetComponent<InputField>().text = melodyParameters.size.ToString();
        }
        public void SetBPM()
        {
            int result;
            if (int.TryParse(publicReferences.bpmInput.text, out result))
            {
                melodyParameters.bpm = result;
            }
        }

        public void SetSize()
        {
            int result;
            if (int.TryParse(publicReferences.sizeInput.text, out result))
            {
                melodyParameters.size = result;
            }
        }

        public void SetSeed()
        {
            int result;
            if (int.TryParse(publicReferences.seedInput.text, out result))
            {
                perlinParameters.seed = result;
            }
        }

        public void SetOctave()
        {
            int result;
            if (int.TryParse(publicReferences.octaveInput.text, out result))
            {
                perlinParameters.octaves = result;
            }
        }

        public void SetLacunarity()
        {
            int result;
            if (int.TryParse(publicReferences.lacunarityInput.text, out result))
            {
                perlinParameters.lacunarity = result;
            }
        }

        public void SetPersistance()
        {
            perlinParameters.persistance = publicReferences.persistanceInput.value;
        }

        public void SetDimensions()
        {
            int result;
            if (int.TryParse(publicReferences.dimensionInput.text, out result))
            {
                perlinParameters.dimensions = result;
            }
        }

        public int GetNotesRange()
        {
            return audioSamples.Length;
        }

        public int GetMusicLength()
        {
            return perlinParameters.dimensions * perlinParameters.dimensions;
        }

        public int GetMusicSize()
        {
            return perlinParameters.dimensions;
        }

        public int GetMusicArmature()
        {
            return melodyParameters.signature;
        }
        #endregion

    }

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

