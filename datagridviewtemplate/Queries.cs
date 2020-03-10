using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using datagridviewtemplate.DBConnectors;
using datagridviewtemplate.Dependencies;

namespace datagridviewtemplate
{
    class Queries
    {
        private string ConnString;
        DBConnectors.DGVDBConnector Db;

        public Queries(string connString)
        {
            ConnString = connString;
            Db = new DGVDBConnector(ConnString);
        }

        public SqlCommand getPositions()
        {
            try
            {
                Db.OpenConnection();

                if (Db.IsOpen)
                {
                    string sql = @"SELECT [positionID],[positionName] FROM [dbo].[position]";
                    using (SqlCommand positions = new SqlCommand(sql, Db.GetConnection))
                    {
                        Db.CloseConnection();
                        return positions;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Db.CloseConnection();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public SqlCommand GetAllEmployees() {
            string sql = "SELECT * FROM [dbo].[View_Employees] ORDER BY EmployeeID";
            try
            {
                Db.OpenConnection();
                if (Db.IsOpen)
                {
                    using (SqlCommand records = new SqlCommand(sql, Db.GetConnection))
                    {
                        Db.CloseConnection();
                        return records;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Db.CloseConnection();
            }
            return null;
        }

        public SqlCommand GetPaginatedEmployees(Paginator paginator) {
            string sql = "";

            if (paginator.CurrentPageIndex == 1)
            {
                sql = "Select TOP " + paginator.PageSize + " * from [dbo].[View_Employees] ORDER BY EmployeeID";
            }
            else {
                int PreviousPageOffSet = (paginator.CurrentPageIndex - 1) * paginator.PageSize;
                sql ="Select TOP " + paginator.PageSize +
                    " * from [dbo].[View_Employees] WHERE EmployeeID NOT IN " +
                    "(Select TOP " + PreviousPageOffSet +
                    " EmployeeID from [dbo].[View_Employees] ORDER BY EmployeeID)  ORDER BY EmployeeID";
            }

            try
            {
                Db.OpenConnection();
                if (Db.IsOpen)
                {
                    using (SqlCommand filteredRecords = new SqlCommand(sql, Db.GetConnection))
                    {
                        Db.CloseConnection();
                        return filteredRecords;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Db.CloseConnection();
            }
            return null;
        }
    }
}
