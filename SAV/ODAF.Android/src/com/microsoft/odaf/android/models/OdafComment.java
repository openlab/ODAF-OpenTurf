package com.microsoft.odaf.android.models;

import java.util.Date;

public class OdafComment {
	private String comment;
	private String author;
	private Date date;
	
	public OdafComment(String comment, String author, Date date) {
		super();
		this.comment = comment;
		this.author = author;
		this.date = date;
	}

	public String getComment() {
		return comment;
	}

	public void setComment(String comment) {
		this.comment = comment;
	}

	public String getAuthor() {
		return author;
	}

	public void setAuthor(String author) {
		this.author = author;
	}

	public Date getDate() {
		return date;
	}

	public void setDate(Date date) {
		this.date = date;
	}
}
