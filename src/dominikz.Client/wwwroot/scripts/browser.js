function getUserAgent() {
    return navigator.userAgent;
}

function attachMouseMoveHandler(containerId, dotNetObjRef) {
    var container = document.getElementById(containerId);
    if (container) {
        container.addEventListener('mousemove', function (event) {
            var rect = container.getBoundingClientRect();
            var x = event.clientX - rect.left;
            var y = event.clientY - rect.top;
            dotNetObjRef.invokeMethodAsync('HandleMouseMove', x, y);
        });
    }
}

function attachMouseOutHandler(containerId, dotNetObjRef) {
    var container = document.getElementById(containerId);
    if (container) {
        container.addEventListener('mouseout', function () {
            dotNetObjRef.invokeMethodAsync('HandleMouseOut');
        });
    }
}