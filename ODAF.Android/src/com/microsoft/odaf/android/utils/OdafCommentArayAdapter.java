package com.microsoft.odaf.android.utils;

import java.util.List;

import com.microsoft.odaf.android.R;
import com.microsoft.odaf.android.models.OdafComment;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.LinearLayout;
import android.widget.TextView;

public class OdafCommentArayAdapter extends ArrayAdapter<OdafComment> {

	int resource;

	public OdafCommentArayAdapter(Context context, int textViewResourceId, List<OdafComment> objects) {
		super(context, textViewResourceId, objects);
		resource = textViewResourceId;
	}
	
	@Override
	public View getView(int position, View convertView, ViewGroup parent) {
		
		LinearLayout commentView;
		
		OdafComment odafComment = getItem(position);
		String comment = odafComment.getComment();
		String author = odafComment.getAuthor();
		String date = odafComment.getDate().toLocaleString();
		
		if (convertView == null) {
			commentView = new LinearLayout(getContext());
			String inflater = Context.LAYOUT_INFLATER_SERVICE;
			LayoutInflater vi = (LayoutInflater)getContext().getSystemService(inflater);
			vi.inflate(resource, commentView, true);
		} else {
			commentView = (LinearLayout) convertView;
		}
		
		TextView commentTextView = (TextView) commentView.findViewById(R.id.commentTextView);
		TextView authorTextView = (TextView) commentView.findViewById(R.id.authorTextView);	
		TextView dateTextView = (TextView) commentView.findViewById(R.id.dateTextView);
		
		commentTextView.setText(comment);
		authorTextView.setText(author);
		dateTextView.setText(date);
		
		return commentView;
	}
	
}
