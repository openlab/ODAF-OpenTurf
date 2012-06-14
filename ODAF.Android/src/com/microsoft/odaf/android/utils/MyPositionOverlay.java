package com.microsoft.odaf.android.utils;

import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Point;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;

public class MyPositionOverlay extends Overlay {
	
	private Bitmap bitmap;
	private GeoPoint geoPoint;
	
	public MyPositionOverlay(GeoPoint geoPoint, Bitmap bitmap){
		this.bitmap = bitmap;
		this.geoPoint = geoPoint;
	}
	
	@Override
	public void draw(Canvas canvas, MapView mapView, boolean shadow) {
		Point screenPoint = new Point();
		mapView.getProjection().toPixels(geoPoint, screenPoint);	
		canvas.drawBitmap(bitmap, screenPoint.x - bitmap.getWidth()/2, screenPoint.y - bitmap.getHeight()/2, null);
	}
}
