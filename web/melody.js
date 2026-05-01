// Basic Linear Congruential Generator (LCG) for seeded random numbers
class SeededRandom {
    constructor(seed) {
        this.multiplier = 1664525;
        this.increment = 1013904223;
        this.modulus = Math.pow(2, 32);
        this.seed = seed;
    }
    random() {
        this.seed = (this.multiplier * this.seed + this.increment) % this.modulus;
        return this.seed / this.modulus;
    }
    nextInt(min, max) {
        return Math.floor(this.random() * (max - min)) + min;
    }
}

// Adapted from Stefan Gustavson's Java implementation, via Sean McCullough's JavaScript port
// https://gist.github.com/banksean/304522 (SimplexNoise)
class PerlinNoise {
    constructor(r) {
        if (r === undefined) r = Math;
        this.grad3 = [
            [1, 1, 0], [-1, 1, 0], [1, -1, 0], [-1, -1, 0],
            [1, 0, 1], [-1, 0, 1], [1, 0, -1], [-1, 0, -1],
            [0, 1, 1], [0, -1, 1], [0, 1, -1], [0, -1, -1],
        ];
        this.p = [];
        for (let i = 0; i < 256; i++) {
            this.p[i] = Math.floor(r.random() * 256);
        }
        this.perm = [];
        for (let i = 0; i < 512; i++) {
            this.perm[i] = this.p[i & 255];
        }
    }
    dot(g, x, y) {
        return g[0] * x + g[1] * y;
    }
    noise(xin, yin) {
        let n0, n1, n2;
        const F2 = 0.5 * (Math.sqrt(3.0) - 1.0);
        const s = (xin + yin) * F2;
        const i = Math.floor(xin + s);
        const j = Math.floor(yin + s);
        const G2 = (3.0 - Math.sqrt(3.0)) / 6.0;
        const t = (i + j) * G2;
        const X0 = i - t;
        const Y0 = j - t;
        const x0 = xin - X0;
        const y0 = yin - Y0;
        let i1, j1;
        if (x0 > y0) { i1 = 1; j1 = 0; }
        else { i1 = 0; j1 = 1; }
        const x1 = x0 - i1 + G2;
        const y1 = y0 - j1 + G2;
        const x2 = x0 - 1.0 + 2.0 * G2;
        const y2 = y0 - 1.0 + 2.0 * G2;
        const ii = i & 255;
        const jj = j & 255;
        const gi0 = this.perm[ii + this.perm[jj]] % 12;
        const gi1 = this.perm[ii + i1 + this.perm[jj + j1]] % 12;
        const gi2 = this.perm[ii + 1 + this.perm[jj + 1]] % 12;
        let t0 = 0.5 - x0 * x0 - y0 * y0;
        if (t0 < 0) n0 = 0.0;
        else { t0 *= t0; n0 = t0 * t0 * this.dot(this.grad3[gi0], x0, y0); }
        let t1 = 0.5 - x1 * x1 - y1 * y1;
        if (t1 < 0) n1 = 0.0;
        else { t1 *= t1; n1 = t1 * t1 * this.dot(this.grad3[gi1], x1, y1); }
        let t2 = 0.5 - x2 * x2 - y2 * y2;
        if (t2 < 0) n2 = 0.0;
        else { t2 *= t2; n2 = t2 * t2 * this.dot(this.grad3[gi2], x2, y2); }
        return 70.0 * (n0 + n1 + n2);
    }
}

const NOTE = 0;
const PAUSE = 1;

function inverseLerp(min, max, value) {
    if (min === max) return 0;
    return (value - min) / (max - min);
}

function roundToInt(value) {
    return Math.round(value);
}

function generateHeights(width, length, seed, noiseScale, range, octaves, persistance, lacunarity, offset) {
    const prng = new SeededRandom(seed);
    const octaveOffsets = [];
    for (let i = 0; i < octaves; i++) {
        const offsetX = prng.nextInt(-100000, 100000) + offset.x;
        const offsetY = prng.nextInt(-100000, 100000) + offset.y;
        octaveOffsets.push({ x: offsetX, y: offsetY });
    }
    const perlinNoiseGenerator = new PerlinNoise(new SeededRandom(seed));
    if (noiseScale <= 0) noiseScale = 0.0001;

    const totalPoints = width * length;
    const heights = Array(totalPoints).fill(null).map(() => [0, 0]);
    const result = Array(totalPoints).fill(null).map(() => [0, 0]);

    let maxNoiseHeight = -Infinity;
    let minNoiseHeight = Infinity;
    let maxPauseHeight = -Infinity;
    let minPauseHeight = Infinity;

    for (let x = 0; x < width; x++) {
        for (let y = 0; y < length; y++) {
            let amplitude = 1;
            let frequency = 1;
            let noiseHeight = 0;
            let pauseHeight = 0;

            for (let i = 0; i < octaves; i++) {
                const sampleX = (x / noiseScale) * frequency + octaveOffsets[i].x;
                const sampleY = (y / noiseScale) * frequency + octaveOffsets[i].y;
                noiseHeight += perlinNoiseGenerator.noise(sampleX, sampleY) * amplitude;
                pauseHeight += perlinNoiseGenerator.noise(sampleX + 1000, sampleY + 1000) * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }

            if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
            if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            if (pauseHeight > maxPauseHeight) maxPauseHeight = pauseHeight;
            if (pauseHeight < minPauseHeight) minPauseHeight = pauseHeight;

            const index = length * x + y;
            heights[index][NOTE] = noiseHeight;
            heights[index][PAUSE] = pauseHeight;
        }
    }

    for (let i = 0; i < totalPoints; i++) {
        const normalizedNote = inverseLerp(minNoiseHeight, maxNoiseHeight, heights[i][NOTE]);
        result[i][NOTE] = roundToInt(normalizedNote * range);

        const normalizedPause = inverseLerp(minPauseHeight, maxPauseHeight, heights[i][PAUSE]);
        result[i][PAUSE] = roundToInt(normalizedPause * 4);
    }

    return result;
}

function quantizeToScale(midiNote, tonicPitchClass, scale) {
    if (!scale || scale.length === 0) return midiNote;
    const degreeFromTonic = midiNote - tonicPitchClass;
    const octaveOffset = Math.floor(degreeFromTonic / 12);
    const semitoneInOctave = ((degreeFromTonic % 12) + 12) % 12;
    let best = scale[0];
    let bestDistance = Math.abs(semitoneInOctave - scale[0]);
    for (let i = 1; i < scale.length; i++) {
        const distance = Math.abs(semitoneInOctave - scale[i]);
        if (distance < bestDistance) {
            best = scale[i];
            bestDistance = distance;
        }
    }
    return tonicPitchClass + octaveOffset * 12 + best;
}

function generateMelody(perlinParameters, melodyParameters) {
    const currentMelodyParameters = melodyParameters ?? {
        tone: 0,
        octave: 2,
        scale: null,
    };

    const { width, length, seed, range, octaves, persistance, lacunarity } = perlinParameters;
    const noiseScale = 20.0;
    const offset = { x: 0, y: 0 };

    const noiseMap = generateHeights(width, length, seed, noiseScale, range, octaves, persistance, lacunarity, offset);
    const melody = new Array(noiseMap.length);

    for (let i = 0; i < melody.length; i++) {
        const raw = noiseMap[i][NOTE] + currentMelodyParameters.octave * 12 + currentMelodyParameters.tone;
        melody[i] = {
            midi: quantizeToScale(raw, currentMelodyParameters.tone, currentMelodyParameters.scale),
            duration: noiseMap[i][PAUSE],
        };
    }

    return melody;
}
