package com.microsoft.odaf.android.activities;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import com.microsoft.odaf.android.R;

import android.app.Activity;
import android.content.Intent;
import android.location.Address;
import android.location.Geocoder;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class ShowDetailsActivity extends Activity {
	
	private String description;
	private String guid;
	private int latitude;
	private int longitude;
	private String layerId;
	
	private TextView showDetailsTextView;
	private TextView showDetailsAddressTextview;
	private Button showDetailsShowComsButton;
	private Button showDetailsCloseViewButton;
	
	private Geocoder geoCoder;
	
	@Override
    public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
        setContentView(R.layout.showdetails);
        
        description = getIntent().getExtras().getString("description");
        guid = getIntent().getExtras().getString("guid");
        latitude = getIntent().getExtras().getInt("latitude");
        longitude = getIntent().getExtras().getInt("longitude");
        layerId = getIntent().getExtras().getString("layerId");
        
        String address = null;
        
        geoCoder = new Geocoder(this, Locale.FRANCE);
        try {
        	Double latDouble = (latitude / 1e6);
        	Double longDouble = (longitude / 1e6);
        	
			List<Address> addresses = geoCoder.getFromLocation(latDouble, longDouble, 1);
			if(addresses != null && !addresses.isEmpty()){
				Address addr = addresses.get(0);
				address = addr.getAddressLine(0) + ", " + addr.getPostalCode() + " " + addr.getLocality();
			}	
		} catch (IOException e) {		
			address = "Addresse non disponible";
		}
        
		showDetailsTextView = (TextView) findViewById(R.id.showDetailsTextView);
		showDetailsAddressTextview = (TextView) findViewById(R.id.showDetailsAddressTextView);
		showDetailsShowComsButton = (Button) findViewById(R.id.showDetailsShowComsButton);
		showDetailsCloseViewButton = (Button) findViewById(R.id.showDetailsCloseViewButton);
		
		showDetailsTextView.setText(description);
		showDetailsAddressTextview.setText(address);
		
		showDetailsShowComsButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View arg0) {
				returnToMainViewAndShowComs();			
			}
		});
		
		showDetailsCloseViewButton.setOnClickListener( new OnClickListener() {
		
			@Override
			public void onClick(View arg0) {			
				returnToMainView();
			}
		});
	}
	
	private void returnToMainViewAndShowComs() {
		Double latDouble = (latitude / 1e6);
    	Double longDouble = (longitude / 1e6);
    	
		Intent i = new Intent(); 
		i.putExtra("guid", guid);
		i.putExtra("latitude", latDouble);
		i.putExtra("longitude", longDouble);
		i.putExtra("name", description);
		i.putExtra("layerId", layerId);
		setResult(RESULT_OK, i);
		finish();
	}
	
	private void returnToMainView() {
		setResult(RESULT_CANCELED);
		finish();
	}
}
