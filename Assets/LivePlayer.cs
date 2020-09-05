using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivePlayer : MonoBehaviour
{
	#region Live keyboard
	public float transpose = 0;  // transpose in semitones
	private List<AudioSource> audioSources = new List<AudioSource>();
	public AudioClip baseNote;

	private void Start()
	{
        if (baseNote == null) {
            Debug.LogError("There's no base note");
            return;
        }
        // Creating audiosources for each note
        for (int i = 0; i < 15; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = baseNote;
            audioSources.Add(audioSource);
        }
	}

	void Update()
	{
		List<int> notes = new List<int>();
		if (Input.GetKeyDown("a")) notes.Add(0);  // C
		if (Input.GetKeyDown("w")) notes.Add(1);  // C#
		if (Input.GetKeyDown("s")) notes.Add(2);  // D
		if (Input.GetKeyDown("e")) notes.Add(3);  // D#
		if (Input.GetKeyDown("d")) notes.Add(4);  // E
		if (Input.GetKeyDown("f")) notes.Add(5);  // F
		if (Input.GetKeyDown("t")) notes.Add(6);  // F#
		if (Input.GetKeyDown("g")) notes.Add(7);  // G
		if (Input.GetKeyDown("y")) notes.Add(8);  // G#
		if (Input.GetKeyDown("h")) notes.Add(9);  // A
		if (Input.GetKeyDown("u")) notes.Add(10); // A#
		if (Input.GetKeyDown("j")) notes.Add(11); // B
		if (Input.GetKeyDown("k")) notes.Add(12); // C
		if (Input.GetKeyDown("o")) notes.Add(13); // C#
		if (Input.GetKeyDown("l")) notes.Add(14); // D  

        // if some key pressed...
		if (notes.Count > 0)
		{
            foreach(int note in notes) {
                AudioSource audioSource = audioSources[note];
                audioSource.pitch = Mathf.Pow(2, (note + transpose) / 12.0f);
                audioSource.Play();
            }
		}
	}
	#endregion
}
