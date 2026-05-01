// DOM Elements
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
const playbackParamsControls = { bpm: document.getElementById('p-bpm') };

// History
const HISTORY_KEY = 'melody-generator-history';
const historyList = document.getElementById('history-list');
const historyEmpty = document.getElementById('history-empty');
const clearHistoryBtn = document.getElementById('clear-history-btn');

const generateMelodyBtn = document.getElementById('generate-melody-btn');
const randomBtn = document.getElementById('random-btn');
const exportMidiBtn = document.getElementById('export-midi-btn');
const playBtn = document.getElementById('play-btn');
const stopBtn = document.getElementById('stop-btn');
const iconPlay = document.getElementById('icon-play');
const iconPause = document.getElementById('icon-pause');
const playerFill = document.getElementById('player-fill');
const playerTime = document.getElementById('player-time');
const statusMessage = document.getElementById('status-message');

function setStatus(text, kind) {
    if (!statusMessage) return;
    statusMessage.textContent = text || '';
    statusMessage.className = 'status-message' + (kind ? ' status-' + kind : '');
}

if (perlinParamsControls.persistance && perlinParamsControls.persistanceVal) {
    perlinParamsControls.persistance.addEventListener('input', () => {
        perlinParamsControls.persistanceVal.textContent = perlinParamsControls.persistance.value;
    });
}

// Scales
const SCALES = {
    'chromatic': null, 'major': [0,2,4,5,7,9,11], 'minor': [0,2,3,5,7,8,10],
    'pentatonic-major': [0,2,4,7,9], 'pentatonic-minor': [0,3,5,7,10], 'blues': [0,3,5,6,7,10]
};
const SCALE_KEYS = Object.keys(SCALES);
const SCALE_LABELS = {'chromatic':'Chromatic','major':'Major','minor':'Minor','pentatonic-major':'Pent. Maj','pentatonic-minor':'Pent. Min','blues':'Blues'};

// Piano
const PIANO_START_MIDI = 36;
const PIANO_END_MIDI = 83;
const NOTE_NAMES = ['C','C#','D','D#','E','F','F#','G','G#','A','A#','B'];
const noteToKeyId = {};

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

// Parameter getters
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
    return { tone: parseInt(melodyParamsControls.tone.value), octave: parseInt(melodyParamsControls.octave.value), scale: SCALES[scaleKey] || null };
}
function getBpm() {
    const raw = parseInt(playbackParamsControls.bpm.value);
    return (isNaN(raw) || raw <= 0) ? 120 : raw;
}
function getScaleKey() {
    return melodyParamsControls.scale ? melodyParamsControls.scale.value : 'chromatic';
}

// State
let currentMelody = [];
let scheduledOscillators = [];
let scheduledTimeouts = [];
let isPlaying = false;
let progressRAF = null;
let playbackStartRealTime = 0;
let playbackTotalDuration = 0;

// --- Player UI ---
function formatTime(s) {
    const m = Math.floor(s / 60);
    return m + ':' + String(Math.floor(s % 60)).padStart(2, '0');
}

function computeMelodyDuration(melody, bpm) {
    const beat = 60 / bpm;
    let total = 0;
    for (const ev of melody) {
        total += (ev.duration === 0 ? 1 : ev.duration) * beat;
    }
    return total;
}

function updatePlayerTime() {
    const dur = currentMelody.length > 0 ? computeMelodyDuration(currentMelody, getBpm()) : 0;
    playerTime.textContent = '0:00 / ' + formatTime(dur);
    playerFill.style.width = '0%';
}

function tickProgress() {
    if (!isPlaying) return;
    const elapsed = (performance.now() - playbackStartRealTime) / 1000;
    const pct = Math.min(elapsed / playbackTotalDuration, 1);
    playerFill.style.width = (pct * 100) + '%';
    playerTime.textContent = formatTime(elapsed) + ' / ' + formatTime(playbackTotalDuration);
    if (pct < 1) {
        progressRAF = requestAnimationFrame(tickProgress);
    }
}

function setPlayingUI() {
    isPlaying = true;
    iconPlay.style.display = 'none';
    iconPause.style.display = '';
    playBtn.classList.add('playing');
    if (generateMelodyBtn) generateMelodyBtn.disabled = true;
    if (randomBtn) randomBtn.disabled = true;
    if (exportMidiBtn) exportMidiBtn.disabled = true;
}

