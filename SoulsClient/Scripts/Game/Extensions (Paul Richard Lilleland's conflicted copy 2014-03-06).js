
PIXIE = {};
PIXIE.Intersects = function (r1, r2, callback) {

    if ((r1.x + r1.width > r2.x) && (r1.x < r2.x + r2.width) && ((r1.y + r1.height > r2.y) && (r1.y < r2.y + r2.height))) {
        callback(false);
    } else {
        callback(false);
    }
}

/// This function requires   obj.x obj.y obj.width, obj.height .. if custom is needed use data 
PIXIE.ScaleGraphics = function (scaleX, scaleY, obj, data) {



    if (scaleX == 1) {
        obj.position.x += (((obj.scale.x - 1) / 2) * obj.width);
        obj.position.y += (((obj.scale.y - 1) / 2) * obj.height);
        obj.scale.x = scaleX;
        obj.scale.y = scaleY;

        return;
    }

    obj.scale.x = scaleX;
    obj.scale.y = scaleY;
    obj.position.x -= (((obj.scale.x - 1) / 2) * obj.width);
    obj.position.y -= (((obj.scale.y - 1) / 2) * obj.height);



}

