
function isAudioPlaying(id) {
    const audio = document.getElementById(id);
    return !audio.paused;
}

function fadeOutAudio(id) {
    const audio = document.getElementById(id);
    const source = document.getElementById(id + '-src');
    const fadeAudio = setInterval(function () {
        
        if (audio.volume > 0.0) {
            audio.volume -= 0.1;
        }
        
        // When volume at zero stop stop
        if (audio.volume === 0.0) {
            clearInterval(fadeAudio);
            audio.pause();
            source.src = "";
        }
    }, 50);
}

function fadeInAudio(id, path, duration) {

    const audio = new Audio('https://interactive-examples.mdn.mozilla.net/media/cc0-audio/t-rex-roar.mp3');
    audio.play();
    
    //
    // const audio = document.getElementById(id);
    // const source = document.getElementById(id + '-src');
    // audio.volume = 0.0;
    // source.src = source;
    // audio.load();
    // audio.play();
    //
    // const fadeAudio = setInterval(function () {
    //
    //     if (audio.volume < 0.9) {
    //         audio.volume += 0.1;
    //     }
    //
    //     // When volume at zero stop stop
    //     if (audio.volume === 1.0) {
    //         clearInterval(fadeAudio);
    //         audio.pause();
    //     }
    // }, 20);
}