function setStoppedUI() {
    isPlaying = false;
    if (progressRAF) cancelAnimationFrame(progressRAF);
    progressRAF = null;
    iconPlay.style.display = '';
    iconPause.style.display = 'none';
    playBtn.classList.remove('playing');
    if (generateMelodyBtn) generateMelodyBtn.disabled = false;
    if (randomBtn) randomBtn.disabled = false;
    if (exportMidiBtn) exportMidiBtn.disabled = (!currentMelody || currentMelody.length === 0);
}

// --- Audio ---
let audioContext;
function initAudioContext() {
    if (!audioContext) audioContext = new (window.AudioContext || window.webkitAudioContext)();
}

function midiToFreq(n) { return 440 * Math.pow(2, (n - 69) / 12); }

function playNote(midiNoteNumber, startTime, duration) {
    if (!audioContext) return;
    const osc = audioContext.createOscillator();
    const gain = audioContext.createGain();
    osc.connect(gain);
    gain.connect(audioContext.destination);
    osc.type = 'sine';
    osc.frequency.setValueAtTime(midiToFreq(midiNoteNumber), startTime);

    const attack = 0.05, decay = 0.1, sustain = 0.7;
    const release = Math.min(0.2, duration * 0.4);
    const play = Math.max(duration - release, 0.01);

    gain.gain.setValueAtTime(0, startTime);
    gain.gain.linearRampToValueAtTime(1.0, startTime + attack);
    gain.gain.linearRampToValueAtTime(sustain, startTime + attack + decay);
    gain.gain.setValueAtTime(sustain, startTime + play);
    gain.gain.linearRampToValueAtTime(0, startTime + play + release);
    osc.start(startTime);
    osc.stop(startTime + duration + 0.1);
    scheduledOscillators.push(osc);

    const keyId = noteToKeyId[midiNoteNumber];
    if (keyId) {
        const el = document.getElementById(keyId);
        if (el) {
            const nowOff = Math.max(0, (startTime - audioContext.currentTime) * 1000);
            const endOff = Math.max(0, (startTime - audioContext.currentTime + duration) * 1000);
            scheduledTimeouts.push(setTimeout(() => el.classList.add('active'), nowOff));
            scheduledTimeouts.push(setTimeout(() => el.classList.remove('active'), endOff));
        }
    }
}

function stopPlayback(resetProgress) {
    for (const o of scheduledOscillators) { try { o.stop(0); } catch(_) {} }
    scheduledOscillators = [];
    for (const t of scheduledTimeouts) clearTimeout(t);
    scheduledTimeouts = [];
    document.querySelectorAll('.key.active').forEach(el => el.classList.remove('active'));
    setStoppedUI();
    if (resetProgress) {
        updatePlayerTime();
    }
}

function startPlayback() {
    initAudioContext();
    if (!currentMelody || currentMelody.length === 0) {
        doGenerate();
        if (!currentMelody || currentMelody.length === 0) return;
    }

    const beatDuration = 60 / getBpm();
    setPlayingUI();
    setStatus('Playing...', 'ok');
    scheduledOscillators = [];
    scheduledTimeouts = [];

    const melodyStart = audioContext.currentTime + 0.1;
    let cursor = 0;
    for (const event of currentMelody) {
        const dur = event.duration === 0 ? 1 : event.duration;
        const eventDuration = dur * beatDuration;
        if (event.duration > 0) playNote(event.midi, melodyStart + cursor, eventDuration);
        cursor += eventDuration;
    }

    playbackTotalDuration = cursor;
    playbackStartRealTime = performance.now() + 100;
    progressRAF = requestAnimationFrame(tickProgress);

    scheduledTimeouts.push(setTimeout(() => {
        if (isPlaying) {
            stopPlayback(false);
            playerFill.style.width = '100%';
            playerTime.textContent = formatTime(playbackTotalDuration) + ' / ' + formatTime(playbackTotalDuration);
            setStatus('Playback finished.', 'ok');
        }
    }, (cursor + 0.5) * 1000));
}

// --- Player buttons ---
playBtn.addEventListener('click', () => {
    if (isPlaying) {
        stopPlayback(true);
        setStatus('Stopped.', 'ok');
    } else {
        startPlayback();
    }
});

stopBtn.addEventListener('click', () => {
    stopPlayback(true);
    setStatus('Stopped.', 'ok');
});

