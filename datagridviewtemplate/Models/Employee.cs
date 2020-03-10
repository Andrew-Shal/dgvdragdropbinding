using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datagridviewtemplate.Models
{

    class Employee
    {
        private int _EmployeeID;
        private string _FirstName;
        private string _LastName;
        private string _PositionName;
        private int _PositionReference;
        private int _SortedOrder;

        private bool _IsModified;

        public int EmployeeID { get => _EmployeeID; set { _IsModified = true; _EmployeeID = value; } }
        public string FirstName { get => _FirstName; set { _IsModified = true; _FirstName = value; } }
        public string LastName { get => _LastName; set{ _IsModified = true; _LastName = value; } }
        public string PositionName { get => _PositionName; set { _IsModified = true; _PositionName = value; } }
        public int PositionReference { get => _PositionReference; set{ _IsModified = true; _PositionReference = value; } }
        public int SortedOrder { get => _SortedOrder; set => _SortedOrder = value; }
        public bool IsModified { get => _IsModified; set => _IsModified = value; }

        public Employee()
        {
            _EmployeeID = -1;
            _FirstName = null;
            _LastName = null;
            _PositionName = null;
            _PositionReference = -1;
            _SortedOrder = -1;
            _IsModified = false;
        }

        public Employee(int employeeID, string firstName, string lastName, string positionName, int positionReference)
        {
            _EmployeeID = employeeID;
            _FirstName = firstName;
            _LastName = lastName;
            _PositionName = positionName;
            _PositionReference = positionReference;
        }
    }
}
