class PAP {

    constructor(isMobil, config, obj, x, y, isTop) {
        this.x = x;
        this.y = y;
        this.isMobil = isMobil;
        this.isTop = isTop;

        this.name = obj['Title'];
        this.image = obj['Image'];
        this.url = '/pap/' + obj['Id'];

        this.config = config;
    }

    get frame() {

        var x = this.x - (this.config.imgWidth / 2);
        var y = this.y - this.config.dot;
        var height = (2 * this.config.dot) + this.config.spacing + this.config.fontSize + this.config.spacing + this.config.imgHeight
        var width = this.config.imgWidth;

        if (this.isMobil) {

            height = (5 * this.config.spacing) + this.config.fontSize + this.config.imgHeight;
            width = window.innerWidth;
            y = this.y - this.config.imgHeight;

            if (!this.isTop) {
                x = 2 * this.config.spacing;
                y = this.y - this.config.imgHeight;
            }

            height = (5 * this.config.spacing) + this.config.fontSize + this.config.imgHeight;
        }

        return {
            x,
            y,
            width,
            height
        };
    }

    //drawFrame(ctx) {
    //    ctx.fillStyle = "#FF0000";
    //    ctx.fillRect(this.frame.x, this.frame.y, this.frame.width, this.frame.height);
    //}

    drawDot(ctx, isHover) {

        if (isHover) {
            ctx.shadowColor = 'white';
            ctx.shadowBlur = 20;
        } else {
            ctx.shadowBlur = 0;
        }

        ctx.fillStyle = 'rgba(255, 255, 255, 1)';
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.config.dot, 0, 2 * Math.PI);
        ctx.fill();

        ctx.fillStyle = isHover ? this.config.primary : this.config.surface;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.config.dot / 1.4, 0, 2 * Math.PI);
        ctx.fill();
    }

    drawInfo(ctx) {
        ctx.font = this.config.fontSize + "px Comic Sans MS";
        ctx.fillStyle = "white";
        ctx.textAlign = "center";

        if (this.isMobil) {
            var x = this.isTop ? (this.x * 2.5) + this.config.spacing : (this.x / 2.5) + this.config.spacing;
            ctx.fillText(this.name, x, this.y - (this.config.imgHeight / 2));
        }
        else
            ctx.fillText(this.name, this.x, (this.y + (2 * this.config.dot) + this.config.spacing));
    }

    drawImage(ctx) {
        var drawCanvasImage = function (ctx, img, x, y) {
            return function () {
                ctx.drawImage(img, x, y, 200, 100);
            }
        }
        var img = new Image;
        img.src = this.image;
        if (this.isMobil) {
            var x = this.isTop ? this.x + (2 * this.config.dot) + this.config.spacing : (2 * this.config.spacing);
            img.onload = drawCanvasImage(ctx, img, x, this.y - (this.config.imgHeight / 4));
        }
        else
            img.onload = drawCanvasImage(ctx, img, this.x - (this.config.imgWidth / 2), (this.y + this.config.dot + this.config.fontSize + (2 * this.config.spacing)));
    }
}