// --- Randomize ---
randomBtn.addEventListener('click', () => {
    perlinParamsControls.seed.value = Math.floor(Math.random() * 10000);
    perlinParamsControls.length.value = Math.floor(Math.random() * 28) + 4;
    perlinParamsControls.octaves.value = Math.floor(Math.random() * 6) + 1;
    const pers = (Math.random() * 0.8 + 0.1).toFixed(2);
    perlinParamsControls.persistance.value = pers;
    perlinParamsControls.persistanceVal.textContent = pers;
    perlinParamsControls.lacunarity.value = (Math.random() * 3 + 1).toFixed(1);
    const octave = Math.floor(Math.random() * 4) + 2; // 2-5
    const tone = Math.floor(Math.random() * 12); // 0-11
    const baseMidi = octave * 12 + tone;
    const maxRange = Math.min(42, PIANO_END_MIDI - baseMidi);
    perlinParamsControls.range.value = Math.max(6, Math.floor(Math.random() * maxRange) + 1);
    melodyParamsControls.octave.value = octave;
    melodyParamsControls.tone.value = tone;
    playbackParamsControls.bpm.value = Math.floor(Math.random() * 160) + 60;
    melodyParamsControls.scale.value = SCALE_KEYS[Math.floor(Math.random() * SCALE_KEYS.length)];
    doGenerate();
});

// --- Generate ---
function doGenerate() {
    const pParams = getPerlinParameters();
    const mParams = getMelodyParameters();
    if (typeof generateMelody !== 'function') {
        setStatus('Melody script not loaded.', 'error');
        return;
    }
    try {
        currentMelody = generateMelody(pParams, mParams);
        const notes = currentMelody.filter(e => e.duration > 0).length;
        const rests = currentMelody.length - notes;
        setStatus(`${notes} notes, ${rests} rests`, 'ok');
        if (exportMidiBtn) exportMidiBtn.disabled = false;
        updatePlayerTime();
        saveToHistory(pParams, mParams);
    } catch (error) {
        console.error("Error generating melody:", error);
        setStatus('Error generating melody.', 'error');
        currentMelody = [];
        if (exportMidiBtn) exportMidiBtn.disabled = true;
    }
}

generateMelodyBtn.addEventListener('click', doGenerate);

// Auto-generate default melody on load (silently catch cache/load issues)
try { doGenerate(); } catch (_) {}

// --- MIDI Export ---
function generateMidiFile(noteEvents, bpm) {
    const TPQ = 480;
    function vlq(v) { const b = [v & 0x7F]; v >>= 7; while (v > 0) { b.push((v & 0x7F)|0x80); v >>= 7; } b.reverse(); return b; }
    function u16(a,v) { a.push((v>>8)&0xFF, v&0xFF); }
    function u32(a,v) { a.push((v>>24)&0xFF,(v>>16)&0xFF,(v>>8)&0xFF,v&0xFF); }
    function str(a,s) { for (let i=0;i<s.length;i++) a.push(s.charCodeAt(i)); }

    const track = [];
    const uspb = Math.round(60000000/bpm);
    track.push(0x00, 0xFF, 0x51, 0x03, (uspb>>16)&0xFF, (uspb>>8)&0xFF, uspb&0xFF);

    for (const ev of noteEvents) {
        const dur = ev.duration === 0 ? 1 : ev.duration;
        const ticks = dur * TPQ;
        if (ev.duration === 0) { track.push(...vlq(ticks), 0xFF, 0x06, 0x00); continue; }
        track.push(...vlq(0), 0x90, ev.midi, 100);
        track.push(...vlq(ticks), 0x80, ev.midi, 0);
    }
    track.push(0x00, 0xFF, 0x2F, 0x00);

    const file = [];
    str(file,'MThd'); u32(file,6); u16(file,0); u16(file,1); u16(file,TPQ);
    str(file,'MTrk'); u32(file,track.length); file.push(...track);
    return new Uint8Array(file);
}

