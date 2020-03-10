using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datagridviewtemplate.DBConnectors
{
    interface IDbConnector
    {
        void OpenConnection();
        void CloseConnection();
        void Connect();
        SqlDataReader DataReader(string query);
        int ExecuteQueries(string query);
        Object ShowDataInGrid(string query);
        DataTable CreateDataTable(string query, string tableName);
    }
}
