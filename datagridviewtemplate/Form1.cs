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
        SqlConnection myConnection;
        DataTable dtBS;

        public Form1()
        {
            myConnection = new SqlConnection(@"server=DESKTOP-QPM5S0C\SQLEXPRESS;Trusted_Connection=yes;database=DGVDB;connection timeout=30");

            InitializeComponent();

            populateEmployeesTable();
            populatePositionCombo();
            setPositionOptions();
        }
        private void setPositionOptions() {
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                DataGridViewComboBoxCell comboPosition = (DataGridViewComboBoxCell)row.Cells["CmbPosition"];
                comboPosition.Value = 3;
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
        private void populateEmployeesTable() {
            if (getEmployees() != null) {
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = getEmployees();
                dtBS = new DataTable();
                da.Fill(dtBS);
                dataGridView1.DataSource = dtBS;
            }
        }
        private SqlCommand getPositions()
        {
            try
            {
                myConnection.Open();
                string sql = @"SELECT [positionID],[positionName] FROM [dbo].[position]";
                using (SqlCommand positions = new SqlCommand(sql, myConnection))
                {
                    myConnection.Close();
                    return positions;
                }
            }
            catch (Exception ex)
            {
                myConnection.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private SqlCommand getEmployees() {
            try {
                myConnection.Open();
                string sql = @"SELECT [EmployeeID],[FirstName],[LastName] FROM [dbo].[View_Employees]";
                using (SqlCommand employees = new SqlCommand(sql, myConnection))
                {
                    myConnection.Close();
                    return employees;
                }
            }
            catch (Exception ex) {
                myConnection.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        private void openConnection(SqlConnection myConn) {
            try
            {
                myConn.Open();
                MessageBox.Show("sucessfully connected!");
            }
            catch (Exception ex) {
                myConnection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            //MessageBox.Show(e.RowIndex.ToString());
        }


        /// <summary>
        /// DATA GRID DRAG AND DROP FUNCTIONALITY
        /// </summary>
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
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

        private void refreshDataGrid()
        {
            dataGridView1.DataSource = dtBS;
            dataGridView1.Refresh();
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
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
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
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

                DataRow oldRow = ((DataRowView)rowToMove.DataBoundItem).Row;

                DataRow newRow = dtBS.NewRow();
                newRow.ItemArray = oldRow.ItemArray;
                

                dtBS.Rows.Remove(oldRow);
                dtBS.Rows.InsertAt(newRow, rowIndexOfItemUnderMouseToDrop);
                //dtBS.Rows.RemoveAt(rowIndexFromMouseDown+1);

                if (rowIndexOfItemUnderMouseToDrop < 0)
                {
                    return;
                }
            }
        }

    }
}
