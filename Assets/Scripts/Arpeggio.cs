using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Arpeggio
{
    public Chord chord;
    public float durationInCompasses;
    public NoteInterval[] arpeggioPattern;

    public Arpeggio(
        Chord _chord,
        float _durationInCompasses = 0,
        NoteInterval[] _arpeggioPattern = null
    )
    {
        chord = _chord;

        int[] chordNotes = ChordDictionary.chords[chord.chordType];
        if (_durationInCompasses <= 0f)
        {
            // Default duration is the one that corresponds to 1 beat per chord note
            durationInCompasses = chordNotes.Length * 1f / MusicParameters.instance.signature;
        }
        else
        {
            durationInCompasses = _durationInCompasses;
        }

        if (_arpeggioPattern == null)
        {
            arpeggioPattern = chordNotes.Select(note => IntervalDictionary.numberToName[note]).ToArray();
        }
        else
        {
            arpeggioPattern = _arpeggioPattern;
        }
    }

    // public static Arpeggio DEFAULT = new Arpeggio(new Chord(0, 2));
}

public static class ArpeggioPatternRepository
{
    public static Dictionary<ArpeggioPattern, NoteInterval[]> patterns = new Dictionary<ArpeggioPattern, NoteInterval[]> {
        {
            ArpeggioPattern.OPEN_CHORD_MAJOR_THIRD,
            new NoteInterval[6] {
                NoteInterval.UNISON,
                NoteInterval.PERFECT_FIFTH,
                NoteInterval.PERFECT_OCTAVE,
                NoteInterval.MAJOR_TENTH,
                NoteInterval.PERFECT_TWELFTH,
                NoteInterval.DOUBLE_OCTAVE
            }
        },
        {
            ArpeggioPattern.OPEN_CHORD_MINOR_THIRD,
            new NoteInterval[6] {
                NoteInterval.UNISON,
                NoteInterval.PERFECT_FIFTH,
                NoteInterval.PERFECT_OCTAVE,
                NoteInterval.MINOR_TENTH,
                NoteInterval.PERFECT_TWELFTH,
                NoteInterval.DOUBLE_OCTAVE
            }
        },
        {
            ArpeggioPattern.OPEN_CHORD_WITHOUT_THIRD,
            new NoteInterval[8] {
                NoteInterval.UNISON,
                NoteInterval.PERFECT_FIFTH,
                NoteInterval.PERFECT_OCTAVE,
                NoteInterval.PERFECT_TWELFTH,
                NoteInterval.DOUBLE_OCTAVE,
                NoteInterval.PERFECT_TWELFTH,
                NoteInterval.PERFECT_OCTAVE,
                NoteInterval.PERFECT_FIFTH,
            }
        },
    };
}

public enum ArpeggioPattern {
    OPEN_CHORD_MAJOR_THIRD = 1,
    OPEN_CHORD_MINOR_THIRD = 2,
    OPEN_CHORD_WITHOUT_THIRD = 3,
}