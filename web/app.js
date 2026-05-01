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
const stopMelodyBtn = document.getElementById('stop-melody-btn');
const exportMidiBtn = document.getElementById('export-midi-btn');
const statusMessage = document.getElementById('status-message');

function setStatus(text, kind) {
    if (!statusMessage) return;
    statusMessage.textContent = text || '';
    statusMessage.className = 'status-message' + (kind ? ' status-' + kind : '');
}

if (playMelodyBtn) playMelodyBtn.disabled = true;
if (stopMelodyBtn) stopMelodyBtn.disabled = true;
if (exportMidiBtn) exportMidiBtn.disabled = true;

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

// State
let currentMelody = []; // NoteEvent[]
let scheduledOscillators = [];
let scheduledTimeouts = [];
let isPlaying = false;

// --- Generate ---
if (generateMelodyBtn) {
    generateMelodyBtn.addEventListener('click', () => {
        const pParams = getPerlinParameters();
        const mParams = getMelodyParameters();

        if (typeof generateMelody === 'function') {
            try {
                currentMelody = generateMelody(pParams, mParams);
                const noteCount = currentMelody.filter(e => e.duration > 0).length;
                const restCount = currentMelody.length - noteCount;
                setStatus(`Melody generated: ${noteCount} notes, ${restCount} rests.`, 'ok');
                if (playMelodyBtn) playMelodyBtn.disabled = (!currentMelody || currentMelody.length === 0);
                if (exportMidiBtn) exportMidiBtn.disabled = (!currentMelody || currentMelody.length === 0);
            } catch (error) {
                console.error("Error generating melody:", error);
                setStatus('Error generating melody. Check console.', 'error');
                currentMelody = [];
                if (playMelodyBtn) playMelodyBtn.disabled = true;
                if (exportMidiBtn) exportMidiBtn.disabled = true;
            }
        } else {
            console.error("generateMelody function not found.");
            setStatus('Melody generation script not loaded correctly.', 'error');
            currentMelody = [];
            if (playMelodyBtn) playMelodyBtn.disabled = true;
            if (exportMidiBtn) exportMidiBtn.disabled = true;
        }
    });
}

// --- Audio ---
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

    scheduledOscillators.push(oscillator);

    const keyId = noteToKeyId[midiNoteNumber];
    if (keyId) {
        const keyElement = document.getElementById(keyId);
        if (keyElement) {
            const nowOffset = Math.max(0, (startTime - audioContext.currentTime) * 1000);
            const endOffset = Math.max(0, (startTime - audioContext.currentTime + duration) * 1000);
            const tOn = setTimeout(() => keyElement.classList.add('active'), nowOffset);
            const tOff = setTimeout(() => keyElement.classList.remove('active'), endOffset);
            scheduledTimeouts.push(tOn, tOff);
        }
    }
}

function stopPlayback() {
    for (const osc of scheduledOscillators) {
        try { osc.stop(0); } catch (_) {}
    }
    scheduledOscillators = [];

    for (const t of scheduledTimeouts) {
        clearTimeout(t);
    }
    scheduledTimeouts = [];

    document.querySelectorAll('.key.active').forEach(el => el.classList.remove('active'));

    isPlaying = false;
    if (playMelodyBtn) playMelodyBtn.disabled = false;
    if (generateMelodyBtn) generateMelodyBtn.disabled = false;
    if (stopMelodyBtn) stopMelodyBtn.disabled = true;
    if (exportMidiBtn) exportMidiBtn.disabled = (!currentMelody || currentMelody.length === 0);
}

