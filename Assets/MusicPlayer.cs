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
        int[] notesToPlay = ChordDictionary.chords[chord.chordType];

        int notesQuant = notesToPlay.Length;
        float durationInBeats = arpeggio.durationInCompasses * MusicParameters.instance.signature;   // Duration of arpeggio
        float timeBeat = 60f / MusicParameters.instance.bpm;           // Time between two beats
        float timeInterval = timeBeat * durationInBeats / notesQuant;  // Time between two notes in this arpeggio
// Debug.Log(notesQuant);
// Debug.Log(arpeggio.durationInCompasses);
// Debug.Log(MusicParameters.instance.signature);
// Debug.Log(timeBeat);
// Debug.Log(timeInterval);
        for (int i = 0; i < notesQuant; i++)
        {
            PlayNote(notesToPlay[i] + chord.intervalFromBaseNote, chord.octave);
            yield return new WaitForSeconds(timeInterval);
        }
    }
}

public static class ChordDictionary
{
    public static Dictionary<ChordType, int[]> chords = new Dictionary<ChordType, int[]> {
        {ChordType.MAJOR, new int[3] {0, 4, 7}},
        {ChordType.MINOR, new int[3] {0, 3, 7}},
        {ChordType.MAJOR7M, new int[4] {0, 4, 7, 11}},
        {ChordType.MAJOR7m, new int[4] {0, 4, 7, 10}},
        {ChordType.MINOR7M, new int[4] {0, 3, 7, 11}},
        {ChordType.MINOR7m, new int[4] {0, 3, 7, 10}},
    };
}

public class Chord
{
    public int intervalFromBaseNote;
    public int octave;
    public ChordType chordType;

    public Chord(
        int _intervalFromBaseNote,
        int _octave,
        ChordType _chordType = ChordType.MAJOR
    )
    {
        intervalFromBaseNote = _intervalFromBaseNote;
        octave = _octave;
        chordType = _chordType;
    }
}

public class Arpeggio
{
    public Chord chord;
    public float durationInCompasses;

    public Arpeggio(
        Chord _chord,
        float _durationInCompasses = -1
    )
    {
        chord = _chord;

        if (_durationInCompasses <= 0)
        {
            // Default duration is the one that corresponds to 1 beat per chord note
            durationInCompasses = ChordDictionary.chords[chord.chordType].Length * 1f / MusicParameters.instance.signature;
Debug.Log(ChordDictionary.chords[chord.chordType].Length);
Debug.Log(MusicParameters.instance.signature);
Debug.Log(durationInCompasses);
        }
    }

    public static Arpeggio DEFAULT = new Arpeggio(new Chord(0, 2));
}

public enum ArpeggioPattern
{
    CRESCENDO = 1,
    TWO_STEPS_AHEAD_ONE_BEHIND = 2,
    THREE_STEPS_AHEAD_ONE_BEHIND = 3,
}

public enum ChordType
{
    MAJOR = 1,
    MINOR = 2,
    MAJOR7M = 3,
    MINOR7M = 4,
    MAJOR7m = 5,
    MINOR7m = 6,
}
