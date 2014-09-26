


using System;
using SubSonic.Schema;
using System.Collections.Generic;
using SubSonic.DataProviders;
using System.Data;

namespace ODAF.Data {
	
        /// <summary>
        /// Table: OAuthAccount
        /// Primary Key: Id
        /// </summary>

        public class OAuthAccountTable: DatabaseTable {
            
            public OAuthAccountTable(IDataProvider provider):base("OAuthAccount",provider){
                ClassName = "OAuthAccount";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("user_id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("screen_name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 30
                });

                Columns.Add(new DatabaseColumn("oauth_token", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("oauth_token_secret", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("oauth_service_id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UserRole", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int16,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UserAccess", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int16,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("LastAccessedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("TokenExpiry", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("profile_image_url", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn user_id{
                get{
                    return this.GetColumn("user_id");
                }
            }
				
   			public static string user_idColumn{
			      get{
        			return "user_id";
      			}
		    }
            
            public IColumn screen_name{
                get{
                    return this.GetColumn("screen_name");
                }
            }
				
   			public static string screen_nameColumn{
			      get{
        			return "screen_name";
      			}
		    }
            
            public IColumn oauth_token{
                get{
                    return this.GetColumn("oauth_token");
                }
            }
				
   			public static string oauth_tokenColumn{
			      get{
        			return "oauth_token";
      			}
		    }
            
            public IColumn oauth_token_secret{
                get{
                    return this.GetColumn("oauth_token_secret");
                }
            }
				
   			public static string oauth_token_secretColumn{
			      get{
        			return "oauth_token_secret";
      			}
		    }
            
            public IColumn oauth_service_id{
                get{
                    return this.GetColumn("oauth_service_id");
                }
            }
				
   			public static string oauth_service_idColumn{
			      get{
        			return "oauth_service_id";
      			}
		    }
            
            public IColumn UserRole{
                get{
                    return this.GetColumn("UserRole");
                }
            }
				
   			public static string UserRoleColumn{
			      get{
        			return "UserRole";
      			}
		    }
            
            public IColumn UserAccess{
                get{
                    return this.GetColumn("UserAccess");
                }
            }
				
   			public static string UserAccessColumn{
			      get{
        			return "UserAccess";
      			}
		    }
            
            public IColumn LastAccessedOn{
                get{
                    return this.GetColumn("LastAccessedOn");
                }
            }
				
   			public static string LastAccessedOnColumn{
			      get{
        			return "LastAccessedOn";
      			}
		    }
            
            public IColumn TokenExpiry{
                get{
                    return this.GetColumn("TokenExpiry");
                }
            }
				
   			public static string TokenExpiryColumn{
			      get{
        			return "TokenExpiry";
      			}
		    }
            
            public IColumn profile_image_url{
                get{
                    return this.GetColumn("profile_image_url");
                }
            }
				
   			public static string profile_image_urlColumn{
			      get{
        			return "profile_image_url";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: OAuthAccountView
        /// Primary Key: 
        /// </summary>

        public class OAuthAccountViewTable: DatabaseTable {
            
            public OAuthAccountViewTable(IDataProvider provider):base("OAuthAccountView",provider){
                ClassName = "OAuthAccountView";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("user_id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("screen_name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 30
                });

                Columns.Add(new DatabaseColumn("oauth_token", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("oauth_token_secret", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("oauth_service_id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UserRole", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int16,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UserAccess", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int16,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("LastAccessedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("TokenExpiry", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("profile_image_url", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Summaries", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Comments", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn user_id{
                get{
                    return this.GetColumn("user_id");
                }
            }
				
   			public static string user_idColumn{
			      get{
        			return "user_id";
      			}
		    }
            
            public IColumn screen_name{
                get{
                    return this.GetColumn("screen_name");
                }
            }
				
   			public static string screen_nameColumn{
			      get{
        			return "screen_name";
      			}
		    }
            
            public IColumn oauth_token{
                get{
                    return this.GetColumn("oauth_token");
                }
            }
				
   			public static string oauth_tokenColumn{
			      get{
        			return "oauth_token";
      			}
		    }
            
            public IColumn oauth_token_secret{
                get{
                    return this.GetColumn("oauth_token_secret");
                }
            }
				
   			public static string oauth_token_secretColumn{
			      get{
        			return "oauth_token_secret";
      			}
		    }
            
            public IColumn oauth_service_id{
                get{
                    return this.GetColumn("oauth_service_id");
                }
            }
				
   			public static string oauth_service_idColumn{
			      get{
        			return "oauth_service_id";
      			}
		    }
            
            public IColumn UserRole{
                get{
                    return this.GetColumn("UserRole");
                }
            }
				
   			public static string UserRoleColumn{
			      get{
        			return "UserRole";
      			}
		    }
            
            public IColumn UserAccess{
                get{
                    return this.GetColumn("UserAccess");
                }
            }
				
   			public static string UserAccessColumn{
			      get{
        			return "UserAccess";
      			}
		    }
            
            public IColumn LastAccessedOn{
                get{
                    return this.GetColumn("LastAccessedOn");
                }
            }
				
   			public static string LastAccessedOnColumn{
			      get{
        			return "LastAccessedOn";
      			}
		    }
            
            public IColumn TokenExpiry{
                get{
                    return this.GetColumn("TokenExpiry");
                }
            }
				
   			public static string TokenExpiryColumn{
			      get{
        			return "TokenExpiry";
      			}
		    }
            
            public IColumn profile_image_url{
                get{
                    return this.GetColumn("profile_image_url");
                }
            }
				
   			public static string profile_image_urlColumn{
			      get{
        			return "profile_image_url";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn Summaries{
                get{
                    return this.GetColumn("Summaries");
                }
            }
				
   			public static string SummariesColumn{
			      get{
        			return "Summaries";
      			}
		    }
            
            public IColumn Comments{
                get{
                    return this.GetColumn("Comments");
                }
            }
				
   			public static string CommentsColumn{
			      get{
        			return "Comments";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: OAuthClientApp
        /// Primary Key: Id
        /// </summary>

        public class OAuthClientAppTable: DatabaseTable {
            
            public OAuthClientAppTable(IDataProvider provider):base("OAuthClientApp",provider){
                ClassName = "OAuthClientApp";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Guid", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 50
                });

                Columns.Add(new DatabaseColumn("Comment", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("ConsumerKey", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("ConsumerSecret", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("CallbackUrl", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 1000
                });

                Columns.Add(new DatabaseColumn("AppUrl", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 1000
                });

                Columns.Add(new DatabaseColumn("oauth_service_name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 50
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Guid{
                get{
                    return this.GetColumn("Guid");
                }
            }
				
   			public static string GuidColumn{
			      get{
        			return "Guid";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
            public IColumn Comment{
                get{
                    return this.GetColumn("Comment");
                }
            }
				
   			public static string CommentColumn{
			      get{
        			return "Comment";
      			}
		    }
            
            public IColumn ConsumerKey{
                get{
                    return this.GetColumn("ConsumerKey");
                }
            }
				
   			public static string ConsumerKeyColumn{
			      get{
        			return "ConsumerKey";
      			}
		    }
            
            public IColumn ConsumerSecret{
                get{
                    return this.GetColumn("ConsumerSecret");
                }
            }
				
   			public static string ConsumerSecretColumn{
			      get{
        			return "ConsumerSecret";
      			}
		    }
            
            public IColumn CallbackUrl{
                get{
                    return this.GetColumn("CallbackUrl");
                }
            }
				
   			public static string CallbackUrlColumn{
			      get{
        			return "CallbackUrl";
      			}
		    }
            
            public IColumn AppUrl{
                get{
                    return this.GetColumn("AppUrl");
                }
            }
				
   			public static string AppUrlColumn{
			      get{
        			return "AppUrl";
      			}
		    }
            
            public IColumn oauth_service_name{
                get{
                    return this.GetColumn("oauth_service_name");
                }
            }
				
   			public static string oauth_service_nameColumn{
			      get{
        			return "oauth_service_name";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataComment
        /// Primary Key: Id
        /// </summary>

        public class PointDataCommentTable: DatabaseTable {
            
            public PointDataCommentTable(IDataProvider provider):base("PointDataComment",provider){
                ClassName = "PointDataComment";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Text", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 4000
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedById", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("SummaryId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Text{
                get{
                    return this.GetColumn("Text");
                }
            }
				
   			public static string TextColumn{
			      get{
        			return "Text";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn CreatedById{
                get{
                    return this.GetColumn("CreatedById");
                }
            }
				
   			public static string CreatedByIdColumn{
			      get{
        			return "CreatedById";
      			}
		    }
            
            public IColumn SummaryId{
                get{
                    return this.GetColumn("SummaryId");
                }
            }
				
   			public static string SummaryIdColumn{
			      get{
        			return "SummaryId";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataCommentView
        /// Primary Key: 
        /// </summary>

        public class PointDataCommentViewTable: DatabaseTable {
            
            public PointDataCommentViewTable(IDataProvider provider):base("PointDataCommentView",provider){
                ClassName = "PointDataCommentView";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Text", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 4000
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedById", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("SummaryId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("screen_name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 30
                });

                Columns.Add(new DatabaseColumn("summary", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Text{
                get{
                    return this.GetColumn("Text");
                }
            }
				
   			public static string TextColumn{
			      get{
        			return "Text";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn CreatedById{
                get{
                    return this.GetColumn("CreatedById");
                }
            }
				
   			public static string CreatedByIdColumn{
			      get{
        			return "CreatedById";
      			}
		    }
            
            public IColumn SummaryId{
                get{
                    return this.GetColumn("SummaryId");
                }
            }
				
   			public static string SummaryIdColumn{
			      get{
        			return "SummaryId";
      			}
		    }
            
            public IColumn screen_name{
                get{
                    return this.GetColumn("screen_name");
                }
            }
				
   			public static string screen_nameColumn{
			      get{
        			return "screen_name";
      			}
		    }
            
            public IColumn summary{
                get{
                    return this.GetColumn("summary");
                }
            }
				
   			public static string summaryColumn{
			      get{
        			return "summary";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataFeed
        /// Primary Key: PointDataFeedId
        /// </summary>

        public class PointDataFeedTable: DatabaseTable {
            
            public PointDataFeedTable(IDataProvider provider):base("PointDataFeed",provider){
                ClassName = "PointDataFeed";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("PointDataFeedId", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("PointDataSourceId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UniqueId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Title", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Summary", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 500
                });

                Columns.Add(new DatabaseColumn("KMLFeedUrl", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("ImageUrl", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("IsRegion", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Active", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UpdatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn PointDataFeedId{
                get{
                    return this.GetColumn("PointDataFeedId");
                }
            }
				
   			public static string PointDataFeedIdColumn{
			      get{
        			return "PointDataFeedId";
      			}
		    }
            
            public IColumn PointDataSourceId{
                get{
                    return this.GetColumn("PointDataSourceId");
                }
            }
				
   			public static string PointDataSourceIdColumn{
			      get{
        			return "PointDataSourceId";
      			}
		    }
            
            public IColumn UniqueId{
                get{
                    return this.GetColumn("UniqueId");
                }
            }
				
   			public static string UniqueIdColumn{
			      get{
        			return "UniqueId";
      			}
		    }
            
            public IColumn Title{
                get{
                    return this.GetColumn("Title");
                }
            }
				
   			public static string TitleColumn{
			      get{
        			return "Title";
      			}
		    }
            
            public IColumn Summary{
                get{
                    return this.GetColumn("Summary");
                }
            }
				
   			public static string SummaryColumn{
			      get{
        			return "Summary";
      			}
		    }
            
            public IColumn KMLFeedUrl{
                get{
                    return this.GetColumn("KMLFeedUrl");
                }
            }
				
   			public static string KMLFeedUrlColumn{
			      get{
        			return "KMLFeedUrl";
      			}
		    }
            
            public IColumn ImageUrl{
                get{
                    return this.GetColumn("ImageUrl");
                }
            }
				
   			public static string ImageUrlColumn{
			      get{
        			return "ImageUrl";
      			}
		    }
            
            public IColumn IsRegion{
                get{
                    return this.GetColumn("IsRegion");
                }
            }
				
   			public static string IsRegionColumn{
			      get{
        			return "IsRegion";
      			}
		    }
            
            public IColumn Active{
                get{
                    return this.GetColumn("Active");
                }
            }
				
   			public static string ActiveColumn{
			      get{
        			return "Active";
      			}
		    }
            
            public IColumn UpdatedOn{
                get{
                    return this.GetColumn("UpdatedOn");
                }
            }
				
   			public static string UpdatedOnColumn{
			      get{
        			return "UpdatedOn";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataFeedView
        /// Primary Key: 
        /// </summary>

        public class PointDataFeedViewTable: DatabaseTable {
            
            public PointDataFeedViewTable(IDataProvider provider):base("PointDataFeedView",provider){
                ClassName = "PointDataFeedView";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("PointDataFeedId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("PointDataSourceId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UniqueId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Title", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Summary", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 500
                });

                Columns.Add(new DatabaseColumn("KMLFeedUrl", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("ImageUrl", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("IsRegion", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Active", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UpdatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("DataSourceName", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });
                    
                
                
            }
            
            public IColumn PointDataFeedId{
                get{
                    return this.GetColumn("PointDataFeedId");
                }
            }
				
   			public static string PointDataFeedIdColumn{
			      get{
        			return "PointDataFeedId";
      			}
		    }
            
            public IColumn PointDataSourceId{
                get{
                    return this.GetColumn("PointDataSourceId");
                }
            }
				
   			public static string PointDataSourceIdColumn{
			      get{
        			return "PointDataSourceId";
      			}
		    }
            
            public IColumn UniqueId{
                get{
                    return this.GetColumn("UniqueId");
                }
            }
				
   			public static string UniqueIdColumn{
			      get{
        			return "UniqueId";
      			}
		    }
            
            public IColumn Title{
                get{
                    return this.GetColumn("Title");
                }
            }
				
   			public static string TitleColumn{
			      get{
        			return "Title";
      			}
		    }
            
            public IColumn Summary{
                get{
                    return this.GetColumn("Summary");
                }
            }
				
   			public static string SummaryColumn{
			      get{
        			return "Summary";
      			}
		    }
            
            public IColumn KMLFeedUrl{
                get{
                    return this.GetColumn("KMLFeedUrl");
                }
            }
				
   			public static string KMLFeedUrlColumn{
			      get{
        			return "KMLFeedUrl";
      			}
		    }
            
            public IColumn ImageUrl{
                get{
                    return this.GetColumn("ImageUrl");
                }
            }
				
   			public static string ImageUrlColumn{
			      get{
        			return "ImageUrl";
      			}
		    }
            
            public IColumn IsRegion{
                get{
                    return this.GetColumn("IsRegion");
                }
            }
				
   			public static string IsRegionColumn{
			      get{
        			return "IsRegion";
      			}
		    }
            
            public IColumn Active{
                get{
                    return this.GetColumn("Active");
                }
            }
				
   			public static string ActiveColumn{
			      get{
        			return "Active";
      			}
		    }
            
            public IColumn UpdatedOn{
                get{
                    return this.GetColumn("UpdatedOn");
                }
            }
				
   			public static string UpdatedOnColumn{
			      get{
        			return "UpdatedOn";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn DataSourceName{
                get{
                    return this.GetColumn("DataSourceName");
                }
            }
				
   			public static string DataSourceNameColumn{
			      get{
        			return "DataSourceName";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataLayer
        /// Primary Key: Id
        /// </summary>

        public class PointDataLayerTable: DatabaseTable {
            
            public PointDataLayerTable(IDataProvider provider):base("PointDataLayer",provider){
                ClassName = "PointDataLayer";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Guid", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 50
                });

                Columns.Add(new DatabaseColumn("IsSystem", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("PointDataSourceId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Guid{
                get{
                    return this.GetColumn("Guid");
                }
            }
				
   			public static string GuidColumn{
			      get{
        			return "Guid";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
            public IColumn IsSystem{
                get{
                    return this.GetColumn("IsSystem");
                }
            }
				
   			public static string IsSystemColumn{
			      get{
        			return "IsSystem";
      			}
		    }
            
            public IColumn PointDataSourceId{
                get{
                    return this.GetColumn("PointDataSourceId");
                }
            }
				
   			public static string PointDataSourceIdColumn{
			      get{
        			return "PointDataSourceId";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataLayerView
        /// Primary Key: 
        /// </summary>

        public class PointDataLayerViewTable: DatabaseTable {
            
            public PointDataLayerViewTable(IDataProvider provider):base("PointDataLayerView",provider){
                ClassName = "PointDataLayerView";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Guid", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 50
                });

                Columns.Add(new DatabaseColumn("IsSystem", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("PointDataSourceId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Expr1", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("DataSourceName", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Guid{
                get{
                    return this.GetColumn("Guid");
                }
            }
				
   			public static string GuidColumn{
			      get{
        			return "Guid";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
            public IColumn IsSystem{
                get{
                    return this.GetColumn("IsSystem");
                }
            }
				
   			public static string IsSystemColumn{
			      get{
        			return "IsSystem";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn PointDataSourceId{
                get{
                    return this.GetColumn("PointDataSourceId");
                }
            }
				
   			public static string PointDataSourceIdColumn{
			      get{
        			return "PointDataSourceId";
      			}
		    }
            
            public IColumn Expr1{
                get{
                    return this.GetColumn("Expr1");
                }
            }
				
   			public static string Expr1Column{
			      get{
        			return "Expr1";
      			}
		    }
            
            public IColumn DataSourceName{
                get{
                    return this.GetColumn("DataSourceName");
                }
            }
				
   			public static string DataSourceNameColumn{
			      get{
        			return "DataSourceName";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataSource
        /// Primary Key: PointDataSourceId
        /// </summary>

        public class PointDataSourceTable: DatabaseTable {
            
            public PointDataSourceTable(IDataProvider provider):base("PointDataSource",provider){
                ClassName = "PointDataSource";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("PointDataSourceId", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UniqueId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Title", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("AuthorName", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("AuthorEmail", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("BoundaryPolygon", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 1000
                });

                Columns.Add(new DatabaseColumn("Active", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Description", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 500
                });

                Columns.Add(new DatabaseColumn("UpdatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn PointDataSourceId{
                get{
                    return this.GetColumn("PointDataSourceId");
                }
            }
				
   			public static string PointDataSourceIdColumn{
			      get{
        			return "PointDataSourceId";
      			}
		    }
            
            public IColumn UniqueId{
                get{
                    return this.GetColumn("UniqueId");
                }
            }
				
   			public static string UniqueIdColumn{
			      get{
        			return "UniqueId";
      			}
		    }
            
            public IColumn Title{
                get{
                    return this.GetColumn("Title");
                }
            }
				
   			public static string TitleColumn{
			      get{
        			return "Title";
      			}
		    }
            
            public IColumn AuthorName{
                get{
                    return this.GetColumn("AuthorName");
                }
            }
				
   			public static string AuthorNameColumn{
			      get{
        			return "AuthorName";
      			}
		    }
            
            public IColumn AuthorEmail{
                get{
                    return this.GetColumn("AuthorEmail");
                }
            }
				
   			public static string AuthorEmailColumn{
			      get{
        			return "AuthorEmail";
      			}
		    }
            
            public IColumn BoundaryPolygon{
                get{
                    return this.GetColumn("BoundaryPolygon");
                }
            }
				
   			public static string BoundaryPolygonColumn{
			      get{
        			return "BoundaryPolygon";
      			}
		    }
            
            public IColumn Active{
                get{
                    return this.GetColumn("Active");
                }
            }
				
   			public static string ActiveColumn{
			      get{
        			return "Active";
      			}
		    }
            
            public IColumn Description{
                get{
                    return this.GetColumn("Description");
                }
            }
				
   			public static string DescriptionColumn{
			      get{
        			return "Description";
      			}
		    }
            
            public IColumn UpdatedOn{
                get{
                    return this.GetColumn("UpdatedOn");
                }
            }
				
   			public static string UpdatedOnColumn{
			      get{
        			return "UpdatedOn";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataSourceView
        /// Primary Key: 
        /// </summary>

        public class PointDataSourceViewTable: DatabaseTable {
            
            public PointDataSourceViewTable(IDataProvider provider):base("PointDataSourceView",provider){
                ClassName = "PointDataSourceView";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("PointDataSourceId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("UniqueId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Title", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("AuthorName", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("AuthorEmail", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("BoundaryPolygon", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 1000
                });

                Columns.Add(new DatabaseColumn("Active", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Boolean,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Description", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 500
                });

                Columns.Add(new DatabaseColumn("UpdatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Feeds", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Layers", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn PointDataSourceId{
                get{
                    return this.GetColumn("PointDataSourceId");
                }
            }
				
   			public static string PointDataSourceIdColumn{
			      get{
        			return "PointDataSourceId";
      			}
		    }
            
            public IColumn UniqueId{
                get{
                    return this.GetColumn("UniqueId");
                }
            }
				
   			public static string UniqueIdColumn{
			      get{
        			return "UniqueId";
      			}
		    }
            
            public IColumn Title{
                get{
                    return this.GetColumn("Title");
                }
            }
				
   			public static string TitleColumn{
			      get{
        			return "Title";
      			}
		    }
            
            public IColumn AuthorName{
                get{
                    return this.GetColumn("AuthorName");
                }
            }
				
   			public static string AuthorNameColumn{
			      get{
        			return "AuthorName";
      			}
		    }
            
            public IColumn AuthorEmail{
                get{
                    return this.GetColumn("AuthorEmail");
                }
            }
				
   			public static string AuthorEmailColumn{
			      get{
        			return "AuthorEmail";
      			}
		    }
            
            public IColumn BoundaryPolygon{
                get{
                    return this.GetColumn("BoundaryPolygon");
                }
            }
				
   			public static string BoundaryPolygonColumn{
			      get{
        			return "BoundaryPolygon";
      			}
		    }
            
            public IColumn Active{
                get{
                    return this.GetColumn("Active");
                }
            }
				
   			public static string ActiveColumn{
			      get{
        			return "Active";
      			}
		    }
            
            public IColumn Description{
                get{
                    return this.GetColumn("Description");
                }
            }
				
   			public static string DescriptionColumn{
			      get{
        			return "Description";
      			}
		    }
            
            public IColumn UpdatedOn{
                get{
                    return this.GetColumn("UpdatedOn");
                }
            }
				
   			public static string UpdatedOnColumn{
			      get{
        			return "UpdatedOn";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn Feeds{
                get{
                    return this.GetColumn("Feeds");
                }
            }
				
   			public static string FeedsColumn{
			      get{
        			return "Feeds";
      			}
		    }
            
            public IColumn Layers{
                get{
                    return this.GetColumn("Layers");
                }
            }
				
   			public static string LayersColumn{
			      get{
        			return "Layers";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataSummary
        /// Primary Key: Id
        /// </summary>

        public class PointDataSummaryTable: DatabaseTable {
            
            public PointDataSummaryTable(IDataProvider provider):base("PointDataSummary",provider){
                ClassName = "PointDataSummary";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = true,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Guid", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 100
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Description", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 4000
                });

                Columns.Add(new DatabaseColumn("Latitude", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Decimal,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Longitude", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Decimal,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("LayerId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 40
                });

                Columns.Add(new DatabaseColumn("Tag", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = -1
                });

                Columns.Add(new DatabaseColumn("RatingCount", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("RatingTotal", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CommentCount", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("ModifiedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedById", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Guid{
                get{
                    return this.GetColumn("Guid");
                }
            }
				
   			public static string GuidColumn{
			      get{
        			return "Guid";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
            public IColumn Description{
                get{
                    return this.GetColumn("Description");
                }
            }
				
   			public static string DescriptionColumn{
			      get{
        			return "Description";
      			}
		    }
            
            public IColumn Latitude{
                get{
                    return this.GetColumn("Latitude");
                }
            }
				
   			public static string LatitudeColumn{
			      get{
        			return "Latitude";
      			}
		    }
            
            public IColumn Longitude{
                get{
                    return this.GetColumn("Longitude");
                }
            }
				
   			public static string LongitudeColumn{
			      get{
        			return "Longitude";
      			}
		    }
            
            public IColumn LayerId{
                get{
                    return this.GetColumn("LayerId");
                }
            }
				
   			public static string LayerIdColumn{
			      get{
        			return "LayerId";
      			}
		    }
            
            public IColumn Tag{
                get{
                    return this.GetColumn("Tag");
                }
            }
				
   			public static string TagColumn{
			      get{
        			return "Tag";
      			}
		    }
            
            public IColumn RatingCount{
                get{
                    return this.GetColumn("RatingCount");
                }
            }
				
   			public static string RatingCountColumn{
			      get{
        			return "RatingCount";
      			}
		    }
            
            public IColumn RatingTotal{
                get{
                    return this.GetColumn("RatingTotal");
                }
            }
				
   			public static string RatingTotalColumn{
			      get{
        			return "RatingTotal";
      			}
		    }
            
            public IColumn CommentCount{
                get{
                    return this.GetColumn("CommentCount");
                }
            }
				
   			public static string CommentCountColumn{
			      get{
        			return "CommentCount";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn ModifiedOn{
                get{
                    return this.GetColumn("ModifiedOn");
                }
            }
				
   			public static string ModifiedOnColumn{
			      get{
        			return "ModifiedOn";
      			}
		    }
            
            public IColumn CreatedById{
                get{
                    return this.GetColumn("CreatedById");
                }
            }
				
   			public static string CreatedByIdColumn{
			      get{
        			return "CreatedById";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: PointDataSummaryView
        /// Primary Key: 
        /// </summary>

        public class PointDataSummaryViewTable: DatabaseTable {
            
            public PointDataSummaryViewTable(IDataProvider provider):base("PointDataSummaryView",provider){
                ClassName = "PointDataSummaryView";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Id", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Guid", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 100
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 255
                });

                Columns.Add(new DatabaseColumn("Description", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 4000
                });

                Columns.Add(new DatabaseColumn("Latitude", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Decimal,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Longitude", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Decimal,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("LayerId", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 40
                });

                Columns.Add(new DatabaseColumn("Tag", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = -1
                });

                Columns.Add(new DatabaseColumn("RatingCount", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("RatingTotal", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CommentCount", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("ModifiedOn", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.DateTime,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("CreatedById", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int64,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("screen_name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 30
                });

                Columns.Add(new DatabaseColumn("Comments", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.Int32,
	                IsNullable = true,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 0
                });
                    
                
                
            }
            
            public IColumn Id{
                get{
                    return this.GetColumn("Id");
                }
            }
				
   			public static string IdColumn{
			      get{
        			return "Id";
      			}
		    }
            
            public IColumn Guid{
                get{
                    return this.GetColumn("Guid");
                }
            }
				
   			public static string GuidColumn{
			      get{
        			return "Guid";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
            public IColumn Description{
                get{
                    return this.GetColumn("Description");
                }
            }
				
   			public static string DescriptionColumn{
			      get{
        			return "Description";
      			}
		    }
            
            public IColumn Latitude{
                get{
                    return this.GetColumn("Latitude");
                }
            }
				
   			public static string LatitudeColumn{
			      get{
        			return "Latitude";
      			}
		    }
            
            public IColumn Longitude{
                get{
                    return this.GetColumn("Longitude");
                }
            }
				
   			public static string LongitudeColumn{
			      get{
        			return "Longitude";
      			}
		    }
            
            public IColumn LayerId{
                get{
                    return this.GetColumn("LayerId");
                }
            }
				
   			public static string LayerIdColumn{
			      get{
        			return "LayerId";
      			}
		    }
            
            public IColumn Tag{
                get{
                    return this.GetColumn("Tag");
                }
            }
				
   			public static string TagColumn{
			      get{
        			return "Tag";
      			}
		    }
            
            public IColumn RatingCount{
                get{
                    return this.GetColumn("RatingCount");
                }
            }
				
   			public static string RatingCountColumn{
			      get{
        			return "RatingCount";
      			}
		    }
            
            public IColumn RatingTotal{
                get{
                    return this.GetColumn("RatingTotal");
                }
            }
				
   			public static string RatingTotalColumn{
			      get{
        			return "RatingTotal";
      			}
		    }
            
            public IColumn CommentCount{
                get{
                    return this.GetColumn("CommentCount");
                }
            }
				
   			public static string CommentCountColumn{
			      get{
        			return "CommentCount";
      			}
		    }
            
            public IColumn CreatedOn{
                get{
                    return this.GetColumn("CreatedOn");
                }
            }
				
   			public static string CreatedOnColumn{
			      get{
        			return "CreatedOn";
      			}
		    }
            
            public IColumn ModifiedOn{
                get{
                    return this.GetColumn("ModifiedOn");
                }
            }
				
   			public static string ModifiedOnColumn{
			      get{
        			return "ModifiedOn";
      			}
		    }
            
            public IColumn CreatedById{
                get{
                    return this.GetColumn("CreatedById");
                }
            }
				
   			public static string CreatedByIdColumn{
			      get{
        			return "CreatedById";
      			}
		    }
            
            public IColumn screen_name{
                get{
                    return this.GetColumn("screen_name");
                }
            }
				
   			public static string screen_nameColumn{
			      get{
        			return "screen_name";
      			}
		    }
            
            public IColumn Comments{
                get{
                    return this.GetColumn("Comments");
                }
            }
				
   			public static string CommentsColumn{
			      get{
        			return "Comments";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: UserAccess
        /// Primary Key: Code
        /// </summary>

        public class UserAccessTable: DatabaseTable {
            
            public UserAccessTable(IDataProvider provider):base("UserAccess",provider){
                ClassName = "UserAccess";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Code", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int16,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 20
                });
                    
                
                
            }
            
            public IColumn Code{
                get{
                    return this.GetColumn("Code");
                }
            }
				
   			public static string CodeColumn{
			      get{
        			return "Code";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
                    
        }
        
        /// <summary>
        /// Table: UserRole
        /// Primary Key: Code
        /// </summary>

        public class UserRoleTable: DatabaseTable {
            
            public UserRoleTable(IDataProvider provider):base("UserRole",provider){
                ClassName = "UserRole";
                //SchemaName = "dbo";
                //SchemaName is deprecated in SQL Server 2008 (thus Azure), so we blank it out
                SchemaName = "";
                

                Columns.Add(new DatabaseColumn("Code", this)
                {
	                IsPrimaryKey = true,
	                DataType = DbType.Int16,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = true,
	                MaxLength = 0
                });

                Columns.Add(new DatabaseColumn("Name", this)
                {
	                IsPrimaryKey = false,
	                DataType = DbType.String,
	                IsNullable = false,
	                AutoIncrement = false,
	                IsForeignKey = false,
	                MaxLength = 20
                });
                    
                
                
            }
            
            public IColumn Code{
                get{
                    return this.GetColumn("Code");
                }
            }
				
   			public static string CodeColumn{
			      get{
        			return "Code";
      			}
		    }
            
            public IColumn Name{
                get{
                    return this.GetColumn("Name");
                }
            }
				
   			public static string NameColumn{
			      get{
        			return "Name";
      			}
		    }
            
                    
        }
        
}