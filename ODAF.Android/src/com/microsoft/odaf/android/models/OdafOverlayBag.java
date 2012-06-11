package com.microsoft.odaf.android.models;

import java.util.ArrayList;

public class OdafOverlayBag {
	
	private static OdafOverlayBag _instance;
	private ArrayList<OdafOverlay> overlays;
	
	public static OdafOverlayBag getInstance(){
		if(_instance == null){
			_instance = new OdafOverlayBag();
			_instance.overlays = new ArrayList<OdafOverlay>();
		}
		return _instance;
	}
	
	private OdafOverlayBag(){
		
	}

	public ArrayList<OdafOverlay> getOverlays() {
		return overlays;
	}

	public void setOverlays(ArrayList<OdafOverlay> overlays) {
		this.overlays = overlays;
	}
	
	
}
