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
using datagridviewtemplate.DBConnectors;
using datagridviewtemplate.Dependencies;
using datagridviewtemplate.Repositories;
using datagridviewtemplate.Models;
using System.Reflection;

namespace datagridviewtemplate
{
    public partial class Form1 : Form
    {
        DGVDBConnector DbConnector;
        SqlDataAdapter Da;

        Paginator Pagination;
        Queries query;

        EmployeeRepository EmployeeRepo;

        DataTable searchResultsDT = new DataTable();
        BindingList<Employee> BLEmployees;

        public Form1()
        {
            string connectionString = @"server=DESKTOP-QPM5S0C\SQLEXPRESS;Trusted_Connection=yes;database=DGVDB;connection timeout=30";
            DbConnector = new DGVDBConnector(connectionString);
            Da = new SqlDataAdapter();
            query = new Queries(connectionString);

            EmployeeRepo = new EmployeeRepository(DbConnector);
            BLEmployees = null;

            Pagination = new Paginator();   // use defaults
            Pagination.PageSize = 3;

            InitializeComponent();

            populatePositionCombo();

            Pagination.CalculateTotalPages(getEmployeesCount());
            Pagination.CurrentPageIndex = 1;    // go to page where selected ledger was on

            dataGridView1.AutoGenerateColumns = false;
            populateDGV();

            //JumpToPage(Pagination.CurrentPageIndex); // go to paginator page where row is on
            btnLastPage.Enabled = false;
            btnFirstPage.Enabled = false;
            btnNextPage.Enabled = false;
            btnPreviousPage.Enabled = false;
        }

        private void populateDGV() {
            BLEmployees = EmployeeRepo.GetAll();

            BindingSource BSEmployee = new BindingSource(BLEmployees, null);
            dataGridView1.DataSource = BSEmployee;


            /*if (query.GetAllEmployees() != null)
            {
                Da.SelectCommand = query.GetAllEmployees();
                Da.Fill(searchResultsDT);

                dataGridView1.DataSource = searchResultsDT;
            }*/
        }

        private void JumpToPage(int pageNumber)
        {
            if (Pagination.CurrentPageIndex <= Pagination.TotalPage && pageNumber <= Pagination.TotalPage) // when not last page
            {
                Pagination.CurrentPageIndex = pageNumber;   // move up n page(s)

                if (query.GetPaginatedEmployees(Pagination) != null)  // get filtered result
                {
                    Da.SelectCommand = query.GetPaginatedEmployees(Pagination);
                    searchResultsDT.Clear();

                    Da.Fill(searchResultsDT);   // populate data table
                    dataGridView1.DataSource = searchResultsDT;   // bind to DGV

                    UpdatePaginationLabel(Pagination.CurrentPageIndex, Pagination.TotalPage);    // update pg of pages label
                }
            }
        }

        private void UpdatePaginationLabel(int currentPageIndex, int totalPage)
        {
            lblCurrentPage.Text = "page: " + currentPageIndex + " of " + (totalPage == 0 ? "1" : totalPage.ToString());
        }

        private void setPositionOptions() {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                DataGridViewComboBoxCell comboPosition;

                comboPosition = (DataGridViewComboBoxCell)row.Cells["CmbPosition"];
                comboPosition.Value = row.Cells["TxtPositionReference"].Value;
            }
        }

        private void populatePositionCombo() {
            if (query.getPositions() != null)
            {
                
                Da.SelectCommand = query.getPositions();
                DataTable dt = new DataTable();
                Da.Fill(dt);

                DataGridViewComboBoxColumn comboColumn = (DataGridViewComboBoxColumn)dataGridView1.Columns["CmbPosition"];
                comboColumn.DisplayMember = "positionName";
                comboColumn.ValueMember = "positionID";
                comboColumn.DataSource = dt;
            }
        }

