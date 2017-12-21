using UnityEngine;

public class NoteDetector : MonoBehaviour
{
	AudioManager am;

	void Start()
	{
		am = FindObjectOfType<AudioManager>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		NoteId nid = other.gameObject.GetComponent<NoteId>();
		if (nid == null)
			return;

		if (nid.info.isActive)
		{
			nid.Play();
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		NoteId nid = other.gameObject.GetComponent<NoteId>();
		if (nid == null)
			return;

		if (nid.info.isActive)
		{
			nid.TurnOff();
		}
	}
}
