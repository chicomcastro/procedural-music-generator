using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMM.Demo
{
    public class VisualConstructor : MonoBehaviour
    {
        public GameObject spotObject;
        public Canvas canvas;
        public GameObject cameraSensorPrefab;
        public GameObject branchCanvas;
        public AudioManager audioManager;

        public Vector2 spacement;
        public Transform gridParent;

        private List<Branch> branch = new List<Branch>();

        private int amplitude;
        private int musicSize;
        private float imageLenght;
        private float imageHeight;

        private Branch currentBranch;

        private GameObject currentCameraSensor;

        private float lastCoordinate;

        void Start()
        {
            amplitude = audioManager.GetNotesRange();
            musicSize = audioManager.GetMusicArmature();

            imageLenght = spotObject.transform.localScale.x;
            imageHeight = spotObject.transform.localScale.y;

            InitializeKeyboard();
        }

        private void Update()
        {
            SeeIfItveFinished();
        }

        private void InitializeKeyboard()
        {
            lastCoordinate = 0;

            CreateNewBranch();

            for (int i = 0; i < audioManager.melodyParameters.size; i++)
            {
                SpawnNewTime();
            }
        }

        public void CreateNewBranch()
        {
            branch.Add(new Branch());
            currentBranch = branch[branch.Count - 1];
            currentBranch.tempo = new List<Tempo>();
        }

        public void SpawnNewTime()
        {
            int actualTempoQuant = GetTempoQuant();

            float lastPosX = 0;
            if (actualTempoQuant >= 1)
                lastPosX = currentBranch.tempo[actualTempoQuant - 1].note[0].obj.transform.position.x;

            for (int i = 0; i < musicSize; i++)
            {
                currentBranch.tempo.Add(new Tempo());

                Tempo currentTempo = currentBranch.tempo[i + actualTempoQuant];
                currentTempo.note = new List<VisualKey>();

                for (int j = 0; j < amplitude; j++)
                {
                    GameObject newGO = Instantiate(spotObject, new Vector3((lastPosX + spacement.x * 2) + (imageLenght + spacement.x) * i + 1f, (imageHeight + spacement.y) * j + imageHeight / 2), transform.rotation, gridParent);
                    currentTempo.note.Add(new VisualKey(newGO, audioManager.notes[j]));
                    newGO.GetComponent<NoteId>().info = currentTempo.note[currentTempo.note.Count - 1];

                    if (newGO.transform.position.x > lastCoordinate)
                    {
                        lastCoordinate = newGO.transform.position.x;
                        FindObjectOfType<CameraFollower>().boundInf.transform.position = new Vector3(
                            lastCoordinate,
                            FindObjectOfType<CameraFollower>().boundInf.transform.position.y,
                            FindObjectOfType<CameraFollower>().boundInf.transform.position.z);
                    }
                }
            }
        }

        public void ApplyMelody(int[] notesHeights)
        {
            for (int i = 0; i < currentBranch.tempo.Count; i++)
            {
                VisualKey noteToActivate = MatchNote(notesHeights, i);
                noteToActivate.obj.GetComponent<NoteId>().SetButtonActivation(true);
                if (i > 0)
                {
                    if (MatchNote(notesHeights, i).note.name == MatchNote(notesHeights, i - 1).note.name)
                    {
                        noteToActivate.obj.GetComponent<NoteId>().SetButtonActivation(false);
                    }
                }
            }
        }

        private VisualKey MatchNote(int[] notesHeights, int i)
        {
            return currentBranch.tempo[i].note[GetValueCircularly(notesHeights, i)];
        }

        private int GetValueCircularly(int[] _array, int i)
        {
            return _array[i % _array.Length];
        }

        private void SpawnSensor()
        {
            if (currentCameraSensor != null)
                Destroy(currentCameraSensor);

            GameObject aux = Instantiate(cameraSensorPrefab, transform.position, transform.rotation);
            currentCameraSensor = aux;

            Play();
        }

        public void Play()
        {
            if (currentCameraSensor == null)
            {
                SpawnSensor();
                return;
            }

            FindObjectOfType<CameraFollower>().followSensor = true;
            currentCameraSensor.GetComponent<Rigidbody2D>().velocity = new Vector2(imageLenght / (60f / audioManager.melodyParameters.bpm), 0f);
        }

        public void Pause()
        {
            if (currentCameraSensor == null)
            {
                Debug.LogWarning("We're trying to PAUSE, but there's no camera sensor instantiated");
                return;
            }

            FindObjectOfType<CameraFollower>().followSensor = false;
            currentCameraSensor.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }

        public void Stop()
        {
            if (currentCameraSensor != null)
            {
                FindObjectOfType<CameraFollower>().GoHome();
                Destroy(currentCameraSensor);
            }
            else
            {
                Debug.LogWarning("We're trying to STOP, but there's no camera sensor instantiated");
                return;
            }
        }

        private void Loop()
        {
            if (currentCameraSensor == null)
                return;

            FindObjectOfType<CameraFollower>().GoHome();
            Destroy(currentCameraSensor);
            SpawnSensor();
            currentCameraSensor.GetComponent<Rigidbody2D>().velocity = new Vector2(imageLenght / (60f / audioManager.melodyParameters.bpm), 0f);
        }

        public void Reset()
        {
            foreach (Tempo t in currentBranch.tempo)
            {
                foreach (VisualKey vs in t.note)
                {
                    if (vs.isActive)
                    {
                        vs.obj.GetComponent<NoteId>().SetButtonActivation(false);
                    }
                }
            }
        }

        public Vector3 GetSensorPosition() // Used on CameraFollower script
        {
            if (currentCameraSensor == null)
                return Vector3.zero;

            return currentCameraSensor.transform.position;
        }

        public int GetTempoQuant()
        {
            return currentBranch.tempo.Count;
        }

        private void SeeIfItveFinished()
        {
            if (currentCameraSensor == null)
                return;

            if (GetSensorPosition().x > lastCoordinate)
            {
                if (audioManager.publicReferences.loopToggle.isOn)
                {
                    Loop();
                    return;
                }
                Pause();
            }
        }

        public void SpawnNewKeyboard()
        {
            for (int i = 0; i < gridParent.childCount; i++)
            {
                Destroy(gridParent.GetChild(i).gameObject);
            }

            InitializeKeyboard();
        }
    }

    public class Branch
    {
        public List<Tempo> tempo;
    }

    public class Tempo
    {
        public List<VisualKey> note;
    }

    public class VisualKey
    {
        public GameObject obj;
        public bool isActive;
        public Note note;

        public VisualKey(GameObject go, Note note_)
        {
            obj = go;
            isActive = false;
            note = note_;
        }
    }
}