package com.microsoft.odaf.android.activities;

import java.util.ArrayList;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.microsoft.odaf.android.R;
import com.microsoft.odaf.android.models.OdafOverlay;
import com.microsoft.odaf.android.models.OdafOverlayBag;
import com.microsoft.odaf.android.models.OdafPoint;
import com.microsoft.odaf.android.utils.MyPositionOverlay;

import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import android.location.Location;
import android.location.LocationManager;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Toast;

public class MainMapActivity extends MapActivity {
	private static final int SHOW_COM = 0;
	private static final int SHOW_DATASOURCES = 1;
	private static final int SHOW_DETAILS = 2;
	
	private Intent launchChooseDataSourcesActivity;
	private Intent launchShowComsActivity;
	private Intent launchShowDetailsActivity;
	
	private MapController mapController;
	private MapView mapView;
	private Button selectDataSourcesButton;
	private Button gpsButton;
	private Button dezoomButton;
	
	
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);       
        initializeMapView();
        
        launchChooseDataSourcesActivity = new Intent(this, ChooseDataSourcesActivity.class);
        launchShowComsActivity = new Intent(this, ShowComsActivity.class);
        launchShowDetailsActivity = new Intent(this, ShowDetailsActivity.class);
        
        gpsButton = (Button) findViewById(R.id.gpsButton);
        dezoomButton = (Button) findViewById(R.id.dezoomButton);
        selectDataSourcesButton = (Button) findViewById(R.id.selectDataSourcesButton);
        
        gpsButton.setOnClickListener(new OnClickListener() {			
			@Override
			public void onClick(View arg0) {
				String serviceString = Context.LOCATION_SERVICE;
				LocationManager locationManager;
				locationManager = (LocationManager)getSystemService(serviceString);
				String provider = LocationManager.GPS_PROVIDER;
				
				Resources resources = getResources();
				Bitmap bitmap = BitmapFactory.decodeResource(resources, R.drawable.me);
				
				Location location = locationManager.getLastKnownLocation(provider);
				int latitude = (int) (location.getLatitude() * 1e6);
				int longitude = (int) (location.getLongitude() * 1e6);	
				GeoPoint geoPoint = new GeoPoint(latitude, longitude);
				
				int size = mapView.getOverlays().size();
				int myPositionOverlayIndex = -1;
				for(int i = 0; i < size; i++){
					if (mapView.getOverlays().get(i).getClass().getName() == MyPositionOverlay.class.getName())
					{
						myPositionOverlayIndex = i;
					}
				}
				if(myPositionOverlayIndex != -1){
					mapView.getOverlays().remove(myPositionOverlayIndex);
				}
				mapView.getOverlays().add(new MyPositionOverlay(geoPoint, bitmap));
			}
		});
        
        dezoomButton.setOnClickListener(new OnClickListener() {			
			@Override
			public void onClick(View arg0) {
				Double latitude = Double.valueOf(getString(R.string.latitude));
		        Double longitude = Double.valueOf(getString(R.string.longitude));
		        GeoPoint point = new GeoPoint((int)(latitude * 1e6), (int)(longitude * 1e6));
		        mapController.setCenter(point);
		        mapController.setZoom( Integer.valueOf(getString(R.string.zoomLevel)));
			}
		});
        
		selectDataSourcesButton.setOnClickListener(new OnClickListener() {			
			@Override
			public void onClick(View arg0) {
				startActivityForResult(launchChooseDataSourcesActivity, SHOW_DATASOURCES);
			}
		});
    }
    
    private void initializeMapView(){
    	
    	mapView = (MapView) findViewById(R.id.mapview);
        mapView.setSatellite(true);
        mapView.setStreetView(true);
        mapView.setTraffic(true);
        mapView.setBuiltInZoomControls(true);
        
        mapController = mapView.getController();
        
        Double latitude = Double.valueOf(getString(R.string.latitude));
        Double longitude = Double.valueOf(getString(R.string.longitude));
        GeoPoint point = new GeoPoint((int)(latitude * 1e6), (int)(longitude * 1e6));
        mapController.setCenter(point);
        mapController.setZoom( Integer.valueOf(getString(R.string.zoomLevel)));       
    }

	@Override
	protected boolean isRouteDisplayed() {
		return false;
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		
		if(requestCode == SHOW_DATASOURCES){
			mapView.getOverlays().clear();
			OdafOverlayBag bag = OdafOverlayBag.getInstance();
			ArrayList<OdafOverlay> overlays = bag.getOverlays();
			for(OdafOverlay overlay : overlays){
				if(overlay.isSelected()){
					overlay.setContext(this);
					mapView.getOverlays().add(overlay);
				}
			}
		}	
		if(requestCode == SHOW_DETAILS){
			if(resultCode == RESULT_OK){
				String guid = data.getStringExtra("guid");
				double latitude = data.getDoubleExtra("latitude", 0);
				double longitude = data.getDoubleExtra("longitude", 0);
				String name = data.getStringExtra("name");
				String layerId = data.getStringExtra("layerId");
				ShowCommentaires(guid, latitude, longitude, name, layerId);
			}
			if(resultCode == RESULT_CANCELED){
				Toast t = Toast.makeText(this, "SHOW_DETAILS + RESULT_CANCELED", Toast.LENGTH_LONG);
				t.show();
			}		
		}
		if(requestCode == SHOW_COM){
			if(resultCode == RESULT_OK){
				Toast toast = Toast.makeText(this, getString(R.string.twitterCommentSent), Toast.LENGTH_LONG);
				toast.show();
				}
			}
	}
	
	public void ShowDetails(OdafPoint odafPoint)
	{
		launchShowDetailsActivity.putExtra("description", odafPoint.getDescription());
		launchShowDetailsActivity.putExtra("guid", odafPoint.getGuid());
		launchShowDetailsActivity.putExtra("latitude", odafPoint.getGeoPoint().getLatitudeE6());
		launchShowDetailsActivity.putExtra("longitude", odafPoint.getGeoPoint().getLongitudeE6());
		launchShowDetailsActivity.putExtra("layerId",odafPoint.getLayerId());
		startActivityForResult(launchShowDetailsActivity, SHOW_DETAILS);
	}
	
	public void ShowCommentaires(String guid, double latitude, double longitude, String name, String layerId){
		launchShowComsActivity.putExtra("guid", guid);
		launchShowComsActivity.putExtra("latitude", latitude);
		launchShowComsActivity.putExtra("longitude", longitude);
		launchShowComsActivity.putExtra("name", name);
		launchShowComsActivity.putExtra("layerId", layerId);
		startActivityForResult(launchShowComsActivity, SHOW_COM);
	}
		
}