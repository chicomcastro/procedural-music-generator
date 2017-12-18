using System.Collections.Generic;
using UnityEngine;

public class VisualConstructor : MonoBehaviour {

	public GameObject spotObject;
	public Canvas canvas;
	public GameObject cameraSensor;
	public GameObject branchCanvas;
	public AudioManager audioManager;

	private List<Branch> branch = new List<Branch>();

	private int amplitude;
	private int musicSize;
	private float imageLenght;
	private float imageHeight;

	private Branch currentBranch;

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

		SpawnSensor();
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
				GameObject newGO = Instantiate(spotObject, new Vector3(imageLenght * i - 8f, imageHeight * j - 5f), transform.rotation);
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
			print(noteToActivate.note.name);
		}
	}

	public void TurnOn (int i)
	{

	}

	private void SetSensorPosition(int i)
	{

	}

	public Vector3 GetSensorPosition()
	{
		return cameraSensor.transform.position;
	}

	private void SpawnSensor()
	{
		GameObject aux = Instantiate(cameraSensor, transform.position, transform.rotation);
		cameraSensor = aux;
	}

	public void TurnOff (int i)
	{

	}

	public static float Map (float x, float xMin, float xMax, float yMin, float yMax)
	{
		return (x - xMin) / (xMax - xMin) * (yMax - yMin) + yMin;
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

	public VisualNote(GameObject go, Note note_)
	{
		obj = go;
		isActive = false;
		note = note_;
	}
}