// Basic Linear Congruential Generator (LCG) for seeded random numbers
class SeededRandom {
    constructor(seed) {
        this.multiplier = 1664525;
        this.increment = 1013904223;
        this.modulus = Math.pow(2, 32); // 2^32
        this.seed = seed;
    }
    // Returns a pseudo-random float between 0 (inclusive) and 1 (exclusive)
    random() {
        this.seed = (this.multiplier * this.seed + this.increment) % this.modulus;
        return this.seed / this.modulus;
    }
    // Returns a pseudo-random integer between min (inclusive) and max (exclusive)
    nextInt(min, max) {
        return Math.floor(this.random() * (max - min)) + min;
    }
}
// Adapted from Stefan Gustavson's Java implementation, via Sean McCullough's JavaScript port
// https://gist.github.com/banksean/304522 (SimplexNoise)
class PerlinNoise {
    // private simplex: number[][]; // Not used in 2D noise, can be removed for clarity
    constructor(r) {
        if (r === undefined)
            r = Math;
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
        let n0, n1, n2; // Noise contributions from the three corners
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
        if (x0 > y0) {
            i1 = 1;
            j1 = 0;
        }
        else {
            i1 = 0;
            j1 = 1;
        }
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
        if (t0 < 0)
            n0 = 0.0;
        else {
            t0 *= t0;
            n0 = t0 * t0 * this.dot(this.grad3[gi0], x0, y0);
        }
        let t1 = 0.5 - x1 * x1 - y1 * y1;
        if (t1 < 0)
            n1 = 0.0;
        else {
            t1 *= t1;
            n1 = t1 * t1 * this.dot(this.grad3[gi1], x1, y1);
        }
        let t2 = 0.5 - x2 * x2 - y2 * y2;
        if (t2 < 0)
            n2 = 0.0;
        else {
            t2 *= t2;
            n2 = t2 * t2 * this.dot(this.grad3[gi2], x2, y2);
        }
        return 70.0 * (n0 + n1 + n2);
    }
}
const NOTE = 0;
const PAUSE = 1; // PAUSE is defined but not directly used in generateMelody, only in generateHeights
function inverseLerp(min, max, value) {
    if (min === max) {
        return 0;
    }
    return (value - min) / (max - min);
}
function roundToInt(value) {
    return Math.round(value);
}
function generateHeights(width, length, seed, noiseScale, // Renamed from 'scale' in C# to avoid confusion
range, octaves, persistance, lacunarity, offset) {
    const prng = new SeededRandom(seed); // PRNG for octave offsets
    const octaveOffsets = [];
    for (let i = 0; i < octaves; i++) {
        const offsetX = prng.nextInt(-100000, 100000) + offset.x;
        const offsetY = prng.nextInt(-100000, 100000) + offset.y;
        octaveOffsets.push({ x: offsetX, y: offsetY });
    }
    const perlinNoiseGenerator = new PerlinNoise(new SeededRandom(seed));
    if (noiseScale <= 0) {
        noiseScale = 0.0001;
    }
    const totalPoints = width * length;
    const heights = Array(totalPoints).fill(null).map(() => [0, 0]);
    const result = Array(totalPoints).fill(null).map(() => [0, 0]);
    let maxNoiseHeight = -Infinity;
    let minNoiseHeight = Infinity;
    for (let x = 0; x < width; x++) {
        for (let y = 0; y < length; y++) {
            let amplitude = 1;
            let frequency = 1;
            let noiseHeight = 0;
            for (let i = 0; i < octaves; i++) {
                const sampleX = (x / noiseScale) * frequency + octaveOffsets[i].x;
                const sampleY = (y / noiseScale) * frequency + octaveOffsets[i].y;
                const perlinValue = perlinNoiseGenerator.noise(sampleX, sampleY);
                noiseHeight += perlinValue * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }
            if (noiseHeight > maxNoiseHeight) {
                maxNoiseHeight = noiseHeight;
            }
            if (noiseHeight < minNoiseHeight) {
                minNoiseHeight = noiseHeight;
            }
            const index = length * x + y;
            heights[index][NOTE] = noiseHeight;
            heights[index][PAUSE] = noiseHeight;
        }
    }
    for (let i = 0; i < totalPoints; i++) {
        const normalizedNote = inverseLerp(minNoiseHeight, maxNoiseHeight, heights[i][NOTE]);
        result[i][NOTE] = roundToInt(normalizedNote * range);
        const normalizedPause = inverseLerp(minNoiseHeight, maxNoiseHeight, heights[i][PAUSE]);
        result[i][PAUSE] = roundToInt(normalizedPause * 4);
    }
    return result;
}
function generateMelody(perlinParameters, melodyParameters) {
    const currentMelodyParameters = melodyParameters !== null && melodyParameters !== void 0 ? melodyParameters : {
        tone: 0,
        octave: 2,
        scale: null,
    };
    const { width, length, seed, range, octaves, persistance, lacunarity, } = perlinParameters;
    const noiseScale = 20.0;
    const offset = { x: 0, y: 0 };
    const noiseMap = generateHeights(width, length, seed, noiseScale, range, octaves, persistance, lacunarity, offset);
    const melody = new Array(noiseMap.length);
    for (let i = 0; i < melody.length; i++) {
        melody[i] =
            noiseMap[i][NOTE] +
                currentMelodyParameters.octave * 12 +
                currentMelodyParameters.tone;
    }
    return melody;
}
// --- Test Code ---
// This block will only run if the file is executed directly (e.g., with ts-node or node after compilation)
// It won't run if the file is merely imported as a module.
if (typeof require !== 'undefined' && require.main === module) {
    const testPerlinParams = {
        length: 8, // How many notes in the melody
        width: 1, // Typically 1 for a single melody line
        seed: 1, // Seed for randomness
        octaves: 2, // Number of noise layers
        persistance: 0.5, // Amplitude scaling between octaves
        lacunarity: 2.0, // Frequency scaling between octaves
        range: 12 // Range of notes (e.g., 12 for one octave)
    };
    const testMelodyParams = {
        tone: 0, // Base note offset (0 = C, 1 = C#, etc.)
        octave: 3, // Default octave (e.g., 3 for middle C range)
        scale: null // No specific scale, use chromatic
    };
    console.log("--- Running Melody Generation Test ---");
    console.log("Test PerlinParameters:", testPerlinParams);
    console.log("Test MelodyParameters:", testMelodyParams);
    const melodyOutput = generateMelody(testPerlinParams, testMelodyParams);
    console.log("Generated Melody (notes):", melodyOutput);
    console.log(`Generated Melody Length: ${melodyOutput.length} (should match PerlinParameters.length * PerlinParameters.width)`);
    // Optional: Test with default melody parameters
    console.log("\n--- Running Melody Generation Test (Default Melody Params) ---");
    console.log("Test PerlinParameters:", testPerlinParams);
    const melodyOutputDefault = generateMelody(testPerlinParams);
    console.log("Generated Melody (default melody params):", melodyOutputDefault);
    // Optional: Direct test of generateHeights
    /*
    console.log("\n--- Running Heights Generation Test ---");
    const testOffset: Vector2 = { x: 0, y: 0 };
    const noiseScaleForHeights = 20.0; // As used in generateMelody
    console.log("Test PerlinParameters (for heights):", testPerlinParams);
    console.log(`Test noiseScale: ${noiseScaleForHeights}, offset:`, testOffset);

    const noiseMapOutput = generateHeights(
        testPerlinParams.width,
        testPerlinParams.length,
        testPerlinParams.seed,
        noiseScaleForHeights,
        testPerlinParams.range,
        testPerlinParams.octaves,
        testPerlinParams.persistance,
        testPerlinParams.lacunarity,
        testOffset
    );
    // Outputting the whole map can be verbose, so let's show a summary or first few entries
    console.log("Generated Noise Map (first 5 entries of [NOTE, PAUSE]):", noiseMapOutput.slice(0,5));
    console.log(`Noise Map Length: ${noiseMapOutput.length}`);
    */
    console.log("\n--- Test Execution Complete ---");
}