exportMidiBtn.addEventListener('click', () => {
    if (!currentMelody || currentMelody.length === 0) { setStatus('No melody to export.','warn'); return; }
    const data = generateMidiFile(currentMelody, getBpm());
    const blob = new Blob([data], { type: 'audio/midi' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `melody-seed${parseInt(perlinParamsControls.seed.value)||0}.mid`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    setStatus('MIDI exported.', 'ok');
});

// --- History ---
function loadHistory() { try { return JSON.parse(localStorage.getItem(HISTORY_KEY)) || []; } catch { return []; } }
function persistHistory(h) { localStorage.setItem(HISTORY_KEY, JSON.stringify(h)); }

function saveToHistory(pParams, mParams) {
    const history = loadHistory();
    const count = history.length + 1;
    const now = new Date();
    const pad = n => String(n).padStart(2, '0');
    const defaultName = `Melody #${count} — ${pad(now.getDate())}/${pad(now.getMonth()+1)} ${pad(now.getHours())}:${pad(now.getMinutes())}`;
    history.unshift({
        id: Date.now(), name: defaultName, timestamp: now.toISOString(),
        bpm: getBpm(), scale: getScaleKey(),
        perlin: { seed: pParams.seed, length: pParams.length, octaves: pParams.octaves, persistance: pParams.persistance, lacunarity: pParams.lacunarity, range: pParams.range },
        melody: { tone: mParams.tone, octave: mParams.octave }
    });
    if (history.length > 50) history.length = 50;
    persistHistory(history);
    renderHistory();
}

function deleteFromHistory(id) { persistHistory(loadHistory().filter(h => h.id !== id)); renderHistory(); }
function renameInHistory(id, name) { const h = loadHistory(); const i = h.find(x => x.id === id); if (i) i.name = name; persistHistory(h); }

function loadFromHistory(entry) {
    perlinParamsControls.seed.value = entry.perlin.seed;
    perlinParamsControls.length.value = entry.perlin.length;
    perlinParamsControls.octaves.value = entry.perlin.octaves;
    perlinParamsControls.persistance.value = entry.perlin.persistance;
    perlinParamsControls.persistanceVal.textContent = entry.perlin.persistance;
    perlinParamsControls.lacunarity.value = entry.perlin.lacunarity;
    perlinParamsControls.range.value = entry.perlin.range;
    melodyParamsControls.tone.value = entry.melody.tone;
    melodyParamsControls.octave.value = entry.melody.octave;
    melodyParamsControls.scale.value = entry.scale;
    playbackParamsControls.bpm.value = entry.bpm;
    setStatus('Parameters loaded.', 'ok');
}

function fmtDate(iso) {
    const d = new Date(iso);
    const p = n => String(n).padStart(2,'0');
    return p(d.getDate())+'/'+p(d.getMonth()+1)+' '+p(d.getHours())+':'+p(d.getMinutes());
}
function renderHistory() {
    const history = loadHistory();
    if (!historyList) return;
    if (history.length === 0) { historyList.innerHTML = ''; if (historyEmpty) historyEmpty.style.display = ''; return; }
    if (historyEmpty) historyEmpty.style.display = 'none';

    historyList.innerHTML = history.map(h => `
        <div class="history-item" data-id="${h.id}">
            <div class="history-item-main">
                <input class="history-name" type="text" value="${(h.name||'').replace(/"/g,'&quot;')}" placeholder="Untitled" data-id="${h.id}">
                <span class="history-meta">${fmtDate(h.timestamp)} &middot; ${SCALE_LABELS[h.scale]||h.scale} &middot; ${h.bpm} bpm &middot; seed ${h.perlin.seed} &middot; ${h.perlin.length} notes</span>
            </div>
            <div class="history-actions">
                <button class="btn btn-ghost btn-sm history-load" data-id="${h.id}">Load</button>
                <button class="btn btn-ghost btn-sm history-delete" data-id="${h.id}">Delete</button>
            </div>
        </div>
    `).join('');

    historyList.querySelectorAll('.history-load').forEach(b => b.addEventListener('click', () => { const e = history.find(h => h.id === Number(b.dataset.id)); if (e) loadFromHistory(e); }));
    historyList.querySelectorAll('.history-delete').forEach(b => b.addEventListener('click', () => deleteFromHistory(Number(b.dataset.id))));
    historyList.querySelectorAll('.history-name').forEach(inp => inp.addEventListener('change', () => renameInHistory(Number(inp.dataset.id), inp.value)));
}

clearHistoryBtn.addEventListener('click', () => { localStorage.removeItem(HISTORY_KEY); renderHistory(); setStatus('History cleared.', 'ok'); });
renderHistory();
