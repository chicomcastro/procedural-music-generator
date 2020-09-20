using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MelodyParameters {
	public int tone = 0;
	public int octave = 2;
	public int[] scale;

	public override string ToString()
    {
        string representation = "<MelodyParameters> tone: " + tone + "; octave: " + octave + "; scale: ";
		
		if (scale != null) representation += String.Join(", ", scale.Select(note => note.ToString()));
		else representation += "None";

		return representation;
    }
}