using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sqllite_rx
{
    public class SQLiteStatement : IDisposable
    {
        SQLiteCommand command;
        SQLiteDatabase database;
        SQLiteConnection connection;
        internal SQLiteStatement(SQLiteDatabase database,string sql)
        {
            this.database = database;
            connection = database.getConnection();
            command = connection.CreateCommand();
            command.CommandText = sql;
            connection.Open();
        }

        public void Bind(string key,object value)
        {
            command.Parameters.AddWithValue(key, value);
        }
        public void Clear()
        {
            command.Parameters.Clear();
        }
        public SQLiteCursor executeQuery()
        {
            try
            {
                return new SQLiteCursor(this,command.ExecuteReader());
            }catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
        }
        public int executeNonQuery()
        {
            try
            {
                var ret = command.ExecuteNonQuery();
                Trace.WriteLine($"{command.CommandText} Query Done...{ret}");
                return ret;
            }catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return 1;
            }
        }
        
        public object executeScalar()
        {
            try
            {
                return command.ExecuteScalar();
            }catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
        }
        public void Dispose()
        {
            command.Dispose();
            connection.Close();
        }
    }
}
