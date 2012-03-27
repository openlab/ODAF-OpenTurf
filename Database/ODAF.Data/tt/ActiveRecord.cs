


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SubSonic.DataProviders;
using SubSonic.Extensions;
using System.Linq.Expressions;
using SubSonic.Schema;
using System.Collections;
using SubSonic;
using SubSonic.Repository;
using System.ComponentModel;
using System.Data.Common;
using System.Web.Script.Serialization;

namespace ODAF.Data
{
    
    
    /// <summary>
    /// A class which represents the OAuthAccount table in the odaf Database.
    /// </summary>
    public partial class OAuthAccount: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<OAuthAccount> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<OAuthAccount>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<OAuthAccount> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(OAuthAccount item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                OAuthAccount item=new OAuthAccount();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<OAuthAccount> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public OAuthAccount(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                OAuthAccount.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<OAuthAccount>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public OAuthAccount(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public OAuthAccount(Expression<Func<OAuthAccount, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<OAuthAccount> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<OAuthAccount> _repo;
            
            if(db.TestMode){
                OAuthAccount.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<OAuthAccount>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<OAuthAccount> GetRepo(){
            return GetRepo("","");
        }
        
        public static OAuthAccount SingleOrDefault(Expression<Func<OAuthAccount, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            OAuthAccount single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static OAuthAccount SingleOrDefault(Expression<Func<OAuthAccount, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            OAuthAccount single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<OAuthAccount, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<OAuthAccount, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<OAuthAccount> Find(Expression<Func<OAuthAccount, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<OAuthAccount> Find(Expression<Func<OAuthAccount, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<OAuthAccount> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<OAuthAccount> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<OAuthAccount> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<OAuthAccount> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<OAuthAccount> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<OAuthAccount> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.screen_name.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(OAuthAccount)){
                OAuthAccount compare=(OAuthAccount)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.screen_name.ToString();
        }

        public string DescriptorColumn() {
            return "screen_name";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "screen_name";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<OAuthClientApp> OAuthClientApps
        {
            get
            {
                
                  var repo=ODAF.Data.OAuthClientApp.GetRepo();
                  return from items in repo.GetAll()
                       where items.Id == _oauth_service_id
                       select items;
            }
        }

		[ScriptIgnore]
        public IQueryable<UserAccess> UserAccesses
        {
            get
            {
                
                  var repo=ODAF.Data.UserAccess.GetRepo();
                  return from items in repo.GetAll()
                       where items.Code == _UserAccess
                       select items;
            }
        }

		[ScriptIgnore]
        public IQueryable<UserRole> UserRoles
        {
            get
            {
                
                  var repo=ODAF.Data.UserRole.GetRepo();
                  return from items in repo.GetAll()
                       where items.Code == _UserRole
                       select items;
            }
        }

		[ScriptIgnore]
        public IQueryable<PointDataComment> PointDataComments
        {
            get
            {
                
                  var repo=ODAF.Data.PointDataComment.GetRepo();
                  return from items in repo.GetAll()
                       where items.CreatedById == _Id
                       select items;
            }
        }

		[ScriptIgnore]
        public IQueryable<PointDataSummary> PointDataSummaries
        {
            get
            {
                
                  var repo=ODAF.Data.PointDataSummary.GetRepo();
                  return from items in repo.GetAll()
                       where items.CreatedById == _Id
                       select items;
            }
        }

        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _user_id;
        public long user_id
        {
            get { return _user_id; }
            set
            {
                if(_user_id!=value){
                    _user_id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="user_id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _screen_name;
        public string screen_name
        {
            get { return _screen_name; }
            set
            {
                if(_screen_name!=value){
                    _screen_name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="screen_name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _oauth_token;
        public string oauth_token
        {
            get { return _oauth_token; }
            set
            {
                if(_oauth_token!=value){
                    _oauth_token=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_token");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _oauth_token_secret;
        public string oauth_token_secret
        {
            get { return _oauth_token_secret; }
            set
            {
                if(_oauth_token_secret!=value){
                    _oauth_token_secret=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_token_secret");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        short _UserRole;
        public short UserRole
        {
            get { return _UserRole; }
            set
            {
                if(_UserRole!=value){
                    _UserRole=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UserRole");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        short _UserAccess;
        public short UserAccess
        {
            get { return _UserAccess; }
            set
            {
                if(_UserAccess!=value){
                    _UserAccess=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UserAccess");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _LastAccessedOn;
        public DateTime LastAccessedOn
        {
            get { return _LastAccessedOn; }
            set
            {
                if(_LastAccessedOn!=value){
                    _LastAccessedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="LastAccessedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _TokenExpiry;
        public DateTime TokenExpiry
        {
            get { return _TokenExpiry; }
            set
            {
                if(_TokenExpiry!=value){
                    _TokenExpiry=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="TokenExpiry");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _oauth_service_id;
        public int oauth_service_id
        {
            get { return _oauth_service_id; }
            set
            {
                if(_oauth_service_id!=value){
                    _oauth_service_id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_service_id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _profile_image_url;
        public string profile_image_url
        {
            get { return _profile_image_url; }
            set
            {
                if(_profile_image_url!=value){
                    _profile_image_url=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="profile_image_url");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<OAuthAccount, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the OAuthAccountView table in the odaf Database.
    /// </summary>
    public partial class OAuthAccountView: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<OAuthAccountView> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<OAuthAccountView>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<OAuthAccountView> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(OAuthAccountView item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                OAuthAccountView item=new OAuthAccountView();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<OAuthAccountView> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public OAuthAccountView(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                OAuthAccountView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<OAuthAccountView>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public OAuthAccountView(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public OAuthAccountView(Expression<Func<OAuthAccountView, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<OAuthAccountView> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<OAuthAccountView> _repo;
            
            if(db.TestMode){
                OAuthAccountView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<OAuthAccountView>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<OAuthAccountView> GetRepo(){
            return GetRepo("","");
        }
        
        public static OAuthAccountView SingleOrDefault(Expression<Func<OAuthAccountView, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            OAuthAccountView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static OAuthAccountView SingleOrDefault(Expression<Func<OAuthAccountView, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            OAuthAccountView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<OAuthAccountView, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<OAuthAccountView, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<OAuthAccountView> Find(Expression<Func<OAuthAccountView, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<OAuthAccountView> Find(Expression<Func<OAuthAccountView, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<OAuthAccountView> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<OAuthAccountView> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<OAuthAccountView> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<OAuthAccountView> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<OAuthAccountView> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<OAuthAccountView> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.screen_name.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(OAuthAccountView)){
                OAuthAccountView compare=(OAuthAccountView)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.screen_name.ToString();
        }

        public string DescriptorColumn() {
            return "screen_name";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "screen_name";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _user_id;
        public long user_id
        {
            get { return _user_id; }
            set
            {
                if(_user_id!=value){
                    _user_id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="user_id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _screen_name;
        public string screen_name
        {
            get { return _screen_name; }
            set
            {
                if(_screen_name!=value){
                    _screen_name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="screen_name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _oauth_token;
        public string oauth_token
        {
            get { return _oauth_token; }
            set
            {
                if(_oauth_token!=value){
                    _oauth_token=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_token");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _oauth_token_secret;
        public string oauth_token_secret
        {
            get { return _oauth_token_secret; }
            set
            {
                if(_oauth_token_secret!=value){
                    _oauth_token_secret=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_token_secret");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        short _UserRole;
        public short UserRole
        {
            get { return _UserRole; }
            set
            {
                if(_UserRole!=value){
                    _UserRole=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UserRole");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        short _UserAccess;
        public short UserAccess
        {
            get { return _UserAccess; }
            set
            {
                if(_UserAccess!=value){
                    _UserAccess=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UserAccess");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _LastAccessedOn;
        public DateTime LastAccessedOn
        {
            get { return _LastAccessedOn; }
            set
            {
                if(_LastAccessedOn!=value){
                    _LastAccessedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="LastAccessedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _TokenExpiry;
        public DateTime TokenExpiry
        {
            get { return _TokenExpiry; }
            set
            {
                if(_TokenExpiry!=value){
                    _TokenExpiry=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="TokenExpiry");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _oauth_service_id;
        public int oauth_service_id
        {
            get { return _oauth_service_id; }
            set
            {
                if(_oauth_service_id!=value){
                    _oauth_service_id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_service_id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _profile_image_url;
        public string profile_image_url
        {
            get { return _profile_image_url; }
            set
            {
                if(_profile_image_url!=value){
                    _profile_image_url=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="profile_image_url");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int? _Summaries;
        public int? Summaries
        {
            get { return _Summaries; }
            set
            {
                if(_Summaries!=value){
                    _Summaries=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Summaries");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int? _Comments;
        public int? Comments
        {
            get { return _Comments; }
            set
            {
                if(_Comments!=value){
                    _Comments=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Comments");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<OAuthAccountView, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the OAuthClientApp table in the odaf Database.
    /// </summary>
    public partial class OAuthClientApp: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<OAuthClientApp> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<OAuthClientApp>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<OAuthClientApp> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(OAuthClientApp item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                OAuthClientApp item=new OAuthClientApp();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<OAuthClientApp> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public OAuthClientApp(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                OAuthClientApp.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<OAuthClientApp>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public OAuthClientApp(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public OAuthClientApp(Expression<Func<OAuthClientApp, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<OAuthClientApp> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<OAuthClientApp> _repo;
            
            if(db.TestMode){
                OAuthClientApp.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<OAuthClientApp>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<OAuthClientApp> GetRepo(){
            return GetRepo("","");
        }
        
        public static OAuthClientApp SingleOrDefault(Expression<Func<OAuthClientApp, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            OAuthClientApp single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static OAuthClientApp SingleOrDefault(Expression<Func<OAuthClientApp, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            OAuthClientApp single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<OAuthClientApp, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<OAuthClientApp, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<OAuthClientApp> Find(Expression<Func<OAuthClientApp, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<OAuthClientApp> Find(Expression<Func<OAuthClientApp, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<OAuthClientApp> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<OAuthClientApp> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<OAuthClientApp> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<OAuthClientApp> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<OAuthClientApp> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<OAuthClientApp> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<int>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Guid.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(OAuthClientApp)){
                OAuthClientApp compare=(OAuthClientApp)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        
        public override int GetHashCode() {
            return this.Id;
        }
        
        public string DescriptorValue()
        {
            return this.Guid.ToString();
        }

        public string DescriptorColumn() {
            return "Guid";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Guid";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<OAuthAccount> OAuthAccounts
        {
            get
            {
                
                  var repo=ODAF.Data.OAuthAccount.GetRepo();
                  return from items in repo.GetAll()
                       where items.oauth_service_id == _Id
                       select items;
            }
        }

        #endregion
        

        int _Id;
        public int Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Guid;
        public string Guid
        {
            get { return _Guid; }
            set
            {
                if(_Guid!=value){
                    _Guid=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Guid");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Comment;
        public string Comment
        {
            get { return _Comment; }
            set
            {
                if(_Comment!=value){
                    _Comment=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Comment");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _ConsumerKey;
        public string ConsumerKey
        {
            get { return _ConsumerKey; }
            set
            {
                if(_ConsumerKey!=value){
                    _ConsumerKey=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="ConsumerKey");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _ConsumerSecret;
        public string ConsumerSecret
        {
            get { return _ConsumerSecret; }
            set
            {
                if(_ConsumerSecret!=value){
                    _ConsumerSecret=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="ConsumerSecret");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _CallbackUrl;
        public string CallbackUrl
        {
            get { return _CallbackUrl; }
            set
            {
                if(_CallbackUrl!=value){
                    _CallbackUrl=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CallbackUrl");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _AppUrl;
        public string AppUrl
        {
            get { return _AppUrl; }
            set
            {
                if(_AppUrl!=value){
                    _AppUrl=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="AppUrl");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _CreatedOn;
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _oauth_service_name;
        public string oauth_service_name
        {
            get { return _oauth_service_name; }
            set
            {
                if(_oauth_service_name!=value){
                    _oauth_service_name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="oauth_service_name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<OAuthClientApp, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataComment table in the odaf Database.
    /// </summary>
    public partial class PointDataComment: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataComment> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataComment>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataComment> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataComment item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataComment item=new PointDataComment();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataComment> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataComment(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataComment.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataComment>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataComment(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataComment(Expression<Func<PointDataComment, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataComment> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataComment> _repo;
            
            if(db.TestMode){
                PointDataComment.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataComment>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataComment> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataComment SingleOrDefault(Expression<Func<PointDataComment, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataComment single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataComment SingleOrDefault(Expression<Func<PointDataComment, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataComment single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataComment, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataComment, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataComment> Find(Expression<Func<PointDataComment, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataComment> Find(Expression<Func<PointDataComment, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataComment> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataComment> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataComment> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataComment> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataComment> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataComment> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Text.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataComment)){
                PointDataComment compare=(PointDataComment)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Text.ToString();
        }

        public string DescriptorColumn() {
            return "Text";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Text";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<OAuthAccount> OAuthAccounts
        {
            get
            {
                
                  var repo=ODAF.Data.OAuthAccount.GetRepo();
                  return from items in repo.GetAll()
                       where items.Id == _CreatedById
                       select items;
            }
        }

		[ScriptIgnore]
        public IQueryable<PointDataSummary> PointDataSummaries
        {
            get
            {
                
                  var repo=ODAF.Data.PointDataSummary.GetRepo();
                  return from items in repo.GetAll()
                       where items.Id == _SummaryId
                       select items;
            }
        }

        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if(_Text!=value){
                    _Text=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Text");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _CreatedOn;
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _CreatedById;
        public long CreatedById
        {
            get { return _CreatedById; }
            set
            {
                if(_CreatedById!=value){
                    _CreatedById=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedById");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _SummaryId;
        public long SummaryId
        {
            get { return _SummaryId; }
            set
            {
                if(_SummaryId!=value){
                    _SummaryId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="SummaryId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataComment, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataCommentView table in the odaf Database.
    /// </summary>
    public partial class PointDataCommentView: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataCommentView> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataCommentView>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataCommentView> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataCommentView item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataCommentView item=new PointDataCommentView();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataCommentView> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataCommentView(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataCommentView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataCommentView>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataCommentView(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataCommentView(Expression<Func<PointDataCommentView, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataCommentView> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataCommentView> _repo;
            
            if(db.TestMode){
                PointDataCommentView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataCommentView>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataCommentView> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataCommentView SingleOrDefault(Expression<Func<PointDataCommentView, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataCommentView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataCommentView SingleOrDefault(Expression<Func<PointDataCommentView, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataCommentView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataCommentView, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataCommentView, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataCommentView> Find(Expression<Func<PointDataCommentView, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataCommentView> Find(Expression<Func<PointDataCommentView, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataCommentView> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataCommentView> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataCommentView> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataCommentView> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataCommentView> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataCommentView> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Text.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataCommentView)){
                PointDataCommentView compare=(PointDataCommentView)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Text.ToString();
        }

        public string DescriptorColumn() {
            return "Text";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Text";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                if(_Text!=value){
                    _Text=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Text");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _CreatedOn;
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _CreatedById;
        public long CreatedById
        {
            get { return _CreatedById; }
            set
            {
                if(_CreatedById!=value){
                    _CreatedById=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedById");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _SummaryId;
        public long SummaryId
        {
            get { return _SummaryId; }
            set
            {
                if(_SummaryId!=value){
                    _SummaryId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="SummaryId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _screen_name;
        public string screen_name
        {
            get { return _screen_name; }
            set
            {
                if(_screen_name!=value){
                    _screen_name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="screen_name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _summary;
        public string summary
        {
            get { return _summary; }
            set
            {
                if(_summary!=value){
                    _summary=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="summary");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataCommentView, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataFeed table in the odaf Database.
    /// </summary>
    public partial class PointDataFeed: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataFeed> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataFeed>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataFeed> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataFeed item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataFeed item=new PointDataFeed();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataFeed> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataFeed(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataFeed.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataFeed>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataFeed(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataFeed(Expression<Func<PointDataFeed, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataFeed> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataFeed> _repo;
            
            if(db.TestMode){
                PointDataFeed.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataFeed>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataFeed> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataFeed SingleOrDefault(Expression<Func<PointDataFeed, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataFeed single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataFeed SingleOrDefault(Expression<Func<PointDataFeed, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataFeed single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataFeed, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataFeed, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataFeed> Find(Expression<Func<PointDataFeed, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataFeed> Find(Expression<Func<PointDataFeed, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataFeed> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataFeed> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataFeed> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataFeed> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataFeed> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataFeed> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "PointDataFeedId";
        }

        public object KeyValue()
        {
            return this.PointDataFeedId;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<int>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.UniqueId.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataFeed)){
                PointDataFeed compare=(PointDataFeed)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        
        public override int GetHashCode() {
            return this.PointDataFeedId;
        }
        
        public string DescriptorValue()
        {
            return this.UniqueId.ToString();
        }

        public string DescriptorColumn() {
            return "UniqueId";
        }
        public static string GetKeyColumn()
        {
            return "PointDataFeedId";
        }        
        public static string GetDescriptorColumn()
        {
            return "UniqueId";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<PointDataSource> PointDataSources
        {
            get
            {
                
                  var repo=ODAF.Data.PointDataSource.GetRepo();
                  return from items in repo.GetAll()
                       where items.PointDataSourceId == _PointDataSourceId
                       select items;
            }
        }

        #endregion
        

        int _PointDataSourceId;
        public int PointDataSourceId
        {
            get { return _PointDataSourceId; }
            set
            {
                if(_PointDataSourceId!=value){
                    _PointDataSourceId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataSourceId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _PointDataFeedId;
        public int PointDataFeedId
        {
            get { return _PointDataFeedId; }
            set
            {
                if(_PointDataFeedId!=value){
                    _PointDataFeedId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataFeedId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _UniqueId;
        public string UniqueId
        {
            get { return _UniqueId; }
            set
            {
                if(_UniqueId!=value){
                    _UniqueId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UniqueId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if(_Title!=value){
                    _Title=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Title");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Summary;
        public string Summary
        {
            get { return _Summary; }
            set
            {
                if(_Summary!=value){
                    _Summary=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Summary");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _KMLFeedUrl;
        public string KMLFeedUrl
        {
            get { return _KMLFeedUrl; }
            set
            {
                if(_KMLFeedUrl!=value){
                    _KMLFeedUrl=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="KMLFeedUrl");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _ImageUrl;
        public string ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                if(_ImageUrl!=value){
                    _ImageUrl=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="ImageUrl");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _IsRegion;
        public bool IsRegion
        {
            get { return _IsRegion; }
            set
            {
                if(_IsRegion!=value){
                    _IsRegion=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="IsRegion");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get { return _UpdatedOn; }
            set
            {
                if(_UpdatedOn!=value){
                    _UpdatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UpdatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _Active;
        public bool Active
        {
            get { return _Active; }
            set
            {
                if(_Active!=value){
                    _Active=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Active");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataFeed, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataFeedView table in the odaf Database.
    /// </summary>
    public partial class PointDataFeedView: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataFeedView> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataFeedView>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataFeedView> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataFeedView item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataFeedView item=new PointDataFeedView();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataFeedView> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataFeedView(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataFeedView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataFeedView>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataFeedView(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataFeedView(Expression<Func<PointDataFeedView, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataFeedView> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataFeedView> _repo;
            
            if(db.TestMode){
                PointDataFeedView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataFeedView>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataFeedView> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataFeedView SingleOrDefault(Expression<Func<PointDataFeedView, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataFeedView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataFeedView SingleOrDefault(Expression<Func<PointDataFeedView, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataFeedView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataFeedView, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataFeedView, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataFeedView> Find(Expression<Func<PointDataFeedView, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataFeedView> Find(Expression<Func<PointDataFeedView, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataFeedView> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataFeedView> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataFeedView> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataFeedView> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataFeedView> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataFeedView> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "PointDataSourceId";
        }

        public object KeyValue()
        {
            return this.PointDataSourceId;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<int>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.UniqueId.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataFeedView)){
                PointDataFeedView compare=(PointDataFeedView)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        
        public override int GetHashCode() {
            return this.PointDataSourceId;
        }
        
        public string DescriptorValue()
        {
            return this.UniqueId.ToString();
        }

        public string DescriptorColumn() {
            return "UniqueId";
        }
        public static string GetKeyColumn()
        {
            return "PointDataSourceId";
        }        
        public static string GetDescriptorColumn()
        {
            return "UniqueId";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        int _PointDataSourceId;
        public int PointDataSourceId
        {
            get { return _PointDataSourceId; }
            set
            {
                if(_PointDataSourceId!=value){
                    _PointDataSourceId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataSourceId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _PointDataFeedId;
        public int PointDataFeedId
        {
            get { return _PointDataFeedId; }
            set
            {
                if(_PointDataFeedId!=value){
                    _PointDataFeedId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataFeedId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _UniqueId;
        public string UniqueId
        {
            get { return _UniqueId; }
            set
            {
                if(_UniqueId!=value){
                    _UniqueId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UniqueId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if(_Title!=value){
                    _Title=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Title");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Summary;
        public string Summary
        {
            get { return _Summary; }
            set
            {
                if(_Summary!=value){
                    _Summary=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Summary");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _KMLFeedUrl;
        public string KMLFeedUrl
        {
            get { return _KMLFeedUrl; }
            set
            {
                if(_KMLFeedUrl!=value){
                    _KMLFeedUrl=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="KMLFeedUrl");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _ImageUrl;
        public string ImageUrl
        {
            get { return _ImageUrl; }
            set
            {
                if(_ImageUrl!=value){
                    _ImageUrl=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="ImageUrl");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _IsRegion;
        public bool IsRegion
        {
            get { return _IsRegion; }
            set
            {
                if(_IsRegion!=value){
                    _IsRegion=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="IsRegion");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get { return _UpdatedOn; }
            set
            {
                if(_UpdatedOn!=value){
                    _UpdatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UpdatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _Active;
        public bool Active
        {
            get { return _Active; }
            set
            {
                if(_Active!=value){
                    _Active=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Active");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _DataSourceName;
        public string DataSourceName
        {
            get { return _DataSourceName; }
            set
            {
                if(_DataSourceName!=value){
                    _DataSourceName=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="DataSourceName");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataFeedView, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataLayer table in the odaf Database.
    /// </summary>
    public partial class PointDataLayer: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataLayer> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataLayer>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataLayer> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataLayer item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataLayer item=new PointDataLayer();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataLayer> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataLayer(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataLayer.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataLayer>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataLayer(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataLayer(Expression<Func<PointDataLayer, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataLayer> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataLayer> _repo;
            
            if(db.TestMode){
                PointDataLayer.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataLayer>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataLayer> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataLayer SingleOrDefault(Expression<Func<PointDataLayer, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataLayer single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataLayer SingleOrDefault(Expression<Func<PointDataLayer, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataLayer single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataLayer, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataLayer, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataLayer> Find(Expression<Func<PointDataLayer, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataLayer> Find(Expression<Func<PointDataLayer, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataLayer> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataLayer> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataLayer> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataLayer> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataLayer> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataLayer> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Guid.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataLayer)){
                PointDataLayer compare=(PointDataLayer)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Guid.ToString();
        }

        public string DescriptorColumn() {
            return "Guid";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Guid";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Guid;
        public string Guid
        {
            get { return _Guid; }
            set
            {
                if(_Guid!=value){
                    _Guid=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Guid");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _IsSystem;
        public bool IsSystem
        {
            get { return _IsSystem; }
            set
            {
                if(_IsSystem!=value){
                    _IsSystem=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="IsSystem");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _PointDataSourceId;
        public int PointDataSourceId
        {
            get { return _PointDataSourceId; }
            set
            {
                if(_PointDataSourceId!=value){
                    _PointDataSourceId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataSourceId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataLayer, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataLayerView table in the odaf Database.
    /// </summary>
    public partial class PointDataLayerView: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataLayerView> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataLayerView>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataLayerView> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataLayerView item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataLayerView item=new PointDataLayerView();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataLayerView> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataLayerView(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataLayerView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataLayerView>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataLayerView(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataLayerView(Expression<Func<PointDataLayerView, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataLayerView> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataLayerView> _repo;
            
            if(db.TestMode){
                PointDataLayerView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataLayerView>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataLayerView> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataLayerView SingleOrDefault(Expression<Func<PointDataLayerView, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataLayerView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataLayerView SingleOrDefault(Expression<Func<PointDataLayerView, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataLayerView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataLayerView, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataLayerView, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataLayerView> Find(Expression<Func<PointDataLayerView, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataLayerView> Find(Expression<Func<PointDataLayerView, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataLayerView> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataLayerView> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataLayerView> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataLayerView> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataLayerView> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataLayerView> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Guid.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataLayerView)){
                PointDataLayerView compare=(PointDataLayerView)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Guid.ToString();
        }

        public string DescriptorColumn() {
            return "Guid";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Guid";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Guid;
        public string Guid
        {
            get { return _Guid; }
            set
            {
                if(_Guid!=value){
                    _Guid=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Guid");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _IsSystem;
        public bool IsSystem
        {
            get { return _IsSystem; }
            set
            {
                if(_IsSystem!=value){
                    _IsSystem=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="IsSystem");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _PointDataSourceId;
        public int PointDataSourceId
        {
            get { return _PointDataSourceId; }
            set
            {
                if(_PointDataSourceId!=value){
                    _PointDataSourceId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataSourceId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int? _Summaries;
        public int? Summaries
        {
            get { return _Summaries; }
            set
            {
                if(_Summaries!=value){
                    _Summaries=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Summaries");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _DataSourceName;
        public string DataSourceName
        {
            get { return _DataSourceName; }
            set
            {
                if(_DataSourceName!=value){
                    _DataSourceName=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="DataSourceName");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataLayerView, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataSource table in the odaf Database.
    /// </summary>
    public partial class PointDataSource: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataSource> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataSource>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataSource> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataSource item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataSource item=new PointDataSource();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataSource> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataSource(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataSource.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSource>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataSource(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataSource(Expression<Func<PointDataSource, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataSource> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataSource> _repo;
            
            if(db.TestMode){
                PointDataSource.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSource>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataSource> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataSource SingleOrDefault(Expression<Func<PointDataSource, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataSource single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataSource SingleOrDefault(Expression<Func<PointDataSource, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataSource single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataSource, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataSource, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataSource> Find(Expression<Func<PointDataSource, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataSource> Find(Expression<Func<PointDataSource, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataSource> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataSource> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataSource> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataSource> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataSource> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataSource> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "PointDataSourceId";
        }

        public object KeyValue()
        {
            return this.PointDataSourceId;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<int>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.UniqueId.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataSource)){
                PointDataSource compare=(PointDataSource)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        
        public override int GetHashCode() {
            return this.PointDataSourceId;
        }
        
        public string DescriptorValue()
        {
            return this.UniqueId.ToString();
        }

        public string DescriptorColumn() {
            return "UniqueId";
        }
        public static string GetKeyColumn()
        {
            return "PointDataSourceId";
        }        
        public static string GetDescriptorColumn()
        {
            return "UniqueId";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<PointDataFeed> PointDataFeeds
        {
            get
            {
                
                  var repo=ODAF.Data.PointDataFeed.GetRepo();
                  return from items in repo.GetAll()
                       where items.PointDataSourceId == _PointDataSourceId
                       select items;
            }
        }

        #endregion
        

        int _PointDataSourceId;
        public int PointDataSourceId
        {
            get { return _PointDataSourceId; }
            set
            {
                if(_PointDataSourceId!=value){
                    _PointDataSourceId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataSourceId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _UniqueId;
        public string UniqueId
        {
            get { return _UniqueId; }
            set
            {
                if(_UniqueId!=value){
                    _UniqueId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UniqueId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if(_Title!=value){
                    _Title=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Title");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _AuthorName;
        public string AuthorName
        {
            get { return _AuthorName; }
            set
            {
                if(_AuthorName!=value){
                    _AuthorName=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="AuthorName");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _AuthorEmail;
        public string AuthorEmail
        {
            get { return _AuthorEmail; }
            set
            {
                if(_AuthorEmail!=value){
                    _AuthorEmail=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="AuthorEmail");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _BoundaryPolygon;
        public string BoundaryPolygon
        {
            get { return _BoundaryPolygon; }
            set
            {
                if(_BoundaryPolygon!=value){
                    _BoundaryPolygon=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="BoundaryPolygon");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get { return _UpdatedOn; }
            set
            {
                if(_UpdatedOn!=value){
                    _UpdatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UpdatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _Active;
        public bool Active
        {
            get { return _Active; }
            set
            {
                if(_Active!=value){
                    _Active=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Active");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if(_Description!=value){
                    _Description=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Description");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataSource, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataSourceView table in the odaf Database.
    /// </summary>
    public partial class PointDataSourceView: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataSourceView> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataSourceView>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataSourceView> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataSourceView item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataSourceView item=new PointDataSourceView();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataSourceView> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataSourceView(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataSourceView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSourceView>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataSourceView(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataSourceView(Expression<Func<PointDataSourceView, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataSourceView> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataSourceView> _repo;
            
            if(db.TestMode){
                PointDataSourceView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSourceView>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataSourceView> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataSourceView SingleOrDefault(Expression<Func<PointDataSourceView, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataSourceView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataSourceView SingleOrDefault(Expression<Func<PointDataSourceView, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataSourceView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataSourceView, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataSourceView, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataSourceView> Find(Expression<Func<PointDataSourceView, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataSourceView> Find(Expression<Func<PointDataSourceView, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataSourceView> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataSourceView> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataSourceView> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataSourceView> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataSourceView> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataSourceView> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "PointDataSourceId";
        }

        public object KeyValue()
        {
            return this.PointDataSourceId;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<int>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.UniqueId.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataSourceView)){
                PointDataSourceView compare=(PointDataSourceView)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        
        public override int GetHashCode() {
            return this.PointDataSourceId;
        }
        
        public string DescriptorValue()
        {
            return this.UniqueId.ToString();
        }

        public string DescriptorColumn() {
            return "UniqueId";
        }
        public static string GetKeyColumn()
        {
            return "PointDataSourceId";
        }        
        public static string GetDescriptorColumn()
        {
            return "UniqueId";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        int _PointDataSourceId;
        public int PointDataSourceId
        {
            get { return _PointDataSourceId; }
            set
            {
                if(_PointDataSourceId!=value){
                    _PointDataSourceId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="PointDataSourceId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _UniqueId;
        public string UniqueId
        {
            get { return _UniqueId; }
            set
            {
                if(_UniqueId!=value){
                    _UniqueId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UniqueId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if(_Title!=value){
                    _Title=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Title");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _AuthorName;
        public string AuthorName
        {
            get { return _AuthorName; }
            set
            {
                if(_AuthorName!=value){
                    _AuthorName=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="AuthorName");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _AuthorEmail;
        public string AuthorEmail
        {
            get { return _AuthorEmail; }
            set
            {
                if(_AuthorEmail!=value){
                    _AuthorEmail=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="AuthorEmail");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _BoundaryPolygon;
        public string BoundaryPolygon
        {
            get { return _BoundaryPolygon; }
            set
            {
                if(_BoundaryPolygon!=value){
                    _BoundaryPolygon=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="BoundaryPolygon");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get { return _UpdatedOn; }
            set
            {
                if(_UpdatedOn!=value){
                    _UpdatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="UpdatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        bool _Active;
        public bool Active
        {
            get { return _Active; }
            set
            {
                if(_Active!=value){
                    _Active=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Active");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if(_Description!=value){
                    _Description=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Description");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int? _Feeds;
        public int? Feeds
        {
            get { return _Feeds; }
            set
            {
                if(_Feeds!=value){
                    _Feeds=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Feeds");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int? _Layers;
        public int? Layers
        {
            get { return _Layers; }
            set
            {
                if(_Layers!=value){
                    _Layers=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Layers");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataSourceView, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataSummary table in the odaf Database.
    /// </summary>
    public partial class PointDataSummary: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataSummary> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataSummary>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataSummary> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataSummary item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataSummary item=new PointDataSummary();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataSummary> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataSummary(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataSummary.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSummary>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataSummary(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataSummary(Expression<Func<PointDataSummary, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataSummary> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataSummary> _repo;
            
            if(db.TestMode){
                PointDataSummary.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSummary>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataSummary> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataSummary SingleOrDefault(Expression<Func<PointDataSummary, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataSummary single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataSummary SingleOrDefault(Expression<Func<PointDataSummary, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataSummary single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataSummary, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataSummary, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataSummary> Find(Expression<Func<PointDataSummary, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataSummary> Find(Expression<Func<PointDataSummary, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataSummary> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataSummary> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataSummary> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataSummary> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataSummary> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataSummary> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Description.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataSummary)){
                PointDataSummary compare=(PointDataSummary)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Description.ToString();
        }

        public string DescriptorColumn() {
            return "Description";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Description";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<PointDataComment> PointDataComments
        {
            get
            {
                
                  var repo=ODAF.Data.PointDataComment.GetRepo();
                  return from items in repo.GetAll()
                       where items.SummaryId == _Id
                       select items;
            }
        }

		[ScriptIgnore]
        public IQueryable<OAuthAccount> OAuthAccounts
        {
            get
            {
                
                  var repo=ODAF.Data.OAuthAccount.GetRepo();
                  return from items in repo.GetAll()
                       where items.Id == _CreatedById
                       select items;
            }
        }

        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if(_Description!=value){
                    _Description=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Description");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        decimal _Latitude;
        public decimal Latitude
        {
            get { return _Latitude; }
            set
            {
                if(_Latitude!=value){
                    _Latitude=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Latitude");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        decimal _Longitude;
        public decimal Longitude
        {
            get { return _Longitude; }
            set
            {
                if(_Longitude!=value){
                    _Longitude=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Longitude");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _LayerId;
        public string LayerId
        {
            get { return _LayerId; }
            set
            {
                if(_LayerId!=value){
                    _LayerId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="LayerId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Tag;
        public string Tag
        {
            get { return _Tag; }
            set
            {
                if(_Tag!=value){
                    _Tag=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Tag");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _RatingCount;
        public int RatingCount
        {
            get { return _RatingCount; }
            set
            {
                if(_RatingCount!=value){
                    _RatingCount=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="RatingCount");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _RatingTotal;
        public long RatingTotal
        {
            get { return _RatingTotal; }
            set
            {
                if(_RatingTotal!=value){
                    _RatingTotal=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="RatingTotal");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _CommentCount;
        public int CommentCount
        {
            get { return _CommentCount; }
            set
            {
                if(_CommentCount!=value){
                    _CommentCount=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CommentCount");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _CreatedOn;
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _ModifiedOn;
        public DateTime? ModifiedOn
        {
            get { return _ModifiedOn; }
            set
            {
                if(_ModifiedOn!=value){
                    _ModifiedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="ModifiedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Guid;
        public string Guid
        {
            get { return _Guid; }
            set
            {
                if(_Guid!=value){
                    _Guid=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Guid");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _CreatedById;
        public long CreatedById
        {
            get { return _CreatedById; }
            set
            {
                if(_CreatedById!=value){
                    _CreatedById=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedById");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if (!_dirtyColumns.Any(x => x.Name.ToLower() == "modifiedon")) {
               this.ModifiedOn=DateTime.Now;
            }            
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            this.ModifiedOn=DateTime.Now;
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            this.ModifiedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataSummary, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the PointDataSummaryView table in the odaf Database.
    /// </summary>
    public partial class PointDataSummaryView: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<PointDataSummaryView> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<PointDataSummaryView>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<PointDataSummaryView> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(PointDataSummaryView item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                PointDataSummaryView item=new PointDataSummaryView();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<PointDataSummaryView> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public PointDataSummaryView(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                PointDataSummaryView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSummaryView>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public PointDataSummaryView(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public PointDataSummaryView(Expression<Func<PointDataSummaryView, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<PointDataSummaryView> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<PointDataSummaryView> _repo;
            
            if(db.TestMode){
                PointDataSummaryView.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<PointDataSummaryView>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<PointDataSummaryView> GetRepo(){
            return GetRepo("","");
        }
        
        public static PointDataSummaryView SingleOrDefault(Expression<Func<PointDataSummaryView, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            PointDataSummaryView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static PointDataSummaryView SingleOrDefault(Expression<Func<PointDataSummaryView, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            PointDataSummaryView single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<PointDataSummaryView, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<PointDataSummaryView, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<PointDataSummaryView> Find(Expression<Func<PointDataSummaryView, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<PointDataSummaryView> Find(Expression<Func<PointDataSummaryView, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<PointDataSummaryView> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<PointDataSummaryView> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<PointDataSummaryView> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<PointDataSummaryView> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<PointDataSummaryView> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<PointDataSummaryView> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Id";
        }

        public object KeyValue()
        {
            return this.Id;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<long>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Description.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(PointDataSummaryView)){
                PointDataSummaryView compare=(PointDataSummaryView)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Description.ToString();
        }

        public string DescriptorColumn() {
            return "Description";
        }
        public static string GetKeyColumn()
        {
            return "Id";
        }        
        public static string GetDescriptorColumn()
        {
            return "Description";
        }
        
        #region ' Foreign Keys '
        #endregion
        

        long _Id;
        public long Id
        {
            get { return _Id; }
            set
            {
                if(_Id!=value){
                    _Id=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Id");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                if(_Description!=value){
                    _Description=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Description");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        decimal _Latitude;
        public decimal Latitude
        {
            get { return _Latitude; }
            set
            {
                if(_Latitude!=value){
                    _Latitude=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Latitude");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        decimal _Longitude;
        public decimal Longitude
        {
            get { return _Longitude; }
            set
            {
                if(_Longitude!=value){
                    _Longitude=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Longitude");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _LayerId;
        public string LayerId
        {
            get { return _LayerId; }
            set
            {
                if(_LayerId!=value){
                    _LayerId=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="LayerId");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Tag;
        public string Tag
        {
            get { return _Tag; }
            set
            {
                if(_Tag!=value){
                    _Tag=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Tag");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _RatingCount;
        public int RatingCount
        {
            get { return _RatingCount; }
            set
            {
                if(_RatingCount!=value){
                    _RatingCount=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="RatingCount");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _RatingTotal;
        public long RatingTotal
        {
            get { return _RatingTotal; }
            set
            {
                if(_RatingTotal!=value){
                    _RatingTotal=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="RatingTotal");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int _CommentCount;
        public int CommentCount
        {
            get { return _CommentCount; }
            set
            {
                if(_CommentCount!=value){
                    _CommentCount=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CommentCount");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _CreatedOn;
        public DateTime? CreatedOn
        {
            get { return _CreatedOn; }
            set
            {
                if(_CreatedOn!=value){
                    _CreatedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        DateTime? _ModifiedOn;
        public DateTime? ModifiedOn
        {
            get { return _ModifiedOn; }
            set
            {
                if(_ModifiedOn!=value){
                    _ModifiedOn=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="ModifiedOn");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Guid;
        public string Guid
        {
            get { return _Guid; }
            set
            {
                if(_Guid!=value){
                    _Guid=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Guid");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        long _CreatedById;
        public long CreatedById
        {
            get { return _CreatedById; }
            set
            {
                if(_CreatedById!=value){
                    _CreatedById=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="CreatedById");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        int? _Comments;
        public int? Comments
        {
            get { return _Comments; }
            set
            {
                if(_Comments!=value){
                    _Comments=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Comments");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _screen_name;
        public string screen_name
        {
            get { return _screen_name; }
            set
            {
                if(_screen_name!=value){
                    _screen_name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="screen_name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if (!_dirtyColumns.Any(x => x.Name.ToLower() == "modifiedon")) {
               this.ModifiedOn=DateTime.Now;
            }            
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            this.ModifiedOn=DateTime.Now;
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            this.CreatedOn=DateTime.Now;
            this.ModifiedOn=DateTime.Now;
            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<PointDataSummaryView, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the UserAccess table in the odaf Database.
    /// </summary>
    public partial class UserAccess: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<UserAccess> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<UserAccess>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<UserAccess> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(UserAccess item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                UserAccess item=new UserAccess();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<UserAccess> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public UserAccess(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                UserAccess.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<UserAccess>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public UserAccess(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public UserAccess(Expression<Func<UserAccess, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<UserAccess> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<UserAccess> _repo;
            
            if(db.TestMode){
                UserAccess.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<UserAccess>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<UserAccess> GetRepo(){
            return GetRepo("","");
        }
        
        public static UserAccess SingleOrDefault(Expression<Func<UserAccess, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            UserAccess single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static UserAccess SingleOrDefault(Expression<Func<UserAccess, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            UserAccess single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<UserAccess, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<UserAccess, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<UserAccess> Find(Expression<Func<UserAccess, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<UserAccess> Find(Expression<Func<UserAccess, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<UserAccess> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<UserAccess> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<UserAccess> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<UserAccess> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<UserAccess> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<UserAccess> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Code";
        }

        public object KeyValue()
        {
            return this.Code;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<short>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Name.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(UserAccess)){
                UserAccess compare=(UserAccess)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Name.ToString();
        }

        public string DescriptorColumn() {
            return "Name";
        }
        public static string GetKeyColumn()
        {
            return "Code";
        }        
        public static string GetDescriptorColumn()
        {
            return "Name";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<OAuthAccount> OAuthAccounts
        {
            get
            {
                
                  var repo=ODAF.Data.OAuthAccount.GetRepo();
                  return from items in repo.GetAll()
                       where items.UserAccess == _Code
                       select items;
            }
        }

        #endregion
        

        short _Code;
        public short Code
        {
            get { return _Code; }
            set
            {
                if(_Code!=value){
                    _Code=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Code");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<UserAccess, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
    
    
    /// <summary>
    /// A class which represents the UserRole table in the odaf Database.
    /// </summary>
    public partial class UserRole: IActiveRecord
    {
    
        #region Built-in testing
        static TestRepository<UserRole> _testRepo;
        

        
        static void SetTestRepo(){
            _testRepo = _testRepo ?? new TestRepository<UserRole>(new ODAF.Data.odafDB());
        }
        public static void ResetTestRepo(){
            _testRepo = null;
            SetTestRepo();
        }
        public static void Setup(List<UserRole> testlist){
            SetTestRepo();
            _testRepo._items = testlist;
        }
        public static void Setup(UserRole item) {
            SetTestRepo();
            _testRepo._items.Add(item);
        }
        public static void Setup(int testItems) {
            SetTestRepo();
            for(int i=0;i<testItems;i++){
                UserRole item=new UserRole();
                _testRepo._items.Add(item);
            }
        }
        
        [ScriptIgnore]
        public bool TestMode = false;


        #endregion

        IRepository<UserRole> _repo;
        ITable tbl;
        bool _isNew;
        public bool IsNew(){
            return _isNew;
        }
        
        public void SetIsLoaded(bool isLoaded){
            _isLoaded=isLoaded;
            if(isLoaded)
                OnLoaded();
        }
        
        public void SetIsNew(bool isNew){
            _isNew=isNew;
        }
        bool _isLoaded;
        public bool IsLoaded(){
            return _isLoaded;
        }
                
        List<IColumn> _dirtyColumns;
        public bool IsDirty(){
            return _dirtyColumns.Count>0;
        }
        
        public List<IColumn> GetDirtyColumns (){
            return _dirtyColumns;
        }

        ODAF.Data.odafDB _db;
        public UserRole(string connectionString, string providerName) {

            _db=new ODAF.Data.odafDB(connectionString, providerName);
            Init();            
         }
        void Init(){
            TestMode=this._db.DataProvider.ConnectionString.Equals("test", StringComparison.InvariantCultureIgnoreCase);
            _dirtyColumns=new List<IColumn>();
            if(TestMode){
                UserRole.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<UserRole>(_db);
            }
            tbl=_repo.GetTable();
            SetIsNew(true);
            OnCreated();       

        }
        
        public UserRole(){
             _db=new ODAF.Data.odafDB();
            Init();            
        }
        
       
        partial void OnCreated();
            
        partial void OnLoaded();
        
        partial void OnSaved();
        
        partial void OnChanged();
        
        [ScriptIgnore]
        public IList<IColumn> Columns{
            get{
                return tbl.Columns;
            }
        }

        public UserRole(Expression<Func<UserRole, bool>> expression):this() {

            SetIsLoaded(_repo.Load(this,expression));
        }
        
       
        
        internal static IRepository<UserRole> GetRepo(string connectionString, string providerName){
            ODAF.Data.odafDB db;
            if(String.IsNullOrEmpty(connectionString)){
                db=new ODAF.Data.odafDB();
            }else{
                db=new ODAF.Data.odafDB(connectionString, providerName);
            }
            IRepository<UserRole> _repo;
            
            if(db.TestMode){
                UserRole.SetTestRepo();
                _repo=_testRepo;
            }else{
                _repo = new SubSonicRepository<UserRole>(db);
            }
            return _repo;        
        }       
        
        internal static IRepository<UserRole> GetRepo(){
            return GetRepo("","");
        }
        
        public static UserRole SingleOrDefault(Expression<Func<UserRole, bool>> expression) {

            var repo = GetRepo();
            var results=repo.Find(expression);
            UserRole single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
                single.OnLoaded();
                single.SetIsLoaded(true);
                single.SetIsNew(false);
            }

            return single;
        }      
        
        public static UserRole SingleOrDefault(Expression<Func<UserRole, bool>> expression,string connectionString, string providerName) {
            var repo = GetRepo(connectionString,providerName);
            var results=repo.Find(expression);
            UserRole single=null;
            if(results.Count() > 0){
                single=results.ToList()[0];
            }

            return single;


        }
        
        
        public static bool Exists(Expression<Func<UserRole, bool>> expression,string connectionString, string providerName) {
           
            return All(connectionString,providerName).Any(expression);
        }        
        public static bool Exists(Expression<Func<UserRole, bool>> expression) {
           
            return All().Any(expression);
        }        

        public static IList<UserRole> Find(Expression<Func<UserRole, bool>> expression) {
            
            var repo = GetRepo();
            return repo.Find(expression).ToList();
        }
        
        public static IList<UserRole> Find(Expression<Func<UserRole, bool>> expression,string connectionString, string providerName) {

            var repo = GetRepo(connectionString,providerName);
            return repo.Find(expression).ToList();

        }
        public static IQueryable<UserRole> All(string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetAll();
        }
        public static IQueryable<UserRole> All() {
            return GetRepo().GetAll();
        }
        
        public static PagedList<UserRole> GetPaged(string sortBy, int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(sortBy, pageIndex, pageSize);
        }
      
        public static PagedList<UserRole> GetPaged(string sortBy, int pageIndex, int pageSize) {
            return GetRepo().GetPaged(sortBy, pageIndex, pageSize);
        }

        public static PagedList<UserRole> GetPaged(int pageIndex, int pageSize,string connectionString, string providerName) {
            return GetRepo(connectionString,providerName).GetPaged(pageIndex, pageSize);
            
        }


        public static PagedList<UserRole> GetPaged(int pageIndex, int pageSize) {
            return GetRepo().GetPaged(pageIndex, pageSize);
            
        }

        public string KeyName()
        {
            return "Code";
        }

        public object KeyValue()
        {
            return this.Code;
        }
        
        public void SetKeyValue(object value) {
            if (value != null && value!=DBNull.Value) {
                var settable = value.ChangeTypeTo<short>();
                this.GetType().GetProperty(this.KeyName()).SetValue(this, settable, null);
            }
        }
        
        public override string ToString(){
            return this.Name.ToString();
        }

        public override bool Equals(object obj){
            if(obj.GetType()==typeof(UserRole)){
                UserRole compare=(UserRole)obj;
                return compare.KeyValue()==this.KeyValue();
            }else{
                return base.Equals(obj);
            }
        }

        public string DescriptorValue()
        {
            return this.Name.ToString();
        }

        public string DescriptorColumn() {
            return "Name";
        }
        public static string GetKeyColumn()
        {
            return "Code";
        }        
        public static string GetDescriptorColumn()
        {
            return "Name";
        }
        
        #region ' Foreign Keys '
		[ScriptIgnore]
        public IQueryable<OAuthAccount> OAuthAccounts
        {
            get
            {
                
                  var repo=ODAF.Data.OAuthAccount.GetRepo();
                  return from items in repo.GetAll()
                       where items.UserRole == _Code
                       select items;
            }
        }

        #endregion
        

        short _Code;
        public short Code
        {
            get { return _Code; }
            set
            {
                if(_Code!=value){
                    _Code=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Code");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if(_Name!=value){
                    _Name=value;
                    var col=tbl.Columns.SingleOrDefault(x=>x.Name=="Name");
                    if(col!=null){
                        if(!_dirtyColumns.Any(x=>x.Name==col.Name) && _isLoaded){
                            _dirtyColumns.Add(col);
                        }
                    }
                    OnChanged();
                }
            }
        }



        public DbCommand GetUpdateCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToUpdateQuery(_db.Provider).GetCommand().ToDbCommand();
            
        }
        public DbCommand GetInsertCommand() {
 
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToInsertQuery(_db.Provider).GetCommand().ToDbCommand();
        }
        
        public DbCommand GetDeleteCommand() {
            if(TestMode)
                return _db.DataProvider.CreateCommand();
            else
                return this.ToDeleteQuery(_db.Provider).GetCommand().ToDbCommand();
        }
       
        
        public void Update(){
            Update(_db.DataProvider);
        }
        
        public void Update(IDataProvider provider){
        
            
            if(this._dirtyColumns.Count>0)
                _repo.Update(this,provider);
            OnSaved();
       }
 
        public void Add(){
            Add(_db.DataProvider);
        }
        
        
       
        public void Add(IDataProvider provider){

            
            var key=KeyValue();
            if(key==null){
                var newKey=_repo.Add(this,provider);
                this.SetKeyValue(newKey);
            }else{
                _repo.Add(this,provider);
            }
            SetIsNew(false);
            OnSaved();
        }
        
                
        
        public void Save() {
            Save(_db.DataProvider);
        }      
        public void Save(IDataProvider provider) {
            
           
            if (_isNew) {
                Add(provider);
                
            } else {
                Update(provider);
            }
            
        }

        

        public void Delete(IDataProvider provider) {
                   
                 
            _repo.Delete(KeyValue());
            
                    }


        public void Delete() {
            Delete(_db.DataProvider);
        }


        public static void Delete(Expression<Func<UserRole, bool>> expression) {
            var repo = GetRepo();
            
       
            
            repo.DeleteMany(expression);
            
        }

        

        public void Load(IDataReader rdr) {
            Load(rdr, true);
        }
        public void Load(IDataReader rdr, bool closeReader) {
            if (rdr.Read()) {

                try {
                    rdr.Load(this);
                    SetIsNew(false);
                    SetIsLoaded(true);
                } catch {
                    SetIsLoaded(false);
                    throw;
                }
            }else{
                SetIsLoaded(false);
            }

            if (closeReader)
                rdr.Dispose();
        }
        

    } 
}
