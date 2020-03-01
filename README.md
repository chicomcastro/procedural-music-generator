# Procedural Music Generator
Development of a online sequencer with procedural generation of melodies

This is a little tool that I'm developing for musical purposes.

It could be used to make infinite and adaptative music for games, mainly for open world games.

But, for now, it's only a prototype on Unity, hope to develop it a little more as soon as I can.

## What happens next?
I'm planning to work on:
- Procedural melody generation (OK!)
- Multiple voices for melody (OK!)
- Adaptative, parametrical design (in dev)
- Friendly user interface (in dev)
- Harmony
- Rhythm
- Bass
- MIDI output for download
- Full documentation for use-as-asset purposes

## What's going on?
I have used perlin noise for seed-based generation, for what you can modify atributes like octaves, persistance, lacunarity, and so on. Have fun! :)

### Tips
Tips that I found nice:

1 - It's cool to combine two melodies with same atributes, but different seeds.

2 - For low sizes or octaves, it sounds more cool for me at lower bpm.

### Little explanations
**BPM**: Beats per minute or, basically, the velocity of the music.

**Dimensions**: here it doesn't have a clear meaning, but it affects how perlin noise generates the melody. It's associated with the dimensions of the noise and may affect melody's smoothness.

**Size**: The length of the music (press + to update notes' keyboard)

**Seed**: it's the key to feed and save the status of the procedural generation

**Octave**: the number of interactions until the final result. It have greater effects combined with the properties bellow.

**Lacunarity**: it's a kind of frequency multiple factor for each new octave. For higher values, less smooth will be the final result (with a more detailed shape).

**Persistance**: it's a number between 0 and 1 associated with the maintence of the fundamental amplitude in each octave, that will naturally reduces (for less-than-1 values)
