package com.microsoft.odaf.android.activities;

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

import com.microsoft.odaf.android.R;
import com.microsoft.odaf.android.models.OdafOverlay;
import com.microsoft.odaf.android.models.OdafOverlayBag;
import com.microsoft.odaf.android.models.OdafPoint;
import com.microsoft.odaf.android.utils.CheckableLinearLayout;
import com.microsoft.odaf.android.utils.OdafOverlayAdapter;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.Button;
import android.widget.ListView;

public class ChooseDataSourcesActivity extends Activity {
	
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.choosedatasource);
        
        OdafOverlayBag bag = getOdafOverlayBag();
        
        prepareListView(bag);
        
        Button button = (Button) findViewById(R.id.chooseDataSourcesOkButton);
        button.setOnClickListener(new OnClickListener() {			
			@Override
			public void onClick(View arg0) {
				returnToMainView();
			}
		});
	}
	
	
	
	@Override
	protected void onStart() {
		super.onStart();
		
		
	}

	private OdafOverlayBag getOdafOverlayBag()
	{
        OdafOverlayBag bag = OdafOverlayBag.getInstance();     
        if(bag.getOverlays().isEmpty()){
            String odafWebsitebaseUrl = getString(R.string.odafWebsitebaseUrl);
            try {
    			downloadDataSources(new URL(odafWebsitebaseUrl + "PointSources.xml"));
    		} catch (MalformedURLException e) {
    			e.printStackTrace();
    		}
        }      
        for(OdafOverlay overlay: bag.getOverlays()){
        	overlay.setSelected(false);
        }      
        return bag;
	}
	
	private void downloadDataSources(URL dataSourcesUrl) {
		try {
			URLConnection connection = dataSourcesUrl.openConnection();
			HttpURLConnection httpConnection = (HttpURLConnection) connection;
			int responseCode = httpConnection.getResponseCode();
			if (responseCode == HttpURLConnection.HTTP_OK) {
				InputStream in = httpConnection.getInputStream();
				DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
				DocumentBuilder db = dbf.newDocumentBuilder();
				Document dom = db.parse(in);
				parseDataSources(dom);
			}
		} catch (IOException e) {
		} catch (ParserConfigurationException e) {
		} catch (SAXException e) {
		}

	}
	
	private void parseDataSources(Document document){
		NodeList nl = document.getElementsByTagName("entry");
		if(nl != null && nl.getLength() > 0){
			long id = 1;
			for(int i=0; i < nl.getLength(); i++){
				Element entry = (Element) nl.item(i);
				
				Element titleElement = (Element)entry.getElementsByTagName("title").item(0);
				String title = titleElement.getFirstChild().getNodeValue();
				
				Element kmlLinkElement = (Element)entry.getElementsByTagName("link").item(0);
				String kmlLink = kmlLinkElement.getAttribute("href");
				
				Element imageLinkElement = (Element)entry.getElementsByTagName("link").item(1);
				String imageLink = imageLinkElement.getAttribute("href");
				if(!imageLink.startsWith("http")){
					imageLink = getString(R.string.odafWebsitebaseUrl) + imageLink;
				}
				
				Element idElement = (Element)entry.getElementsByTagName("id").item(0);
				String layerId = idElement.getFirstChild().getNodeValue();	
				
				OdafOverlay newOverlay = new OdafOverlay(id++,imageLink,kmlLink,layerId,false,title, new ArrayList<OdafPoint>());
				OdafOverlayBag.getInstance().getOverlays().add(newOverlay);
			}
		}
	}

	private void prepareListView(OdafOverlayBag bag)
	{
        OdafOverlayAdapter adapter = new OdafOverlayAdapter(this, R.layout.odafoverlay_list_item, bag.getOverlays());
        ListView listView = (ListView)findViewById(R.id.listView);
        listView.setAdapter(adapter);
        listView.setChoiceMode(ListView.CHOICE_MODE_MULTIPLE);
        listView.setItemsCanFocus(false);             
        
        listView.setOnItemClickListener(new OnItemClickListener(){

			@Override
			public void onItemClick(AdapterView<?> parent, View view, int pos, long id) {
				ListView listView = (ListView)findViewById(R.id.listView);
				CheckableLinearLayout cll = (CheckableLinearLayout) listView.getChildAt(pos);
				cll.toggle();
				
				OdafOverlay overlay = (OdafOverlay) listView.getItemAtPosition(pos);
				overlay.setSelected(!overlay.isSelected());
			}});
	}
	
	private void returnToMainView() {
		setResult(RESULT_OK);
		finish();
	}

}
