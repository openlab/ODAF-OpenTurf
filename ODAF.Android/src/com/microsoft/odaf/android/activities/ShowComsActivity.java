package com.microsoft.odaf.android.activities;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.StatusLine;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.protocol.HTTP;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import winterwell.jtwitter.OAuthSignpostClient;
import winterwell.jtwitter.Twitter;
import winterwell.jtwitter.TwitterException;

import com.microsoft.odaf.android.R;
import com.microsoft.odaf.android.models.OdafComment;
import com.microsoft.odaf.android.utils.OdafCommentArayAdapter;

import android.app.Activity;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.webkit.WebView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

public class ShowComsActivity extends Activity {
	
	private String guid;
	private double latitude;
	private double longitude;
	private String name;
	private String layerId;
	
	private ListView comsListView;
	private Button addComButton;
	private Button closeButton;
	
	private WebView webView;
	private TextView loginTextView;
	private EditText loginEditText;
	private Button loginButton;
	
	private EditText commentEditText;
	private Button validateCommentButton;
	
	private OAuthSignpostClient oauthClient;
	private SharedPreferences preferences;
	String accessToken1;
	String accessToken2;
	
	@Override
    public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
        setContentView(R.layout.showcoms);
        
        comsListView = (ListView) findViewById(R.id.comsListView);
        addComButton = (Button) findViewById(R.id.showComsAddComButton);
        closeButton = (Button) findViewById(R.id.showComsCloseViewButton);
        webView = (WebView) findViewById(R.id.loginWebView);
        loginTextView = (TextView) findViewById(R.id.loginTextView);
        loginEditText = (EditText) findViewById(R.id.loginEditText);
        loginButton = (Button) findViewById(R.id.loginButton);
        commentEditText = (EditText) findViewById(R.id.commentEditText);
        validateCommentButton = (Button) findViewById(R.id.validateCommentButton);
        
        addComButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View arg0) {
				showAddComInterface();				
			}
		});
        closeButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View arg0) {
				returnToMainView(RESULT_CANCELED);
			}
		});
        
        latitude = getIntent().getDoubleExtra("latitude", 0);
        longitude = getIntent().getDoubleExtra("longitude", 0);
        guid = getIntent().getExtras().getString("guid");
        name = getIntent().getExtras().getString("name");
        layerId = getIntent().getExtras().getString("layerId");
        
        JSONArray comments = downloadComs(guid);
        if(comments != null){
        	ArrayList<OdafComment> odafComments = parseComs(comments);
        	
        	OdafCommentArayAdapter adapter = new OdafCommentArayAdapter(this, R.layout.odafcomment_list_item, odafComments);
        	comsListView.setAdapter(adapter);
        }else{
//        	TextView tv = new TextView(this);
//        	tv.setText("Pas de commentaires");
        	
        }
    }
	
	private JSONArray downloadComs(String guid){
		StringBuilder builder = new StringBuilder();
		HttpClient client = new DefaultHttpClient();
		HttpGet httpGet = new HttpGet( getString(R.string.odafWebsitebaseUrl) + "Comments/List.json/0?guid=" + guid);
		try {
			HttpResponse response = client.execute(httpGet);
			StatusLine statusLine = response.getStatusLine();
			int statusCode = statusLine.getStatusCode();
			if (statusCode == 200) {
				HttpEntity entity = response.getEntity();
				InputStream content = entity.getContent();
				BufferedReader reader = new BufferedReader(
						new InputStreamReader(content));
				String line;
				while ((line = reader.readLine()) != null) {
					builder.append(line);
				}
			}
		} catch (ClientProtocolException e) {
		} catch (IOException e) {
		}
		try {
			JSONArray array = new JSONArray(builder.toString());
			return array;
		} catch (JSONException e) {
		}
		return null;
	}
	
	private ArrayList<OdafComment> parseComs(JSONArray commentArrayBag){
		
		ArrayList<OdafComment> odafComments = new ArrayList<OdafComment>();
		int length = commentArrayBag.length();
		for(int i = 0; i < length; i++){
			try {
				JSONObject commentBag = commentArrayBag.getJSONObject(i);		
				String commentAuthor = commentBag.getString("CommentAuthor");
				
				JSONObject comment = commentBag.getJSONObject("Comment");
				String text = comment.getString("Text");
				String createdOnString = comment.getString("CreatedOn");
				int startIndex = createdOnString.indexOf("(");
				int endIndex = createdOnString.indexOf(")");
				Long createdOnLong = Long.parseLong(createdOnString.substring(startIndex + 1, endIndex));
				Date createdOn = new Date(createdOnLong);
				
				OdafComment odafComment = new OdafComment(text, commentAuthor, createdOn);
				odafComments.add(odafComment);
			} catch (JSONException e) {
			}		
		}
		return odafComments;
	}
	
	private void showAddComInterface() {		
		preferences = getSharedPreferences("MY_PREFS", Activity.MODE_PRIVATE);
		accessToken1 =  preferences.getString("accessToken1", "");
		accessToken2 =  preferences.getString("accessToken2", "");
		
		if(accessToken1 == "" || accessToken2 == ""){
			
			String consumerKey = getString(R.string.consumerKey);
			String consumerSecret = getString(R.string.consumerSecret);	
			oauthClient = new OAuthSignpostClient(consumerKey, consumerSecret, "oob");
			
			comsListView.setVisibility(View.GONE);
			addComButton.setVisibility(View.GONE);
			
			webView.setVisibility(View.VISIBLE);
			loginTextView.setVisibility(View.VISIBLE);
			loginEditText.setVisibility(View.VISIBLE);
			loginButton.setVisibility(View.VISIBLE);
			
			webView.loadUrl(oauthClient.authorizeUrl().toString());
			webView.requestFocus(View.FOCUS_DOWN);
			webView.setOnTouchListener(new View.OnTouchListener() {
				public boolean onTouch(View v, MotionEvent event) {
					switch (event.getAction()) {
					case MotionEvent.ACTION_DOWN:
					case MotionEvent.ACTION_UP:
						if (!v.hasFocus()) {
							v.requestFocus();
						}
						break;
					}
					return false;
				}
			});
			
			loginButton.setOnClickListener(new OnClickListener() {			
				@Override
				public void onClick(View v) {
					try{
					oauthClient.setAuthorizationCode(loginEditText.getText().toString());
					String[] accessToken = oauthClient.getAccessToken();
					SharedPreferences.Editor editor = preferences.edit();
					editor.putString("accessToken1", accessToken[0]);
					editor.putString("accessToken2", accessToken[1]);
					editor.commit();
					
					accessToken1 =  preferences.getString("accessToken1", "");
					accessToken2 =  preferences.getString("accessToken2", "");
					
					ShowSubmitCommentInterface();
					}catch (TwitterException e) {						
					}
				}
			});
		}
		else{
			String consumerKey = getString(R.string.consumerKey);
			String consumerSecret = getString(R.string.consumerSecret);	
			oauthClient = new OAuthSignpostClient(consumerKey, consumerSecret, accessToken1, accessToken2);
			ShowSubmitCommentInterface();
		}
		
	}
	
	private void returnToMainView(int resultCode) {
		setResult(resultCode);
		finish();
	}
	
	private void ShowSubmitCommentInterface(){
		
		comsListView.setVisibility(View.GONE);
		addComButton.setVisibility(View.GONE);
		
		webView.setVisibility(View.GONE);
		loginTextView.setVisibility(View.GONE);
		loginEditText.setVisibility(View.GONE);
		loginButton.setVisibility(View.GONE);
		
		commentEditText.setVisibility(View.VISIBLE);
		validateCommentButton.setVisibility(View.VISIBLE);
		
			validateCommentButton.setOnClickListener(new OnClickListener() {	
				@Override
				public void onClick(View arg0){
					try{
						Twitter twitter = new Twitter(null, oauthClient);
						twitter.setMyLocation(new double[]{ latitude, longitude});
						twitter.setStatus(commentEditText.getText().toString());
					}catch (TwitterException e){				
					}catch (Throwable th) {					
					}finally{
						SendCommentToOdaf(commentEditText.getText().toString());
						returnToMainView(RESULT_OK);
					}
				}
			});			
	}
	
	private void SendCommentToOdaf(String text) {
		try {
			DefaultHttpClient httpClient = new DefaultHttpClient();

			HttpPost postAuthenticate = new HttpPost(
					getString(R.string.odafWebsitebaseUrl)
							+ "User/Authenticate.json/" + "?oauth_token="
							+ accessToken1 + "&oauth_token_secret="
							+ accessToken2 + "&appId="
							+ getString(R.string.odafAppId));

			HttpResponse respAuthenticate = httpClient
					.execute(postAuthenticate);
			int respAuthenticateStatusCode = respAuthenticate.getStatusLine()
					.getStatusCode();

			if (respAuthenticateStatusCode == 200) {

				HttpPost postCreateSummary = new HttpPost(
						getString(R.string.odafWebsitebaseUrl)
								+ "Summaries/Add.json");
				List<NameValuePair> nvpsCreateSummary = new ArrayList<NameValuePair>();
				nvpsCreateSummary
						.add(new BasicNameValuePair("Description", " "));
				nvpsCreateSummary
						.add(new BasicNameValuePair("LayerId", layerId));
				nvpsCreateSummary.add(new BasicNameValuePair("Latitude", String
						.valueOf(latitude)));
				nvpsCreateSummary.add(new BasicNameValuePair("Longitude",
						String.valueOf(longitude)));
				nvpsCreateSummary.add(new BasicNameValuePair("Guid", guid));
				nvpsCreateSummary.add(new BasicNameValuePair("Name", name));
				nvpsCreateSummary.add(new BasicNameValuePair("Tag", " "));
				postCreateSummary.setEntity(new UrlEncodedFormEntity(
						nvpsCreateSummary, HTTP.UTF_8));

				HttpResponse respCreateSummary = httpClient
						.execute(postCreateSummary);
				int respCreateSummaryStatusCode = respCreateSummary
						.getStatusLine().getStatusCode();

				HttpPost postAddComment = new HttpPost(
						getString(R.string.odafWebsitebaseUrl)
								+ "Comments/Add.json/" + "0?guid=" + guid);
				List<NameValuePair> nvpsAddComment = new ArrayList<NameValuePair>();
				nvpsAddComment.add(new BasicNameValuePair("Text", text));
				postAddComment.setEntity(new UrlEncodedFormEntity(
						nvpsAddComment, HTTP.UTF_8));

				HttpResponse respAddComment = httpClient
						.execute(postAddComment);
				int respAddCommentStatusCode = respAddComment.getStatusLine()
						.getStatusCode();
			}
		} catch (ClientProtocolException e) {
		} catch (IOException e) {
		}
	}
	
}
