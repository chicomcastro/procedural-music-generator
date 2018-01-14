using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConfigManager : MonoBehaviour {

	private Slider volume;
	private GameObject keyboard;
	//public Toggle cameraFollow;

	public GameObject[] objectsToDesactivate;

	private void Start()
	{
		foreach (GameObject go in objectsToDesactivate)
		{
			if (go.GetComponent<Slider>() != null)
				volume = go.GetComponent<Slider>();
			if (go.GetComponentInChildren<Toggle>() != null)
				keyboard = go;

			go.SetActive(false);
		}
	}

	public void ToggleCameraFollower ()
	{
		FindObjectOfType<CameraFollower>().followSensor = !FindObjectOfType<CameraFollower>().followSensor;
	
		// Trocar o ícone do botão
	}

	public float GetCurrentVolume ()
	{
		return volume.normalizedValue;
	}

	public void AttEscala ()
	{
		Toggle[] group = keyboard.GetComponentsInChildren<Toggle>();
		List<string> notes = new List<string>();

		foreach (Toggle t in group)
		{
			if (t.isOn)
				notes.Add(t.gameObject.name);
		}

		FindObjectOfType<AudioManager>().scaleNotes = notes.ToArray();
	}
}
