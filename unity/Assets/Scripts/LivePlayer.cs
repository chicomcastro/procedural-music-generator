using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo script to play notes through computer's keyboard
/// </summary>
public class LivePlayer : MonoBehaviour
{
	#region Live keyboard
	public MusicPlayer musicPlayer;
	public int transpose = 0;  // transpose in semitones
    public int octave;
	public ChordType chordType = ChordType.MAJOR;
	public LivePlayerMode livePlayerMode = LivePlayerMode.NOTE;

	private void Start() {
		octave = musicPlayer.baseOctave;
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

        if (Input.GetKeyDown(KeyCode.UpArrow)) octave += 1;
        if (Input.GetKeyDown(KeyCode.DownArrow)) octave += -1;

        // if some key pressed...
		if (notes.Count > 0)
		{
            foreach(int note in notes) {
				switch (livePlayerMode)
				{
					case LivePlayerMode.NOTE:
						musicPlayer.PlayNote(note + transpose, octave);
						break;
					case LivePlayerMode.CHORD:
						musicPlayer.PlayChord(new Chord(note + transpose, octave, chordType));
						break;
					case LivePlayerMode.ARPEGGIO:
						musicPlayer.PlayChordArpeggio(
							new Arpeggio(
								new Chord(note + transpose, octave, chordType),
								1f,
								ArpeggioPatternRepository.patterns[ArpeggioPattern.OPEN_CHORD_WITHOUT_THIRD]
							)
						);
						break;
					default:
						break;
				}

                // Control the position of the main VisualNote instantiated
                PositionController.MoveNoteToPosition(VisualNote.main, note + transpose + octave * 12);
            }
		}
	}
	#endregion
}

public enum LivePlayerMode {
	NOTE = 1,
	CHORD = 2,
	ARPEGGIO = 3
}