// --- Play ---
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

        const beatDuration = 60 / getBpm();
        isPlaying = true;
        playMelodyBtn.disabled = true;
        if (generateMelodyBtn) generateMelodyBtn.disabled = true;
        if (stopMelodyBtn) stopMelodyBtn.disabled = false;
        if (exportMidiBtn) exportMidiBtn.disabled = true;
        setStatus('Playing…', 'ok');

        scheduledOscillators = [];
        scheduledTimeouts = [];

        const melodyStartTime = audioContext.currentTime + 0.1;
        let cursor = 0;

        for (let i = 0; i < currentMelody.length; i++) {
            const event = currentMelody[i];
            const dur = event.duration === 0 ? 1 : event.duration;
            const eventDuration = dur * beatDuration;

            if (event.duration > 0) {
                playNote(event.midi, melodyStartTime + cursor, eventDuration);
            }

            cursor += eventDuration;
        }

        const endTimeout = setTimeout(() => {
            if (isPlaying) {
                stopPlayback();
                setStatus('Playback finished.', 'ok');
            }
        }, (cursor + 0.5) * 1000);
        scheduledTimeouts.push(endTimeout);
    });
}

// --- Stop ---
if (stopMelodyBtn) {
    stopMelodyBtn.addEventListener('click', () => {
        stopPlayback();
        setStatus('Playback stopped.', 'ok');
    });
}

// --- MIDI Export ---
function generateMidiFile(noteEvents, bpm) {
    const TICKS_PER_QUARTER = 480;

    function writeVLQ(value) {
        const bytes = [];
        bytes.push(value & 0x7F);
        value >>= 7;
        while (value > 0) {
            bytes.push((value & 0x7F) | 0x80);
            value >>= 7;
        }
        bytes.reverse();
        return bytes;
    }

    function writeUint16(arr, value) {
        arr.push((value >> 8) & 0xFF, value & 0xFF);
    }

    function writeUint32(arr, value) {
        arr.push((value >> 24) & 0xFF, (value >> 16) & 0xFF, (value >> 8) & 0xFF, value & 0xFF);
    }

    function writeString(arr, str) {
        for (let i = 0; i < str.length; i++) arr.push(str.charCodeAt(i));
    }

    // Build track data
    const track = [];

    // Tempo meta event (delta=0): FF 51 03 tttttt
    const microsecondsPerBeat = Math.round(60000000 / bpm);
    track.push(0x00); // delta time
    track.push(0xFF, 0x51, 0x03);
    track.push((microsecondsPerBeat >> 16) & 0xFF);
    track.push((microsecondsPerBeat >> 8) & 0xFF);
    track.push(microsecondsPerBeat & 0xFF);

    // Note events
    const CHANNEL = 0;
    const VELOCITY = 100;

    for (const event of noteEvents) {
        const dur = event.duration === 0 ? 1 : event.duration;
        const ticks = dur * TICKS_PER_QUARTER;

        if (event.duration === 0) {
            // Rest: just advance time
            track.push(...writeVLQ(ticks));
            // Write a dummy meta event to carry the delta
            track.push(0xFF, 0x06, 0x00); // marker with empty text
            continue;
        }

        // Note On (delta=0 relative to previous event end)
        track.push(...writeVLQ(0));
        track.push(0x90 | CHANNEL, event.midi, VELOCITY);

        // Note Off after duration ticks
        track.push(...writeVLQ(ticks));
        track.push(0x80 | CHANNEL, event.midi, 0);
    }

    // End of track
    track.push(0x00, 0xFF, 0x2F, 0x00);

    // Assemble file
    const file = [];

    // Header: MThd
    writeString(file, 'MThd');
    writeUint32(file, 6);       // chunk length
    writeUint16(file, 0);       // format 0
    writeUint16(file, 1);       // 1 track
    writeUint16(file, TICKS_PER_QUARTER);

    // Track: MTrk
    writeString(file, 'MTrk');
    writeUint32(file, track.length);
    file.push(...track);

    return new Uint8Array(file);
}

if (exportMidiBtn) {
    exportMidiBtn.addEventListener('click', () => {
        if (!currentMelody || currentMelody.length === 0) {
            setStatus('No melody to export.', 'warn');
            return;
        }

        const midiData = generateMidiFile(currentMelody, getBpm());
        const blob = new Blob([midiData], { type: 'audio/midi' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        const seed = parseInt(perlinParamsControls.seed.value) || 0;
        a.href = url;
        a.download = `melody-seed${seed}.mid`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        setStatus('MIDI file exported.', 'ok');
    });
}
