package com.microsoft.odaf.android.models;

import java.io.Serializable;

import android.graphics.RectF;

import com.google.android.maps.GeoPoint;

public class OdafPoint implements Serializable {
	
	private static final long serialVersionUID = 1L;
	
	private GeoPoint geoPoint;
	private String guid;
	private String description;
	private String layerId;
	private RectF bounds;
	
	public OdafPoint(GeoPoint geoPoint, String guid, String description, String layerId) {
		this.geoPoint = geoPoint;
		this.guid = guid;
		this.description = description;
		this.layerId = layerId;
	}
	
	public GeoPoint getGeoPoint() {
		return geoPoint;
	}
	public void setGeoPoint(GeoPoint geoPoint) {
		this.geoPoint = geoPoint;
	}
	public String getGuid() {
		return guid;
	}
	public void setGuid(String guid) {
		this.guid = guid;
	}
	public String getDescription() {
		return description;
	}
	public void setDescription(String description) {
		this.description = description;
	}
	public String getLayerId() {
		return layerId;
	}
	public void setLayerId(String layerId) {
		this.layerId = layerId;
	}
	public RectF getBounds() {
		return bounds;
	}
	public void setBounds(RectF bounds) {
		this.bounds = bounds;
	}	
}
