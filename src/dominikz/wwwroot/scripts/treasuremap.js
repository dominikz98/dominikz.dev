function drawTreasureMap(papsJson, configJson) {

    var isMobil = (window.innerWidth - 24) < 769;
    var minWidth = isMobil ? 0 : JSON.parse(papsJson).length * 325
    var minHeight = !isMobil ? 0 : JSON.parse(papsJson).length * 325

    var cWidth = Math.max((window.innerWidth - 24), minWidth);
    var cHeight = Math.max((window.innerHeight - 75), minHeight);

    // create canvas
    var tm_scene = document.getElementById("tm_scene");
    tm_scene.width = cWidth;
    tm_scene.height = cHeight;
    var ctx_scene = tm_scene.getContext('2d');

    // Draw sine wave
    var peeks = drawSineWave(ctx_scene, tm_scene.width, tm_scene.height, isMobil);

    // Parse blazor passed entities
    var config = new Config(JSON.parse(configJson));
    var paps = castJson(isMobil, config, papsJson, peeks);

    // draw map points
    for (var i = 0; i < paps.length; i++) {

        var pap = paps[i];
        pap.drawDot(ctx_scene, false);
        pap.drawInfo(ctx_scene);
        pap.drawImage(ctx_scene);
    }

    bindEvents(paps, cWidth, cHeight);
}

function drawSineWave(ctx, width, height, isMobil) {

    var d = isMobil ? width / 2 : height / 2;

    ctx.beginPath();

    if (isMobil)
        ctx.moveTo(d, 0);
    else
        ctx.moveTo(0, d);

    ctx.strokeStyle = 'rgba(255, 255, 255, 0.85)';
    ctx.lineWidth = 10;
    ctx.setLineDash([20]);

    var peeks = [];
    var peekReached = false;
    for (var i = 0; i < (isMobil ? height : width); i++) {

        var y = d + Math.sin(i * 0.01) * 100;
        var roundedY = Math.round(y * 100) / 100;

        if (isMobil)
            ctx.lineTo(y, i);
        else
            ctx.lineTo(i, y);

        if (!peekReached && (roundedY === (d - 100) || roundedY === (d + 100))) {
            peekReached = true;
            var x = isMobil ? roundedY : i;
            var y = isMobil ? i : roundedY;
            var top = roundedY === (d - 100);
            peeks.push({ x, y, top });
        }
        else if (peekReached) {
            peekReached = false;
        }
    }

    ctx.stroke();
    return peeks;
}

function castJson(isMobil, config, json, peeks) {

    var paps = [];
    var entries = JSON.parse(json);

    for (var i = 0; i < entries.length; i++) {
        paps.push(new PAP(isMobil, config, entries[i], peeks[i].x, peeks[i].y, peeks[i].top));
    }

    return paps;
}

function bindEvents(paps, cWidth, cHeight) {
    var tm_selection = document.getElementById("tm_selection");
    tm_selection.width = cWidth;
    tm_selection.height = cHeight;
    var ctx_selection = tm_selection.getContext('2d');

    tm_selection.addEventListener('mousemove', (e) => onMouseMove(tm_selection, ctx_selection, e, paps), false);
    tm_selection.addEventListener('mousedown', (e) => onMouseDown(e, paps), false);
}

function onMouseMove(canvas, ctx, e, paps) {

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    for (var i = 0; i < paps.length; i++) {
        var pap = paps[i];
        paps[i].drawDot(ctx, isInSection(e, pap));
    }
}

function onMouseDown(e, paps) {
    if (e.button !== 0)
        return;

    for (var i = 0; i < paps.length; i++) {
        var pap = paps[i];
        if (isInSection(e, pap)) {
            window.location.replace(pap.url);
        }
    }
}

function isInSection(e, pap) {

    var mouseX, mouseY;
    if (e.offsetX) {
        mouseX = e.offsetX;
        mouseY = e.offsetY;
    }
    else if (e.layerX) {
        mouseX = e.layerX;
        mouseY = e.layerY;
    }

    return (mouseX >= pap.frame.x)
        & (mouseX <= pap.frame.x + pap.frame.width)
        & (mouseY >= pap.frame.y)
        & (mouseY <= pap.frame.y + pap.frame.height);
}
