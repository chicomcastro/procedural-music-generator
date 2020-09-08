using System.Collections;
using System.Collections.Generic;

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