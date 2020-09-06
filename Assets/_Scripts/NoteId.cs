using UnityEngine;

public class NoteId : MonoBehaviour
{
	public VisualKey info;
	public Color pressedColor;
	public Color idleColor;
	public Color playingColor;

	private Color currentColor;
	private ConfigManager configManager;

	private void Awake()
	{
		GetComponent<SpriteRenderer>().color = idleColor;
	}

	private void Start()
	{
		GetComponent<AudioSource>().clip = info.note.clip;
		currentColor = GetComponent<SpriteRenderer>().color;

		configManager = FindObjectOfType<ConfigManager>();
	}

	void OnMouseDown()
	{
		Activate();
	}

	public void SetButtonActivation (bool state)
	{
		info.isActive = state;

		SetColorOnActivation();
	}

	private void Activate()
	{
		info.isActive = !info.isActive;

		SetColorOnActivation();
	}

	public void Play()
	{
		TurnOn();

		if (configManager == null)
			Debug.LogWarning("Erro no configManager da célula " + gameObject.name);
		//else
		//	GetComponent<AudioSource>().volume = configManager.GetCurrentVolume();

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

	private void SetColorOnActivation()
	{
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
}
