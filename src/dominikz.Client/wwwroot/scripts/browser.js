function getUserAgent() {
    return navigator.userAgent;
}

function startMuteVideo(videoId) {
    const video = document.getElementById(videoId);
    video.addEventListener("canplay", function () {
        video.play();
        video.volume = 0.0;
        video.muted = true;
        console.log('trailer playing')
    });
}

function toggleFullscreen(videoId) {
    const video = document.getElementById(videoId);
    if (video.requestFullscreen) {
        video.requestFullscreen();
    } else if (video.webkitRequestFullscreen) { /* Safari */
        video.webkitRequestFullscreen();
    } else if (video.msRequestFullscreen) { /* IE11 */
        video.msRequestFullscreen();
    }
}