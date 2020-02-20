using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace datagridviewtemplate
{
    public partial class Form1 : Form
    {
        SqlConnection con;

        public Form1()
        {
            con = new SqlConnection(@"server=DESKTOP-QPM5S0C\SQLEXPRESS;Trusted_Connection=yes;database=DGVDB;connection timeout=30");

            InitializeComponent();

            // populateEmployeesTable();
            CalculateTotalPages();
            populatePositionCombo();
            setPositionOptions();

            btnFirstPage.PerformClick();
        }
        private void setPositionOptions() {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                DataGridViewComboBoxCell comboPosition = (DataGridViewComboBoxCell)row.Cells["CmbPosition"];
                comboPosition.Value = 1;
            }
        }

        private void populatePositionCombo() {
            if (getPositions() != null)
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = getPositions();
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataGridViewComboBoxColumn comboColumn = (DataGridViewComboBoxColumn)dataGridView1.Columns["CmbPosition"];
                comboColumn.DisplayMember = "positionName";
                comboColumn.ValueMember = "positionID";
                comboColumn.DataSource = dt;
            }
        }
        /*private void populateEmployeesTable() {
            if (getEmployees() != null) {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = getEmployees();
                dtBS = new DataTable();
                da.Fill(dtBS);
                dataGridView1.DataSource = dtBS;
            }
        }*/

        private SqlCommand getPositions()
        {
            try
            {
                con.Open();
                string sql = @"SELECT [positionID],[positionName] FROM [dbo].[position]";
                using (SqlCommand positions = new SqlCommand(sql, con))
                {
                    con.Close();
                    return positions;
                }
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private int getEmployeesCount() {
            try
            {
                con.Open();
                string sql = @"SELECT COUNT(*) FROM [dbo].[View_Employees]";
                using (SqlCommand tblCount = new SqlCommand(sql, con))
                {
                    Int32 count = (Int32)tblCount.ExecuteScalar();
                    con.Close();
                    return count;
                }
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private SqlCommand getEmployees() {
            try {
                con.Open();
                string sql = @"SELECT [EmployeeID],[FirstName],[LastName] FROM [dbo].[View_Employees]";
                using (SqlCommand employees = new SqlCommand(sql, con))
                {
                    con.Close();
                    return employees;
                }
            }
            catch (Exception ex) {
                con.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private int PgSize = 10;
        private int CurrentPageIndex = 1;
        private int TotalPage = 0;

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            CurrentPageIndex = 1;
            dataGridView1.DataSource = GetCurrentRecords(CurrentPageIndex, con);
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (CurrentPageIndex > 1)
            {
                CurrentPageIndex--;
                dataGridView1.DataSource = GetCurrentRecords(CurrentPageIndex, con);
            }
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            CurrentPageIndex = TotalPage;
            dataGridView1.DataSource = GetCurrentRecords(CurrentPageIndex, con);
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (CurrentPageIndex < TotalPage)
            {
                CurrentPageIndex++;
                dataGridView1.DataSource = GetCurrentRecords(CurrentPageIndex, con);
            }
        }
        private DataTable GetCurrentRecords(int page, SqlConnection con)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd2;
            if (page == 1)
            {
                cmd2 = new SqlCommand("Select TOP " + PgSize +
                " * from [dbo].[View_Employees] ORDER BY EmployeeID", con);
            }
            else
            {
                int PreviousPageOffSet = (page - 1) * PgSize;

                cmd2 = new SqlCommand("Select TOP " + PgSize +
                    " * from [dbo].[View_Employees] WHERE EmployeeID NOT IN " +
                    "(Select TOP " + PreviousPageOffSet +
            " EmployeeID from [dbo].[View_Employees] ORDER BY EmployeeID) ", con);
            }
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd2;
                da.Fill(dt);
            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        private void CalculateTotalPages()
        {
            if (getEmployees() != null)
            {
                /*SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = getEmployees();
                DataTable dt = new DataTable();
                da.Fill(dt);*/

                // int rowCount = dt.Rows.Count;
                int rowCount = getEmployeesCount();
                TotalPage = rowCount / PgSize;
                // if any row left after calculated pages, add one more page 
                if (rowCount % PgSize > 0)
                    TotalPage += 1;
            }
        }
    }
}
