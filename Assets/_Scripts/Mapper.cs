using System.Collections.Generic;
using UnityEngine;

namespace PMM.Demo
{
    public static class Mapper
    {
        public static List<Note> MapAudioClips(AudioClip[] samples)
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

        public static List<Note> GetScale(List<Note> notes, string[] scale)
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

        public static string[] GetNotesNamesFromIntervals(string tone, int[] scaleIntervals)
        {
            // TODO implement tone

            string[] notesNames = new string[7] { "C", "D", "E", "F", "G", "A", "B" };

            List<string> result = new List<string>();
            foreach (int i in scaleIntervals)
            {
                result.Add(notesNames[i - 1]);
            }

            return result.ToArray();
        }

        public static int[] GetNotesFromMelody(int[] melody, List<Note> allSamples, List<Note> scaleNotes)
        {
            // Get notes for our melody based on scale
            Note[] melodyNotes = new Note[melody.Length];

            for (int i = 0; i < melody.Length; i++)
            {
                melodyNotes[i] = scaleNotes[melody[i]];
            }

            // Find index of melody on original Note list to play
            int[] result = new int[melody.Length];
            int j = 0;

            foreach (Note n in melodyNotes)
            {
                result[j] = System.Array.FindIndex(allSamples.ToArray(), note => note == n);
                j++;
            }

            return result;
        }
    }
}
