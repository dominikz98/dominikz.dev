
function stopAudio(id) {
    const audio = document.getElementById(id);
    audio.pause();
}

function setAudioSource(id, path) {
    const audio = document.getElementById(id);
    const source = document.getElementById(id + '-src');
    source.src = path;
    audio.load();
}
