


using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using SubSonic.DataProviders;
using SubSonic.Extensions;
using SubSonic.Linq.Structure;
using SubSonic.Query;
using SubSonic.Schema;
using System.Data.Common;
using System.Collections.Generic;

namespace ODAF.Data
{
    public partial class odafDB : IQuerySurface
    {

        public IDataProvider DataProvider;
        public DbQueryProvider provider;
        
        public bool TestMode
		{
            get
			{
                return DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public odafDB() 
        { 
            DataProvider = ProviderFactory.GetProvider("ODAF");
            Init();
        }

        public odafDB(string connectionStringName)
        {
            DataProvider = ProviderFactory.GetProvider(connectionStringName);
            Init();
        }

		public odafDB(string connectionString, string providerName)
        {
            DataProvider = ProviderFactory.GetProvider(connectionString,providerName);
            Init();
        }

		public ITable FindByPrimaryKey(string pkName)
        {
            return DataProvider.Schema.Tables.SingleOrDefault(x => x.PrimaryKey.Name.Equals(pkName, StringComparison.InvariantCultureIgnoreCase));
        }

        public Query<T> GetQuery<T>()
        {
            return new Query<T>(provider);
        }
        
        public ITable FindTable(string tableName)
        {
            return DataProvider.FindTable(tableName);
        }
               
        public IDataProvider Provider
        {
            get { return DataProvider; }
            set {DataProvider=value;}
        }
        
        public DbQueryProvider QueryProvider
        {
            get { return provider; }
        }
        
        BatchQuery _batch = null;
        public void Queue<T>(IQueryable<T> qry)
        {
            if (_batch == null)
                _batch = new BatchQuery(Provider, QueryProvider);
            _batch.Queue(qry);
        }

        public void Queue(ISqlQuery qry)
        {
            if (_batch == null)
                _batch = new BatchQuery(Provider, QueryProvider);
            _batch.Queue(qry);
        }

        public void ExecuteTransaction(IList<DbCommand> commands)
		{
            if(!TestMode)
			{
                using(var connection = commands[0].Connection)
				{
                   if (connection.State == ConnectionState.Closed)
                        connection.Open();
                   
                   using (var trans = connection.BeginTransaction()) 
				   {
                        foreach (var cmd in commands) 
						{
                            cmd.Transaction = trans;
                            cmd.Connection = connection;
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    connection.Close();
                }
            }
        }

        public IDataReader ExecuteBatch()
        {
            if (_batch == null)
                throw new InvalidOperationException("There's nothing in the queue");
            if(!TestMode)
                return _batch.ExecuteReader();
            return null;
        }
			
        public Query<OAuthAccount> OAuthAccounts { get; set; }
        public Query<OAuthAccountView> OAuthAccountViews { get; set; }
        public Query<OAuthClientApp> OAuthClientApps { get; set; }
        public Query<PointDataComment> PointDataComments { get; set; }
        public Query<PointDataCommentView> PointDataCommentViews { get; set; }
        public Query<PointDataFeed> PointDataFeeds { get; set; }
        public Query<PointDataFeedView> PointDataFeedViews { get; set; }
        public Query<PointDataLayer> PointDataLayers { get; set; }
        public Query<PointDataLayerView> PointDataLayerViews { get; set; }
        public Query<PointDataSource> PointDataSources { get; set; }
        public Query<PointDataSourceView> PointDataSourceViews { get; set; }
        public Query<PointDataSummary> PointDataSummaries { get; set; }
        public Query<PointDataSummaryView> PointDataSummaryViews { get; set; }
        public Query<UserAccess> UserAccesses { get; set; }
        public Query<UserRole> UserRoles { get; set; }

			

        #region ' Aggregates and SubSonic Queries '
        public Select SelectColumns(params string[] columns)
        {
            return new Select(DataProvider, columns);
        }

        public Select Select
        {
            get { return new Select(this.Provider); }
        }

        public Insert Insert
		{
            get { return new Insert(this.Provider); }
        }

        public Update<T> Update<T>() where T:new()
		{
            return new Update<T>(this.Provider);
        }

        public SqlQuery Delete<T>(Expression<Func<T,bool>> column) where T:new()
        {
            LambdaExpression lamda = column;
            SqlQuery result = new Delete<T>(this.Provider);
            result = result.From<T>();
            result.Constraints=lamda.ParseConstraints().ToList();
            return result;
        }

        public SqlQuery Max<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = DataProvider.FindTable(objectName).Name;
            return new Select(DataProvider, new Aggregate(colName, AggregateFunction.Max)).From(tableName);
        }

        public SqlQuery Min<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = this.Provider.FindTable(objectName).Name;
            return new Select(this.Provider, new Aggregate(colName, AggregateFunction.Min)).From(tableName);
        }

        public SqlQuery Sum<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = this.Provider.FindTable(objectName).Name;
            return new Select(this.Provider, new Aggregate(colName, AggregateFunction.Sum)).From(tableName);
        }

        public SqlQuery Avg<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = this.Provider.FindTable(objectName).Name;
            return new Select(this.Provider, new Aggregate(colName, AggregateFunction.Avg)).From(tableName);
        }

        public SqlQuery Count<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = this.Provider.FindTable(objectName).Name;
            return new Select(this.Provider, new Aggregate(colName, AggregateFunction.Count)).From(tableName);
        }

        public SqlQuery Variance<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = this.Provider.FindTable(objectName).Name;
            return new Select(this.Provider, new Aggregate(colName, AggregateFunction.Var)).From(tableName);
        }

        public SqlQuery StandardDeviation<T>(Expression<Func<T,object>> column)
        {
            LambdaExpression lamda = column;
            string colName = lamda.ParseObjectValue();
            string objectName = typeof(T).Name;
            string tableName = this.Provider.FindTable(objectName).Name;
            return new Select(this.Provider, new Aggregate(colName, AggregateFunction.StDev)).From(tableName);
        }

        #endregion

        void Init()
        {
            provider = new DbQueryProvider(this.Provider);

            #region ' Query Defs '
            OAuthAccounts = new Query<OAuthAccount>(provider);
            OAuthAccountViews = new Query<OAuthAccountView>(provider);
            OAuthClientApps = new Query<OAuthClientApp>(provider);
            PointDataComments = new Query<PointDataComment>(provider);
            PointDataCommentViews = new Query<PointDataCommentView>(provider);
            PointDataFeeds = new Query<PointDataFeed>(provider);
            PointDataFeedViews = new Query<PointDataFeedView>(provider);
            PointDataLayers = new Query<PointDataLayer>(provider);
            PointDataLayerViews = new Query<PointDataLayerView>(provider);
            PointDataSources = new Query<PointDataSource>(provider);
            PointDataSourceViews = new Query<PointDataSourceView>(provider);
            PointDataSummaries = new Query<PointDataSummary>(provider);
            PointDataSummaryViews = new Query<PointDataSummaryView>(provider);
            UserAccesses = new Query<UserAccess>(provider);
            UserRoles = new Query<UserRole>(provider);
            #endregion


            #region ' Schemas '
        	if(DataProvider.Schema.Tables.Count == 0)
			{
            	DataProvider.Schema.Tables.Add(new OAuthAccountTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new OAuthAccountViewTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new OAuthClientAppTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataCommentTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataCommentViewTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataFeedTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataFeedViewTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataLayerTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataLayerViewTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataSourceTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataSourceViewTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataSummaryTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new PointDataSummaryViewTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new UserAccessTable(DataProvider));
            	DataProvider.Schema.Tables.Add(new UserRoleTable(DataProvider));
            }
            #endregion
        }
    }
}