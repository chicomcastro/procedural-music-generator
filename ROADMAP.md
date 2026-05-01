# Roadmap

This document tracks the planned evolution of the web version of the
Procedural Music Generator. The Unity prototype under `Assets/` is kept
as a historical reference and a source of feature ideas.

Tasks are grouped by horizon and roughly ordered by value / effort.

## Just shipped

- Dynamic piano keyboard covering C2–B5 (MIDI 36–83) so every default
  parameter combination is visible.
- BPM control wired into playback (previously hard-coded to 120).
- Scale selector: chromatic, major, natural minor, pentatonic
  major/minor, blues. Notes are quantized to the nearest scale degree
  based on the configured tonic (`tone`).
- Inline status messages replace the blocking `alert()` popups.
- **Rhythm / pauses**: the PAUSE track from Perlin noise is now
  decorrelated from pitch and mapped to note durations (rest, quarter,
  half, dotted half, whole).
- **Stop playback button**: cancels all scheduled oscillators and
  keyboard highlights on click.
- **MIDI export**: download the generated melody as a `.mid` file
  (SMF Format 0) with proper tempo and rhythm.

## Near term (next)

- [ ] **Note-length control**: expose "note value" (whole / half /
      quarter / eighth) alongside BPM so tempo and subdivision are
      independent.
- [ ] **Copy / share URL**: serialize current parameters into
      `location.hash` so a seed + settings combo is shareable.
- [ ] **Waveform selector**: sine only today — add square, sawtooth,
      triangle via `oscillator.type`.
- [ ] **Visualize the noise curve**: small canvas above the keyboard
      showing the raw Perlin values that become notes.

## Medium term

- [ ] **Harmony track**: port `Chord` / `ChordDictionary` / `Arpeggio`
      from `unity/Assets/Scripts/` to TS and play chords under the melody.
- [ ] **Bass line**: secondary voice with its own seed and lower
      octave, following the harmony.
- [ ] **Multiple voices**: generalize the generator to produce N voices
      with shared harmonic context (the Unity version already
      supports "multiple voices for melody").
- [ ] **Preset library**: save / load named parameter sets in
      `localStorage`.

## Long term

- [ ] **Adaptive / parametric design**: expose parameters that can be
      modulated over time (intensity, tension) so the music reacts to
      external input — matches the original "adaptive music for open
      world games" pitch.
- [ ] **Game-engine-friendly packaging**: publish the core generator
      as an npm package with a framework-agnostic API so it can be
      embedded in web games and other tools.
- [ ] **Real instrument samples**: swap the oscillator for sampled
      instruments (piano, pads) via a small sample library.
- [ ] **Documentation site** for use-as-asset purposes (also from the
      original README).

## Tech debt / housekeeping

- [ ] Add a `package.json` with a `tsc` script so `melody.ts` is the
      single source of truth and `melody.js` is generated.
- [ ] Minimal test setup (node's built-in test runner is fine) for
      `generateMelody` and `quantizeToScale`.
- [ ] Lint / format config (ESLint + Prettier).
- [ ] Decide the long-term status of the Unity project: keep, archive,
      or extract useful logic and remove.

## How to pick a task

Prefer small, user-visible changes first (rhythm, stop button, share
URL). Harmony and multi-voice are the biggest unlocks but require
non-trivial design work — sketch the data model in an issue before
coding.
