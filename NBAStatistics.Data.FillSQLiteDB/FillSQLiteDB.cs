using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite.Generic;

namespace NBAStatistics.Data.FillSQLiteDB
{
    public class FillSQLiteDB
    {
        private static SQLiteConnection connection = new SQLiteConnection();
        private readonly string connectionString = "../../SqlliteDb.sqlite";
    }
}
