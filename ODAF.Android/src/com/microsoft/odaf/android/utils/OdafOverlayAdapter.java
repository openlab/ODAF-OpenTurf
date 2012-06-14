package com.microsoft.odaf.android.utils;

import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;

import com.microsoft.odaf.android.R;
import com.microsoft.odaf.android.models.OdafOverlay;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

public class OdafOverlayAdapter extends ArrayAdapter<OdafOverlay> {

	int resource;

	public OdafOverlayAdapter(Context context, int textViewResourceId, List<OdafOverlay> objects) {
		super(context, textViewResourceId, objects);
		resource = textViewResourceId;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		CheckableLinearLayout layout;
		
		OdafOverlay overlay = getItem(position);
		String title = overlay.getTitle();
		
		Bitmap imageBmp = null;
		String imageLink = overlay.getImageLink();
		try {
			URL ulrn = new URL(imageLink);
			HttpURLConnection con = (HttpURLConnection)ulrn.openConnection();
		    InputStream is = con.getInputStream();
		    imageBmp = BitmapFactory.decodeStream(is);
		} catch (MalformedURLException e) {
		} catch (IOException e) {
		}
		
		if(convertView == null){
			layout = new CheckableLinearLayout(getContext(), 0);
			String inflater = Context.LAYOUT_INFLATER_SERVICE;
			LayoutInflater vi = (LayoutInflater)getContext().getSystemService(inflater);
			layout = (CheckableLinearLayout) vi.inflate(resource, null);
		}else{
			layout = (CheckableLinearLayout) convertView;
		}
	    
		TextView textView = (TextView) layout.findViewById(R.id.textView);
		ImageView imageView = (ImageView) layout.findViewById(R.id.imageView);
		
			
		textView.setText(title);
		imageView.setImageBitmap(imageBmp);
		layout.title = title;
				
		return layout;
	}
	
	@Override
	public long getItemId(int position) {
		return getItem(position).getId();
	}

	@Override
	public boolean hasStableIds() {
		return true;
	}
}
