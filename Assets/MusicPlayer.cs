using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	public AudioClip baseNote;              // Typically a sample of note 2C

    public void PlayNote(float intervalFromBaseNote)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = baseNote;
        audioSource.pitch = Mathf.Pow(2, intervalFromBaseNote / 12.0f);
        audioSource.Play();
        Destroy(audioSource, 1f);
    }
}
