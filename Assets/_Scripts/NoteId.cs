using UnityEngine;

public class NoteId : MonoBehaviour
{
	public VisualNote info;
	public Color pressedColor;
	public Color idleColor;
	public Color playingColor;

	private Color currentColor;

	private void Awake()
	{
		GetComponent<SpriteRenderer>().color = idleColor;
	}

	private void Start()
	{
		GetComponent<AudioSource>().clip = info.note.clip;
		currentColor = GetComponent<SpriteRenderer>().color;
	}

	void OnMouseDown()
	{
		Activate();
	}

	public void Activate ()
	{
		info.isActive = !info.isActive;

		if (info.isActive)
		{
			currentColor = pressedColor;

			if (GetComponent<SpriteRenderer>().color == playingColor)
				return;

			GetComponent<SpriteRenderer>().color = pressedColor;
		}
		else
		{
			currentColor = idleColor;

			if (GetComponent<SpriteRenderer>().color == playingColor)
				return;

			GetComponent<SpriteRenderer>().color = idleColor;
		}
	}

	public void Play()
	{
		TurnOn();
		GetComponent<AudioSource>().Play();
	}

	private void TurnOn ()
	{
		GetComponent<SpriteRenderer>().color = playingColor;
	}

	public void TurnOff()
	{
		GetComponent<SpriteRenderer>().color = currentColor;
	}
}
