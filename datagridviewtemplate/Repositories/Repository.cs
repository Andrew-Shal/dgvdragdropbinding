using datagridviewtemplate.DBConnectors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datagridviewtemplate.Repositories
{
    public abstract class Repository<T>
        : IRepository<T> where T : class
    {
        // database connection object
        protected DBConnector _Db;
        protected SqlDataAdapter _Da;
        public DBConnector Db { get => _Db; set => _Db = value; }

        public Repository()
        {
            // set accessors
            _Db = null; // set injected implementation of db connection
            _Da = new SqlDataAdapter();
        }

        public abstract string Delete(string key);
        public abstract BindingList<T> GetAll();
        public abstract T GetById(string key);
        public abstract int Insert(T obj);
        public abstract int Update(T obj);
    }
}
