
function stopAudio(id) {
    var audio = document.getElementById(id);
    audio.pause();
}

function setAudioSource(id, path) {

    var audio = document.getElementById(id);
    var source = document.getElementById(id + '-src');
    source.src = path;
    audio.load();
}
