using System.Collections.Generic;
using UnityEngine;

public static class Mapper {
	
	public static List<Note> MapAudioClips (AudioClip[] samples)
	{
		List<Note> notes = new List<Note>();

		foreach (AudioClip a in samples)
		{
			char[] n = a.name.ToCharArray();
			string name = n[1].ToString();
			bool sharp = false;
			int octave = 3;

			int.TryParse(n[0].ToString(), out octave);

			if (n.Length > 2)
			{
				if (n[2] == 's' || n[2] == '#')
				{
					name = name + "#";
					sharp = true;
				}
			}

			notes.Add(new Note(name, octave, sharp, a));
		}

		return notes;
	}

	public static List<Note> GetScale (List<Note> notes, string[] scale)
	{
		List<Note> scaleMap = new List<Note>();

		foreach (Note n in notes)
		{
			foreach (string s in scale)
			{
				if (n.name == s)
				{
					scaleMap.Add(n);
				}
			}
		}

		return scaleMap;
	}
}
