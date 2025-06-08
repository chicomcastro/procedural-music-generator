// DOM Elements for Parameters
const perlinParamsControls = {
    length: document.getElementById('p-length'),
    seed: document.getElementById('p-seed'),
    octaves: document.getElementById('p-octaves'),
    persistance: document.getElementById('p-persistance'),
    persistanceVal: document.getElementById('p-persistance-val'),
    lacunarity: document.getElementById('p-lacunarity'),
    range: document.getElementById('p-range')
};

const melodyParamsControls = {
    tone: document.getElementById('m-tone'),
    octave: document.getElementById('m-octave')
};

// Buttons
const generateMelodyBtn = document.getElementById('generate-melody-btn');
const playMelodyBtn = document.getElementById('play-melody-btn');

// Disable Play Melody button initially
if (playMelodyBtn) {
    playMelodyBtn.disabled = true;
}

// Update persistance value display
if (perlinParamsControls.persistance && perlinParamsControls.persistanceVal) {
    perlinParamsControls.persistance.addEventListener('input', () => {
        perlinParamsControls.persistanceVal.textContent = perlinParamsControls.persistance.value;
    });
}

// Function to get current parameter values from UI
function getPerlinParameters() {
    return {
        length: parseInt(perlinParamsControls.length.value),
        width: 1, // Fixed for single melody line
        seed: parseInt(perlinParamsControls.seed.value),
        octaves: parseInt(perlinParamsControls.octaves.value),
        persistance: parseFloat(perlinParamsControls.persistance.value),
        lacunarity: parseFloat(perlinParamsControls.lacunarity.value),
        range: parseInt(perlinParamsControls.range.value)
    };
}

function getMelodyParameters() {
    return {
        tone: parseInt(melodyParamsControls.tone.value),
        octave: parseInt(melodyParamsControls.octave.value),
        scale: null // Scale UI not implemented in this step
    };
}

let currentMelody = []; // To store the generated melody

if (generateMelodyBtn) {
    generateMelodyBtn.addEventListener('click', () => {
        const pParams = getPerlinParameters();
        const mParams = getMelodyParameters();

        console.log("Perlin Params from UI:", pParams);
        console.log("Melody Params from UI:", mParams);

        if (typeof generateMelody === 'function') {
            try {
                currentMelody = generateMelody(pParams, mParams);
                console.log("Generated Melody:", currentMelody);
                alert(`Melody generated with ${currentMelody.length} notes!`);

                if (playMelodyBtn) {
                    playMelodyBtn.disabled = (!currentMelody || currentMelody.length === 0);
                }
            } catch (error) {
                console.error("Error generating melody:", error);
                alert("Error generating melody. Check console for details.");
                currentMelody = []; // Clear melody on error
                if (playMelodyBtn) {
                    playMelodyBtn.disabled = true;
                }
            }
        } else {
            console.error("generateMelody function not found. Ensure melody.js is loaded and correct.");
            alert("Melody generation script not loaded correctly.");
            currentMelody = []; // Clear melody if function not found
            if (playMelodyBtn) {
                playMelodyBtn.disabled = true;
            }
        }
    });
} else {
    console.error("Generate Melody button not found.");
}

// Web Audio API and Playback
let audioContext;
let tempo = 120; // BPM
let noteDuration = 60 / tempo; // Duration of a quarter note in seconds

function initAudioContext() {
    if (!audioContext) {
        audioContext = new (window.AudioContext || window.webkitAudioContext)();
        console.log("AudioContext initialized.");
    }
}

function midiToFreq(midiNote) {
    return 440 * Math.pow(2, (midiNote - 69) / 12);
}

function mapNoteToMidi(noteValue) {
    return noteValue;
}

const noteToKeyId = {
    60: 'key-C4', 61: 'key-Cs4', 62: 'key-D4', 63: 'key-Ds4', 64: 'key-E4',
    65: 'key-F4', 66: 'key-Fs4', 67: 'key-G4', 68: 'key-Gs4', 69: 'key-A4',
    70: 'key-As4', 71: 'key-B4',
    72: 'key-C5', 73: 'key-Cs5', 74: 'key-D5', 75: 'key-Ds5', 76: 'key-E5',
    77: 'key-F5', 78: 'key-Fs5', 79: 'key-G5', 80: 'key-Gs5', 81: 'key-A5',
    82: 'key-As5', 83: 'key-B5'
};

function playNote(midiNoteNumber, startTime, duration) {
    if (!audioContext) {
        console.warn("AudioContext not initialized. Cannot play note.");
        return;
    }
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();
    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);
    oscillator.type = 'sine';
    oscillator.frequency.setValueAtTime(midiToFreq(midiNoteNumber), startTime);

    const attackTime = 0.05;
    const decayTime = 0.1;
    const sustainLevel = 0.7;
    const releaseTime = 0.2;
    const notePlayDuration = duration - releaseTime > 0 ? duration - releaseTime : 0.01;

    gainNode.gain.setValueAtTime(0, startTime);
    gainNode.gain.linearRampToValueAtTime(1.0, startTime + attackTime);
    gainNode.gain.linearRampToValueAtTime(sustainLevel, startTime + attackTime + decayTime);
    gainNode.gain.setValueAtTime(sustainLevel, startTime + notePlayDuration);
    gainNode.gain.linearRampToValueAtTime(0, startTime + notePlayDuration + releaseTime);
    oscillator.start(startTime);
    oscillator.stop(startTime + duration + 0.1);

    const keyId = noteToKeyId[midiNoteNumber];
    if (keyId) {
        const keyElement = document.getElementById(keyId);
        if (keyElement) {
            const highlightDelay = (startTime - audioContext.currentTime) > 0 ? (startTime - audioContext.currentTime) * 1000 : 0;
            const removeHighlightDelay = (startTime - audioContext.currentTime + duration) > 0 ? (startTime - audioContext.currentTime + duration) * 1000 : duration * 1000;
            setTimeout(() => keyElement.classList.add('active'), highlightDelay);
            setTimeout(() => keyElement.classList.remove('active'), removeHighlightDelay);
        }
    }
}

if (playMelodyBtn) {
    playMelodyBtn.addEventListener('click', () => {
        initAudioContext();
        if (!currentMelody || currentMelody.length === 0) {
            alert("No melody generated yet, or melody is empty. Click 'Generate Melody' first.");
            return;
        }
        if (!audioContext) {
            alert("AudioContext could not be initialized.");
            return;
        }

        if (playMelodyBtn) playMelodyBtn.disabled = true;
        if (generateMelodyBtn) generateMelodyBtn.disabled = true;

        console.log("Playing melody:", currentMelody);
        const melodyStartTime = audioContext.currentTime + 0.1;
        for (let i = 0; i < currentMelody.length; i++) {
            const noteValue = currentMelody[i];
            const midiNote = mapNoteToMidi(noteValue);
            const noteStartTime = melodyStartTime + i * noteDuration;
            playNote(midiNote, noteStartTime, noteDuration);
        }

        const totalMelodyDuration = currentMelody.length * noteDuration;
        setTimeout(() => {
            if (playMelodyBtn) playMelodyBtn.disabled = false;
            if (generateMelodyBtn) generateMelodyBtn.disabled = false;
            console.log("Playback finished. Controls re-enabled.");
        }, (totalMelodyDuration + 0.5) * 1000);
    });
} else {
    console.error("Play Melody button not found.");
}

console.log('app.js fully loaded with UI refinements and playback logic.');
