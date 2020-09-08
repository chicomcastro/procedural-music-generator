using System.Collections;
using System.Collections.Generic;

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

public enum ChordType
{
    MAJOR = 1,
    MINOR = 2,
    MAJOR7M = 3,
    MINOR7M = 4,
    MAJOR7m = 5,
    MINOR7m = 6,
}
