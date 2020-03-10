using datagridviewtemplate.DBConnectors;
using datagridviewtemplate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace datagridviewtemplate.Repositories
{
    class EmployeeRepository : Repository<Employee>
    {

        public EmployeeRepository(DGVDBConnector dGVDB) : base()
        {
            _Db = dGVDB;
        }

        public override string Delete(string key)
        {
            throw new NotImplementedException();
        }

        public override BindingList<Employee> GetAll()
        {
            try
            {
                _Db.OpenConnection();
                if (_Db.IsOpen)
                {
                    string sql = @"SELECT * FROM [dbo].[View_Employees] WHERE [IsRemoved] = 0 ORDER BY [SortedOrder] ASC";

                    using (_Db.GetConnection)
                    {
                        var list = new List<Employee>();
                        _Da.SelectCommand = new SqlCommand(sql, _Db.GetConnection);
                        DataTable dt = new DataTable();
                        _Da.Fill(dt);

                        if (dt.Rows.Count < 1) return null;

                        foreach (DataRow row in dt.AsEnumerable())
                        {
                            Employee record = new Employee(
                                row.Field<int>("EmployeeID"),
                                row.Field<string>("FirstName"),
                                row.Field<string>("LastName"),
                                row.Field<string>("positionname"),
                                row.Field<int>("PositionReference"));

                            list.Add(record);
                        }

                        _Db.CloseConnection();
                        return new BindingList<Employee>(list);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _Db.CloseConnection();
                return null;
            }
        }

        public override Employee GetById(string key)
        {
            throw new NotImplementedException();
        }

        public override int Insert(Employee obj) // returns Employee ID
        {
            throw new NotImplementedException();
        }

        public override int Update(Employee obj)
        {
            throw new NotImplementedException();
        }

        public void SaveAll(BindingList<Employee> bLEmployees)
        {
            try
            {
                for (int i = 0; i < bLEmployees.Count; i++) {
                    int employeeID = -1;
                    Employee temp = bLEmployees[i];
                    if (temp.EmployeeID == -1) // this is a new employee added from dgv
                    {
                        // call create procedure
                        employeeID = Insert(bLEmployees[i]);
                    }
                    else { 
                        // call update procedure
                        if(temp.IsModified == true) // we know that a data property of this employee was modified
                        {
                            // we update this employee
                            employeeID = Update(bLEmployees[i]);
                        }
                    }
                    // call internal method to update the sort order for record entry
                    setEmployeeSortOrder(i+1, employeeID != -1 ? employeeID : bLEmployees[i].EmployeeID);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                _Db.CloseConnection();
            }
        }

        private void setEmployeeSortOrder(int idx, int employeeID)
        {
            return;
            // set sort order for record entry item
            string Procedure = "[dbo].[EmployeeSortOrder]";
            try
            {
                _Db.OpenConnection();
                if (_Db.IsOpen)
                {
                    // Prepare to call procedure
                    SqlCommand updateSortOrder = new SqlCommand(Procedure, _Db.GetConnection);
                    updateSortOrder.CommandType = CommandType.StoredProcedure;

                    // Parameters for the procedure
                    updateSortOrder.Parameters.Add(new SqlParameter("@employeeID", employeeID));
                    updateSortOrder.Parameters.Add(new SqlParameter("@sortOrder", idx));

                    //updateSortOrder.Parameters.Add(new SqlParameter("@isSuccess", System.Data.SqlDbType.Bit)).Direction = ParameterDirection.Output;
                    //updateSortOrder.Parameters.Add(new SqlParameter("@warning", System.Data.SqlDbType.NVarChar, 200)).Direction = ParameterDirection.Output;

                    updateSortOrder.ExecuteNonQuery();

                    //bool isSuccess = Convert.ToBoolean(updateSortOrder.Parameters["@isSuccess"].Value);
                    //string warning = Convert.ToString(updateSortOrder.Parameters["@warning"].Value);

                   // if (!isSuccess)
                   // {
                    //    MessageBox.Show("An error occured while saving the record, please contact Administration. Error Msg:" + warning);
                    //}
                    _Db.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _Db.CloseConnection();
            }
        }
    }
}