        private int getEmployeesCount() {
            try
            {
                DbConnector.OpenConnection();

                if (DbConnector.IsOpen) { 
                    string sql = @"SELECT COUNT(*) FROM [dbo].[View_Employees]";
                    using (SqlCommand tblCount = new SqlCommand(sql, DbConnector.GetConnection))
                    {
                        Int32 count = (Int32)tblCount.ExecuteScalar();
                        DbConnector.CloseConnection();
                        return count;
                    }                
                }
                return 0;
            }
            catch (Exception ex)
            {
                DbConnector.CloseConnection();
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            searchResultsDT.Clear();
            JumpToPage(1);
            setPositionOptions();
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (Pagination.CurrentPageIndex > 1)    // when not on page 1   
            {
                Pagination.CurrentPageIndex -= 1;   // move 1 page back 
                searchResultsDT.Clear();    // empty data table to accomodate new seach results

                JumpToPage(Pagination.CurrentPageIndex);
                setPositionOptions();
            }
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            if (Pagination.CurrentPageIndex < Pagination.TotalPage) // not last page
            {
                Pagination.CurrentPageIndex = Pagination.TotalPage; // move to last page
                searchResultsDT.Clear();    // empty data table to accomodate new seach results

                JumpToPage(Pagination.CurrentPageIndex);
                setPositionOptions();
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (Pagination.CurrentPageIndex < Pagination.TotalPage) // when not last page
            {
                Pagination.CurrentPageIndex += 1;   // move up 1 page
                searchResultsDT.Clear();    // empty data table to accomodate new seach results

                JumpToPage(Pagination.CurrentPageIndex);
                setPositionOptions();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setPositionOptions();
        }



        #region DATA GRID DRAG/DROP FUNCTIONALITY

        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        private void REDFrmRecordManagementEntryDGVRecordEntry_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dataGridView1.DoDragDrop(
                    dataGridView1.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void REDFrmRecordManagementEntryDGVRecordEntry_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                                               dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void REDFrmRecordManagementEntryDGVRecordEntry_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void REDFrmRecordManagementEntryDGVRecordEntry_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(
                    typeof(DataGridViewRow)) as DataGridViewRow;

                Employee oldEmployee = (Employee)rowToMove.DataBoundItem;
                Employee newEmployee = oldEmployee;

                if (rowIndexOfItemUnderMouseToDrop < 0 || rowIndexOfItemUnderMouseToDrop == dataGridView1.RowCount - 1)
                {
                    return;
                }
                #region datatable updates login on row drag drop
                BLEmployees.Remove(oldEmployee);
                BLEmployees.Insert(rowIndexOfItemUnderMouseToDrop, newEmployee);


                //setSelectedRecordDropDownValues(rowIndexOfItemUnderMouseToDrop);    // set the selected value in combo box since dt row doesn't bind the comboboxes
                //setAllReadOnlyRows();
                //hasDGVCellUpdated = true;   // DGV cell data has been modified
                #endregion
            }
        }

        private void setSelectedRecordDropDownValues(int rowIdx)
        {
            DataGridViewRowCollection rows = dataGridView1.Rows;

            //string positionValue = rows[rowIdx].Cells["TxtPositionReference"].Value.ToString();   // get the hidden preloaded value

            //rows[rowIdx].Cells["CmbPosition"].Value = positionValue;


            DataGridViewComboBoxCell comboPosition;

            comboPosition = (DataGridViewComboBoxCell)rows[rowIdx].Cells["CmbPosition"];
            comboPosition.Value = rows[rowIdx].Cells["TxtPositionReference"].Value;

        }
        #endregion

        public T ToObject<T>(DataRow dataRow)
        where T : new()
        {
            T item = new T();
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                if (dataRow[column] != DBNull.Value)
                {
                    PropertyInfo prop = item.GetType().GetProperty(column.ColumnName);
                    if (prop != null)
                    {
                        object result = Convert.ChangeType(dataRow[column], prop.PropertyType);
                        prop.SetValue(item, result, null);
                        continue;
                    }
                    else
                    {
                        FieldInfo fld = item.GetType().GetField(column.ColumnName);
                        if (fld != null)
                        {
                            object result = Convert.ChangeType(dataRow[column], fld.FieldType);
                            fld.SetValue(item, result);
                        }
                    }
                }
            }
            return item;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // save the state of the dgv and its values
            EmployeeRepo.SaveAll(BLEmployees);



        }
    }
}
