using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip baseNote;              // Typically a sample of note 2C
    public int baseOctave = 2;

    public void PlayNote(int intervalFromBaseNote, int octave)
    {
        AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = baseNote;
        audioSource.pitch = Mathf.Pow(2, (intervalFromBaseNote + (octave - baseOctave) * 12) / 12.0f);
        audioSource.Play();
        Destroy(audioSource, 2f);
    }

    public void PlayChord(Chord chord)
    {
        int[] notesToPlay = ChordDictionary.chords[chord.chordType];

        foreach (int note in notesToPlay)
        {
            PlayNote(note + chord.intervalFromBaseNote, chord.octave);
        }
    }

    public void PlayChordArpeggio(Arpeggio arpeggio)
    {
        StartCoroutine(PlayArpeggio(arpeggio));
    }

    IEnumerator PlayArpeggio(Arpeggio arpeggio)
    {
        Chord chord = arpeggio.chord;
        int notesQuant = arpeggio.arpeggioPattern.Length;
        float durationInBeats = arpeggio.durationInCompasses * MusicParameters.instance.signature;   // Duration of arpeggio
        float timeBeat = 60f / MusicParameters.instance.bpm;           // Time between two beats
        float timeInterval = timeBeat * durationInBeats / notesQuant;  // Time between two notes in this arpeggio

        for (int i = 0; i < notesQuant; i++)
        {
            int note = IntervalDictionary.nameToNumber[arpeggio.arpeggioPattern[i]];
            PlayNote(note + chord.intervalFromBaseNote, chord.octave);
            yield return new WaitForSeconds(timeInterval);
        }
    }
}