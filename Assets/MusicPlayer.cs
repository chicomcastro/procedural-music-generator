using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip baseNote;              // Typically a sample of note 2C
    public int baseOctave = 2;

    public Dictionary<ChordType, int[]> chordDictionary = new Dictionary<ChordType, int[]> {
        {ChordType.MAJOR, new int[3] {0, 4, 7}},
        {ChordType.MINOR, new int[3] {0, 3, 7}},
    };

    public void PlayNote(float intervalFromBaseNote, float octave)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = baseNote;
        audioSource.pitch = Mathf.Pow(2, (intervalFromBaseNote + (octave - baseOctave) * 12) / 12.0f);
        audioSource.Play();
        Destroy(audioSource, 1f);
    }

    public void PlayChord(float intervalFromBaseNote, float octave, ChordType chordType)
    {
        int[] notesToPlay = chordDictionary[chordType];

        foreach (int note in notesToPlay)
        {
            PlayNote(note + intervalFromBaseNote, octave);
        }
    }
}

public enum ChordType
{
    MAJOR = 1,
    MINOR = 2,
}
