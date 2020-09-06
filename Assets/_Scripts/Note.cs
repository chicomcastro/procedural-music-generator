using UnityEngine;

[System.Serializable]
public class Note
{
	public string name;
	public int octave;
	public bool sharp;
	public AudioClip clip;

	public Note (string _name, int _octave, bool _sharp, AudioClip _audioClip)
	{
		name = _name;
		octave = _octave;
		sharp = _sharp;
		clip = _audioClip;
	}
}