using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicComposer : MonoBehaviour
{
    public MusicPlayer musicPlayer;

    private void Start()
    {
        Play();
    }

    [ContextMenu("Play music")]
    public void Play()
    {
        int[] melody = MelodyProvider.GenerateMelody(new PerlinParameters());
        MelodyProvider.PrintMelodyData(melody);
        StartCoroutine(PlayMelody(melody));
    }

    IEnumerator PlayMelody(int[] melody, int repeatQuant = 1)
    {
        for (int repetition = 0; repetition < repeatQuant; repetition++)
        {
            print("Repeating: " + repetition);
            foreach (int note in melody)
            {
                print("Playing: " + note);
                yield return NextBeat();
                musicPlayer.PlayNote(note % 12, (note / 12) - musicPlayer.baseOctave);
                PositionController.MoveNoteToPosition(VisualNote.main, note);
            }
        }
    }

    IEnumerator PlayScriptMusic()
    {
        // Wrote here your music
        // TODO turn this visual
        int octave = 4;
        Chord C = new Chord(0, octave, ChordType.MAJOR);
        Chord E = new Chord(4, octave, ChordType.MAJOR);
        Chord F = new Chord(5, octave, ChordType.MAJOR);

        // while (true)
        // {
        for (int i = 0; i < 16; i++)
        {
            yield return NextCompass();
            musicPlayer.PlayChord(C);

            yield return NextBeat();
            musicPlayer.PlayChord(C);

            yield return NextBeat();
            musicPlayer.PlayChord(C);

            yield return NextBeat();
            musicPlayer.PlayChord(C);
        }
        // }

        // // Example:
        // NextCompass();
        // // Compasso 1

        // NextCompass();
        // // Compasso 2

        // NextCompass();
        // // Compasso 3

        // // ...

        yield return null;
    }

    private WaitUntil NextCompass()
    {
        return new WaitUntil(() => AudioManager.instance.currentTempo == 1f);
    }

    private WaitForSeconds NextBeat()
    {
        return new WaitForSeconds(60f / MusicParameters.instance.bpm);
    }

    private WaitForSeconds NextHalfBeat()
    {
        return new WaitForSeconds(60f / MusicParameters.instance.bpm / 2);
    }
}
