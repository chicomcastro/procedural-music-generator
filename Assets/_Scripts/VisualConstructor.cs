using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualConstructor : MonoBehaviour {

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

	void Start()
	{
		amplitude = audioManager.GetNotesRange();
		musicSize = audioManager.GetMusicSize();

		imageLenght = spotObject.transform.localScale.x;
		imageHeight = spotObject.transform.localScale.y;

		InitializeKeyboard();
	}

	private void InitializeKeyboard ()
	{
		CreateNewBranch();

		SpawnNewTime(currentBranch);
	}

	public void CreateNewBranch()
	{
		branch.Add(new Branch());
		currentBranch = branch[branch.Count - 1];
		currentBranch.tempo = new List<Tempo>();
	}

	public void SpawnNewTime(Branch currentBranch)
	{
		int actualTempoQuant = currentBranch.tempo.Count;

		for (int i = 0; i < musicSize; i++)
		{
			currentBranch.tempo.Add(new Tempo());

			Tempo currentTempo = currentBranch.tempo[i + actualTempoQuant];
			currentTempo.note = new List<VisualNote>();

			for (int j = 0; j < amplitude; j++)
			{
				GameObject newGO = Instantiate(spotObject, new Vector3((imageLenght + spacement.x) * i + 1f, (imageHeight + spacement.y) * j), transform.rotation, gridParent);
				currentTempo.note.Add(new VisualNote(newGO, audioManager.notes[j]));
				newGO.GetComponent<NoteId>().info = currentTempo.note[currentTempo.note.Count - 1];
			}
		}
	}

	public void ApplyMelody (int[] notesHeights)
	{
		for (int i = 0; i < currentBranch.tempo.Count; i++)
		{
			VisualNote noteToActivate = currentBranch.tempo[i].note[notesHeights[i]];
			noteToActivate.obj.GetComponent<NoteId>().Activate();
		}
	}

	public void SpawnSensor()
	{
		if (currentCameraSensor != null)
			Destroy(currentCameraSensor);

		GameObject aux = Instantiate(cameraSensorPrefab, transform.position, transform.rotation);
		currentCameraSensor = aux;

		currentCameraSensor.GetComponent<Rigidbody2D>().velocity = new Vector2(imageHeight / (60f / audioManager.bpm), 0f);
	}

	public Vector3 GetSensorPosition() // Used on CameraFollower script
	{
		return currentCameraSensor.transform.position;
	}
}

public class Branch
{
	public List<Tempo> tempo;
}

public class Tempo
{
	public List<VisualNote> note;
}

public class VisualNote
{
	public GameObject obj;
	public bool isActive;
	public Note note;
	public LineRenderer lineRenderer;

	public VisualNote(GameObject go, Note note_)
	{
		obj = go;
		isActive = false;
		note = note_;
	}
}