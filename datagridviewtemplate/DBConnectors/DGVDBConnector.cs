using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datagridviewtemplate.DBConnectors
{
    class DGVDBConnector:DBConnector
    {
        public DGVDBConnector(string connectionString) : base()
        {
            _ConnectionString = connectionString;
            Connect();
        }
    }
}
