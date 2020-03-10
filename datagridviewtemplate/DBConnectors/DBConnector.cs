using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;

namespace datagridviewtemplate.DBConnectors
{
    public abstract class DBConnector : IDbConnector
    {
        protected string _ConnectionString;

        protected SqlConnection _MyConn;
        protected bool _Open = false;

        public string ConnectionString { get => _ConnectionString; set => _ConnectionString = value; }
        public SqlConnection GetConnection { get => _MyConn; }
        public bool IsOpen { get => _Open; }

        public DBConnector()
        {
            _ConnectionString = null;
        }

        #region DEFAULT IMPLEMENTATIONS
        public virtual void OpenConnection()
        {
            if (!_Open)
            {
                _MyConn.Open();
                _Open = true;
            }
        }
        public virtual void CloseConnection()
        {
            if (_Open)
            {
                _MyConn.Close();
                _Open = false;
            }
        }
        public virtual void Connect()
        {
            try
            {
                _MyConn = new SqlConnection(_ConnectionString);
            }
            catch (Exception)
            {
                MessageBox.Show("Database Connection Failed, please contact Systems Amdministrators");
            }
        }
        public virtual void GetConnected()
        {
            try
            {
                _MyConn = new SqlConnection(_ConnectionString);
            }
            catch (Exception)
            {
                MessageBox.Show("Database Connection Failed, please contact Systems Amdministrators");

            }
        }
        public virtual SqlDataReader DataReader(String query)
        {
            SqlCommand cmd = new SqlCommand(query, _MyConn);
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }
        public virtual int ExecuteQueries(string query)
        {
            SqlCommand cmd = new SqlCommand(query, _MyConn);
            int rows = cmd.ExecuteNonQuery();
            return rows;
        }
        public virtual Object ShowDataInGrid(string query)
        {
            SqlDataAdapter da = new SqlDataAdapter(query, _ConnectionString);
            DataSet ds = new DataSet();
            da.Fill(ds);
            object data = ds.Tables[0];
            return data;
        }
        public virtual DataTable CreateDataTable(string query, string TableName = "")
        {
            DataTable table = new DataTable(TableName);
            SqlDataReader dr = DataReader(query);
            table.Load(dr);
            dr.Close();
            return table;
        }
        #endregion
    }
}
