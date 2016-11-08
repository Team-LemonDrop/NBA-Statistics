using System.Data.SQLite;

namespace NBAStatistics.Data.SQLite
{
    public class FillSQLiteDB
    {
        private static SQLiteConnection connection = new SQLiteConnection();
        private readonly string connectionString = "../../SqlliteDb.sqlite";
    }
}
