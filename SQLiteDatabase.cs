using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace sqllite_rx
{
    public class SQLiteDatabase
    {
        string connectionString;
        internal SQLiteDatabase(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public object ExecuteSclar(string sql)
        {
            using(var statement = new SQLiteStatement(this, sql))
            {
                return statement.executeScalar();
            }
        }
        public int ExecuteNonQuery(string sql,Action<SQLiteStatement> mapper)
        {
            using (var statement = new SQLiteStatement(this, sql))
            {
                mapper?.Invoke(statement);
                return statement.executeNonQuery();
            }
        }
        public SQLiteCursor ExecuteQuery(string sql,Action<SQLiteStatement> mapper)
        {
            var statement = new SQLiteStatement(this, sql);
            mapper?.Invoke(statement);
            return statement.executeQuery();
        }
        public SQLiteCursor Select(string selector,string table, string where,Action<SQLiteStatement> mapper)
        {
            StringBuilder sb = new StringBuilder($"Select {selector} from {table}");
            if (!string.IsNullOrEmpty(where))
            {
                sb.Append($" where {where}");
            }
            return ExecuteQuery(sb.ToString(), mapper);
        }
        public SQLiteCursor Select(string table)
        {
            return Select("*", table, null, null);
        }
        public bool Update(string tableName,string[] columns, string where,Action<SQLiteStatement> mapper)
        {
            string setString = string.Join(",", columns.Select(x => $"'{x}' = @{x}"));
            return ExecuteNonQuery($"update {tableName} set {setString} where {where}", mapper) >= 1 ? true : false;
        }
        public bool Delete(string tableName, string where, Action<SQLiteStatement> mapper)
        {
            return ExecuteNonQuery($"delete from {tableName} where {where}", mapper) >= 1 ? true : false; 
        }

        public bool Insert(string tableName,string[] columns,Action<SQLiteStatement> mapper)
        {
            string columnString = string.Join(",", columns.Select(x =>$"'{x}'"));
            string valuesString = string.Join(",", columns.Select(x => $"@{x}"));
            return ExecuteNonQuery($"insert into {tableName}  ({columnString}) values({valuesString})", mapper) >= 1 ? true : false;
        }

        public bool ClearTable(string tableName)
        {
            return ExecuteNonQuery($"delete from {tableName}",null) < 1 ? true : false;
        }
        public SQLiteConnection getConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
