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
    octave: document.getElementById('m-octave'),
    scale: document.getElementById('m-scale')
};

const playbackParamsControls = {
    bpm: document.getElementById('p-bpm')
};

// Buttons and status
const generateMelodyBtn = document.getElementById('generate-melody-btn');
const playMelodyBtn = document.getElementById('play-melody-btn');
const statusMessage = document.getElementById('status-message');

function setStatus(text, kind) {
    if (!statusMessage) return;
    statusMessage.textContent = text || '';
    statusMessage.className = 'status-message' + (kind ? ' status-' + kind : '');
}

if (playMelodyBtn) {
    playMelodyBtn.disabled = true;
}

if (perlinParamsControls.persistance && perlinParamsControls.persistanceVal) {
    perlinParamsControls.persistance.addEventListener('input', () => {
        perlinParamsControls.persistanceVal.textContent = perlinParamsControls.persistance.value;
    });
}

// Scale definitions: semitone offsets from the tonic
const SCALES = {
    'chromatic': null,
    'major': [0, 2, 4, 5, 7, 9, 11],
    'minor': [0, 2, 3, 5, 7, 8, 10],
    'pentatonic-major': [0, 2, 4, 7, 9],
    'pentatonic-minor': [0, 3, 5, 7, 10],
    'blues': [0, 3, 5, 6, 7, 10]
};

// Piano keyboard: MIDI 36 (C2) to 83 (B5)
const PIANO_START_MIDI = 36;
const PIANO_END_MIDI = 83;
const NOTE_NAMES = ['C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B'];
const noteToKeyId = {};

function midiToNoteName(midi) {
    const pitchClass = ((midi % 12) + 12) % 12;
    const octave = Math.floor(midi / 12) - 1;
    return NOTE_NAMES[pitchClass] + octave;
}

function renderPiano() {
    const piano = document.getElementById('piano');
    if (!piano) return;
    piano.innerHTML = '';
    for (let midi = PIANO_START_MIDI; midi <= PIANO_END_MIDI; midi++) {
        const pitchClass = ((midi % 12) + 12) % 12;
        const octave = Math.floor(midi / 12) - 1;
        const name = NOTE_NAMES[pitchClass];
        const isBlack = name.includes('#');
        const keyId = 'key-' + name.replace('#', 's') + octave;
        const el = document.createElement('div');
        el.className = 'key ' + (isBlack ? 'black' : 'white');
        el.dataset.note = name + octave;
        el.id = keyId;
        if (!isBlack) {
            const label = document.createElement('span');
            label.className = 'key-label';
            label.textContent = name + octave;
            el.appendChild(label);
        }
        piano.appendChild(el);
        noteToKeyId[midi] = keyId;
    }
}

renderPiano();

function getPerlinParameters() {
    return {
        length: parseInt(perlinParamsControls.length.value),
        width: 1,
        seed: parseInt(perlinParamsControls.seed.value),
        octaves: parseInt(perlinParamsControls.octaves.value),
        persistance: parseFloat(perlinParamsControls.persistance.value),
        lacunarity: parseFloat(perlinParamsControls.lacunarity.value),
        range: parseInt(perlinParamsControls.range.value)
    };
}

function getMelodyParameters() {
    const scaleKey = melodyParamsControls.scale ? melodyParamsControls.scale.value : 'chromatic';
    return {
        tone: parseInt(melodyParamsControls.tone.value),
        octave: parseInt(melodyParamsControls.octave.value),
        scale: SCALES[scaleKey] || null
    };
}

function getBpm() {
    const raw = parseInt(playbackParamsControls.bpm.value);
    if (isNaN(raw) || raw <= 0) return 120;
    return raw;
}

let currentMelody = [];

if (generateMelodyBtn) {
    generateMelodyBtn.addEventListener('click', () => {
        const pParams = getPerlinParameters();
        const mParams = getMelodyParameters();

        if (typeof generateMelody === 'function') {
            try {
                currentMelody = generateMelody(pParams, mParams);
                setStatus(`Melody generated with ${currentMelody.length} notes.`, 'ok');
                if (playMelodyBtn) {
                    playMelodyBtn.disabled = (!currentMelody || currentMelody.length === 0);
                }
            } catch (error) {
                console.error("Error generating melody:", error);
                setStatus('Error generating melody. Check console.', 'error');
                currentMelody = [];
                if (playMelodyBtn) playMelodyBtn.disabled = true;
            }
        } else {
            console.error("generateMelody function not found.");
            setStatus('Melody generation script not loaded correctly.', 'error');
            currentMelody = [];
            if (playMelodyBtn) playMelodyBtn.disabled = true;
        }
    });
}

let audioContext;

function initAudioContext() {
    if (!audioContext) {
        audioContext = new (window.AudioContext || window.webkitAudioContext)();
    }
}

function midiToFreq(midiNote) {
    return 440 * Math.pow(2, (midiNote - 69) / 12);
}

function playNote(midiNoteNumber, startTime, duration) {
    if (!audioContext) return;
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();
    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);
    oscillator.type = 'sine';
    oscillator.frequency.setValueAtTime(midiToFreq(midiNoteNumber), startTime);

    const attackTime = 0.05;
    const decayTime = 0.1;
    const sustainLevel = 0.7;
    const releaseTime = Math.min(0.2, duration * 0.4);
    const notePlayDuration = Math.max(duration - releaseTime, 0.01);

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
            const nowOffset = Math.max(0, (startTime - audioContext.currentTime) * 1000);
            const endOffset = Math.max(0, (startTime - audioContext.currentTime + duration) * 1000);
            setTimeout(() => keyElement.classList.add('active'), nowOffset);
            setTimeout(() => keyElement.classList.remove('active'), endOffset);
        }
    }
}

if (playMelodyBtn) {
    playMelodyBtn.addEventListener('click', () => {
        initAudioContext();
        if (!currentMelody || currentMelody.length === 0) {
            setStatus("No melody generated yet. Click 'Generate Melody' first.", 'warn');
            return;
        }
        if (!audioContext) {
            setStatus('AudioContext could not be initialized.', 'error');
            return;
        }

        const noteDuration = 60 / getBpm();
        playMelodyBtn.disabled = true;
        if (generateMelodyBtn) generateMelodyBtn.disabled = true;
        setStatus('Playing…', 'ok');

        const melodyStartTime = audioContext.currentTime + 0.1;
        for (let i = 0; i < currentMelody.length; i++) {
            const midiNote = currentMelody[i];
            const noteStartTime = melodyStartTime + i * noteDuration;
            playNote(midiNote, noteStartTime, noteDuration);
        }

        const totalMelodyDuration = currentMelody.length * noteDuration;
        setTimeout(() => {
            playMelodyBtn.disabled = false;
            if (generateMelodyBtn) generateMelodyBtn.disabled = false;
            setStatus('Playback finished.', 'ok');
        }, (totalMelodyDuration + 0.5) * 1000);
    });
}
