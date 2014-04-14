
PIXIE = {};
PIXIE.Intersects = function (r1, r2) {

    if ((r1.x + r1.width > r2.x) && (r1.x < r2.x + r2.width) && ((r1.y + r1.height > r2.y) && (r1.y < r2.y + r2.height))) {
        return true;
    } else {
        return false;
    }
}

/// This function requires   obj.x obj.y obj.width, obj.height .. if custom is needed use data 
PIXIE.ScaleGraphics = function (scaleX, scaleY, obj) {

	deltaX = (((scaleX - 1) / 2) * obj.width);
	deltaY = (((scaleY - 1) / 2) * obj.height);

    obj.position.x -= (scaleX != 1) ? deltaX : ((obj.scale.x - 1) / 2) * obj.width*(-1);
	obj.position.y -= (scaleY != 1) ? deltaY : ((obj.scale.y - 1) / 2) * obj.height*(-1);
	
    obj.scale.x = scaleX;
    obj.scale.y = scaleY;
}


