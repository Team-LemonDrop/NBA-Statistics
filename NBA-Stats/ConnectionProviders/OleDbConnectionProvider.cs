using NBA_Stats.ConnectionProviders.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBA_Stats.ConnectionProviders
{
    public class OleDbConnectionProvider : IConnectionProvider
    {
        public static string GetConnectionString(string filePath, bool xlsx)
        {
            Dictionary<string, string> props;

            if (xlsx)
            {
                // XLSX - Excel 2007, 2010, 2012, 2013
                props = new Dictionary<string, string>
                {
                    ["Provider"] = "Microsoft.ACE.OLEDB.12.0",
                    ["Data Source"] = filePath, // "../../trainers.xlsx"
                    ["Extended Properties"] = "\"Excel 12.0 XML\""                    
                };
            }
            else
            {
                // XLS - Excel 2003 and Older
                props = new Dictionary<string, string>
                {
                    ["Provider"] = "Microsoft.Jet.OLEDB.4.0",
                    ["Data Source"] = filePath, //"../../trainers.xls",
                    ["Extended Properties"] = "\"Excel 8.0;HDR=YES;\""
                };
            }           

            var sb = new StringBuilder();
            foreach (var prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        public IDbConnection CreateConnection(string connectionString = null)
        {
            if (connectionString == null)
            {
                connectionString = GetConnectionString("demo.xlsx", false);
            }

            var connection = new OleDbConnection(connectionString);

            return connection;
        }
    }
}
