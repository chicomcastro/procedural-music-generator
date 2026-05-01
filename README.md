# Procedural Music Generator

A tool for generating melodies procedurally using Perlin noise. Designed for infinite, adaptive music — especially suited for open-world games.

**[Try the web demo](https://chicomcastro.github.io/procedural-music-generator/)** · **[Unity prototype on itch.io](https://chicomcastro.itch.io/music-maker)**

## Quick start (web)

Open `web/index.html` in a modern browser (or use the [live demo](https://chicomcastro.github.io/procedural-music-generator/)). No build step or server required.

The generator creates melodies from Perlin noise controlled by seed, octaves, lacunarity, and persistence. You can pick a scale (major, minor, pentatonic, blues, chromatic), set BPM, and hear the result instantly through the Web Audio API.

## How it works

Perlin noise produces a smooth, deterministic curve from a given seed. That curve is mapped to pitch values and quantized to the chosen musical scale. Tweaking the noise parameters changes the character of the melody:

| Parameter | Effect |
|---|---|
| **Seed** | Determines the shape of the noise curve — same seed = same melody |
| **Octaves** | Number of noise layers combined. More octaves = more detail |
| **Lacunarity** | Frequency multiplier per octave. Higher = less smooth, more variation |
| **Persistence** | Amplitude retention per octave (0–1). Lower = smoother, dominated by the base shape |
| **Note range** | How many semitones the melody can span |
| **Length** | Number of notes generated |

**Tips:** try two melodies with the same parameters but different seeds, or use lower BPM with few octaves for a calmer feel.

## Project structure

| Path | Description |
|---|---|
| `web/melody.ts` | Core generator (Perlin noise + scale quantization) — source of truth |
| `web/melody.js` | Compiled output of `melody.ts` (checked in until a build step is added) |
| `web/app.js` | Web UI wiring (controls, keyboard rendering, playback) |
| `web/index.html` / `style.css` | Web interface |
| `unity/` | Legacy Unity prototype with chords, arpeggios, MIDI integration, and multi-voice support |

## Roadmap

See [ROADMAP.md](./ROADMAP.md) for the detailed task list. High-level goals:

- [x] Procedural melody generation
- [x] Configurable scales and BPM
- [x] Dynamic piano keyboard visualization
- [x] Rhythm and rests (Perlin noise PAUSE track)
- [x] Stop playback button
- [x] MIDI file export
- [ ] Note-length control (subdivision independent of BPM)
- [ ] Harmony and chord tracks
- [ ] Bass line
- [ ] Multiple simultaneous voices
- [ ] Adaptive/parametric design for game integration

## Screenshot

The Unity prototype's sequencer grid (the web version uses an interactive piano keyboard):

![Unity sequencer](https://raw.githubusercontent.com/chicomcastro/procedural-music-generator/master/unity/Screenshot1.PNG)

## License

[MIT](./LICENSE) — Francisco Matheus Moreira de Castro, 2017
