using semigura.DBContext.Entities;
using System.Diagnostics;

namespace semigura.DAL
{
    public class MyFileRepository : IMyFileRepository, IDisposable
    {
        private DBEntities db;
        private IConfiguration _configuration;

        public MyFileRepository(DBEntities context, IConfiguration configuration)
        {
            this.db = context;
            _configuration = configuration;

        }

        public IQueryable<MyFile> GetAll()
        {
            return db.MyFiles;
        }
        public string GetUploadPath()
        {
            return _configuration.GetSection("FileUpload")["Location"];
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    //db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public MyFile GetByName(string fileName)
        {
            try
            {
                var file = db.MyFiles.Where(f => f.LocalPath == fileName).FirstOrDefault();
                return file;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public MyFile GetById(string id)
        {
            try
            {
                var file = db.MyFiles.Find(id);
                return file;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public void Add(MyFile myFile)
        {
            db.MyFiles.Add(myFile);
            db.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await db.SaveChangesAsync();
        }
        public async Task<MyFile> FindAsync(string id)
        {
            return await db.MyFiles.FindAsync(id);
        }
        public MyFile Remove(MyFile myFile)
        {
            db.MyFiles.Remove(myFile);
            return myFile;
        }
        public bool Exists(string id)
        {
            return db.MyFiles.Count(e => e.Id == id) > 0;
        }
    }
}
