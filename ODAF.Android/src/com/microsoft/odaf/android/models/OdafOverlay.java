package com.microsoft.odaf.android.models;

import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.util.ArrayList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Point;
import android.graphics.RectF;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;
import com.microsoft.odaf.android.activities.MainMapActivity;

public class OdafOverlay extends Overlay {

	private Context context;
	private long id;
	private String imageLink;
	private String kmlLink;
	private String layerId;
	private boolean selected;
	private String title;
	private ArrayList<OdafPoint> points;
	
	public OdafOverlay(long id,String imageLink, String kmlLink, String layerId,
			boolean selected, String title, ArrayList<OdafPoint> points) {
		super();
		this.id = id;
		this.imageLink = imageLink;
		this.kmlLink = kmlLink;
		this.layerId = layerId;
		this.selected = selected;
		this.title = title;
		this.points = points;
	}

	@Override
	public void draw(Canvas canvas, MapView mapView, boolean shadow) {
		if(points.isEmpty()){
			Document kmlDocument = downloadKml();
			points = parseKmlDocument(kmlDocument);	
		}
		drawOdafPointsOnMap(canvas, mapView, shadow);
	}

	@Override
	public boolean onTap(GeoPoint point, MapView mapView) {
		Point screenPoint = new Point();
		mapView.getProjection().toPixels(point, screenPoint);
		
		for(OdafPoint odafPoint : this.points){
			if(odafPoint.getBounds().contains(screenPoint.x, screenPoint.y)){
				MainMapActivity parent = (MainMapActivity) this.context;
				parent.ShowDetails(odafPoint);
				return true;
			}
		}	
		return false;
	}
	
	private Document downloadKml(){
		Document dom = null;
		try {
			URL url = new URL(kmlLink.replaceAll(" ", "%20"));

			URLConnection connection = url.openConnection();
			HttpURLConnection httpConnection = (HttpURLConnection) connection;
			int responseCode = httpConnection.getResponseCode();
			if (responseCode == HttpURLConnection.HTTP_OK) {
				InputStream in = httpConnection.getInputStream();
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				dom = db.parse(in);
			}	
		} catch (MalformedURLException e) {
		} catch (IOException e) {
		} catch (ParserConfigurationException e) {
		} catch (SAXException e) {
		}
		return dom;
	}
	
	private ArrayList<OdafPoint> parseKmlDocument(Document kml){
		ArrayList<OdafPoint> points = new ArrayList<OdafPoint>();
		NodeList nl = kml.getElementsByTagName("Placemark");
		if(nl != null && nl.getLength() > 0){
			for(int i=0; i < nl.getLength(); i++){
				Element placemark = (Element) nl.item(i);
				
				Element guidElement = (Element)placemark.getElementsByTagName("name").item(0);
				String guid = guidElement.getFirstChild().getNodeValue();
				
				Element coordinatesElement = (Element) placemark.getElementsByTagName("coordinates").item(0);
				String coordinates = coordinatesElement.getFirstChild().getNodeValue();
				String[] longlat = coordinates.split(",");
				Double longitude = Double.valueOf(longlat[0]);
				Double latitude = Double.valueOf(longlat[1]);
				GeoPoint geoPoint = new GeoPoint((int)(latitude * 1e6), (int)(longitude * 1e6));
				
				Element descriptionElement = (Element)placemark.getElementsByTagName("description").item(0);
				String description = descriptionElement.getFirstChild().getNodeValue();
				
				OdafPoint newOdafPoint = new OdafPoint(geoPoint, guid, description, layerId);
				points.add(newOdafPoint);				
			}
		}
		return points;
	}
	
	private void drawOdafPointsOnMap(Canvas canvas, MapView mapView, boolean shadow){
		Bitmap imageBmp = null;
		String imageLink = this.imageLink;
		try {
			URL ulrn = new URL(imageLink);
			HttpURLConnection con = (HttpURLConnection)ulrn.openConnection();
		    InputStream is = con.getInputStream();
		    imageBmp = BitmapFactory.decodeStream(is);
		} catch (MalformedURLException e) {
		} catch (IOException e) {
		}
		
		for(OdafPoint odafPoint : this.points){
			Point screenPoint = new Point();
			mapView.getProjection().toPixels(odafPoint.getGeoPoint(), screenPoint);		
			canvas.drawBitmap(imageBmp, screenPoint.x - imageBmp.getWidth()/2, screenPoint.y - imageBmp.getHeight()/2, null);
			RectF bounds = new RectF(
					screenPoint.x  - imageBmp.getWidth()/2,
					screenPoint.y - imageBmp.getHeight()/2,
					screenPoint.x + imageBmp.getWidth()/2,
					screenPoint.y + imageBmp.getHeight()/2);
			odafPoint.setBounds(bounds);
		}
	}
	
	public Context getContext() {
		return context;
	}

	public void setContext(Context context) {
		this.context = context;
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	public String getLayerId() {
		return layerId;
	}

	public void setLayerId(String layerId) {
		this.layerId = layerId;
	}

	public String getImageLink() {
		return imageLink;
	}

	public void setImageLink(String imageLink) {
		this.imageLink = imageLink;
	}

	public String getKmlLink() {
		return kmlLink;
	}

	public void setKmlLink(String kmlLink) {
		this.kmlLink = kmlLink;
	}

	public String getTitle() {
		return title;
	}

	public void setTitle(String title) {
		this.title = title;
	}

	public ArrayList<OdafPoint> getPoints() {
		return points;
	}

	public void setPoints(ArrayList<OdafPoint> points) {
		this.points = points;
	}

	public boolean isSelected() {
		return selected;
	}

	public void setSelected(boolean selected) {
		this.selected = selected;
	}	
}
