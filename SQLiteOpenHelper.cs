using System.Data.SQLite;

namespace sqllite_rx
{
    public class SQLiteOpenHelper
    {
        private SQLiteConnectionStringBuilder builder;
        public SQLiteOpenHelper(string DB)
        {
            builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = DB;
            builder.DefaultTimeout = 5000;
            builder.SyncMode = SynchronizationModes.Off;
            builder.JournalMode = SQLiteJournalModeEnum.Memory;
            builder.Add("cache", "shared");
            builder.Pooling = true;
            builder.PageSize = 65536;
            builder.CacheSize = 16777216;
            builder.FailIfMissing = false;
            builder.Version = 3;

        }
        public SQLiteDatabase getReadableDatabase()
        {
            builder.ReadOnly = true;
            return new SQLiteDatabase(builder.ConnectionString);
        }
        public SQLiteDatabase getWriteableDatabase()
        {
            builder.ReadOnly = false;
            return new SQLiteDatabase(builder.ConnectionString);
        }
    }
}
