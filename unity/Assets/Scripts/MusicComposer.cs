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
        int[] melody;
        PerlinParameters perlinParameters;
        MelodyParameters melodyParameters;

        perlinParameters = new PerlinParameters();
        perlinParameters.range = 2 * 8;
        perlinParameters.length = 2;
        perlinParameters.width = 4;
        melody = MelodyProvider.GenerateMelody(perlinParameters);
        MelodyProvider.PrintMelodyData(melody);
        StartCoroutine(PlayMelody(melody));

        perlinParameters.range = 1 * 8;
        perlinParameters.length = 1;
        perlinParameters.width = 4;
        melodyParameters = new MelodyParameters();
        melodyParameters.octave = 4;
        melody = MelodyProvider.GenerateMelody(perlinParameters, melodyParameters);
        StartCoroutine(PlayMelody(melody: melody, initialWait: 4, noteDuration: 2));

        perlinParameters.range = 1 * 8;
        perlinParameters.length = 1;
        perlinParameters.width = 8;
        perlinParameters.seed = 44;
        melodyParameters = new MelodyParameters();
        melodyParameters.octave = 6;
        melody = MelodyProvider.GenerateMelody(perlinParameters, melodyParameters);
        StartCoroutine(PlayMelody(melody: melody, initialWait: 8, noteDuration: 4));
    }

    IEnumerator PlayMelody(int[] melody, float repeatQuant = -1f, int initialWait = 0, int noteDuration = 1)
    {
        if (repeatQuant == -1) {
            repeatQuant = Mathf.Infinity;
        }
        while (initialWait > 0) {
            print("Waiting");
            yield return NextCompass();
            initialWait--;
        }
        for (int repetition = 0; repetition < repeatQuant; repetition++)
        {
            foreach (int note in melody)
            {
                print("Playing: " + note);
                yield return WaitBeats(noteDuration);
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

    private WaitForSeconds NextCompass()
    {
        return new WaitForSeconds(60f / MusicParameters.instance.bpm * MusicParameters.instance.signature);
    }

    private WaitForSeconds NextBeat()
    {
        return new WaitForSeconds(60f / MusicParameters.instance.bpm);
    }

    private WaitForSeconds NextHalfBeat()
    {
        return new WaitForSeconds(60f / MusicParameters.instance.bpm / 2);
    }

    private WaitForSeconds WaitBeats(int beatsToWait)
    {
        return new WaitForSeconds(60f / MusicParameters.instance.bpm * beatsToWait);
    }
}
