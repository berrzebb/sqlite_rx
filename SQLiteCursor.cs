using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace sqllite_rx
{
    public static class CursorExtensions
    {
        public static IObservable<T> mapToOne<T>(this SQLiteCursor cursor, Func<SQLiteCursor, T> mapper)
        {
            return Observable.Create((IObserver<T> x) =>
            {
                if (cursor != null)
                {
                    if (cursor.MoveNext())
                    {
                        x.OnNext(mapper(cursor));
                        if (cursor.MoveNext())
                        {
                            x.OnError(new InvalidOperationException("Cursor returned more than 1 row"));
                        }
                        cursor.Dispose();
                        x.OnCompleted();
                    }
                }
                return Disposable.Create(() => Trace.WriteLine("Query Completed"));
            });
        }
        public static IObservable<List<T>> mapToList<T>(this SQLiteCursor cursor, Func<SQLiteCursor, T> mapper)
        {
            return Observable.Create((IObserver<List<T>> x) =>
            {
                List<T> items = new List<T>();
                try
                {
                    while (cursor.MoveNext())
                    {
                        items.Add(mapper(cursor));
                    }
                    x.OnNext(items);
                }
                catch (Exception e)
                {
                    x.OnError(e);
                }
                finally
                {
                    cursor.Dispose();
                    x.OnCompleted();
                }
                return Disposable.Create(() => Trace.WriteLine($"Query Completed...Record Count {items.Count}"));
            });
        }

        public static IObservable<T> asRow<T>(this SQLiteCursor cursor, Func<SQLiteCursor, T> mapper)
        {
            return Observable.Create<T>((IObserver<T> x) =>
            {
                int affectedRecord = 0;
                if (cursor != null)
                {
                    try
                    {
                        while (cursor.MoveNext())
                        {
                            x.OnNext(mapper(cursor));
                            affectedRecord++;
                        }
                    }
                    catch (Exception e)
                    {
                        x.OnError(e);
                    }
                    finally
                    {
                        cursor.Dispose();
                        x.OnCompleted();
                    }
                }
                return Disposable.Create(() => Trace.WriteLine($"Query Completed...Record Count {affectedRecord}"));
            });
        }
    }
    public class SQLiteCursor : IDisposable
    {
        public const int FIELD_TYPE_BLOB = 4;
        public const int FIELD_TYPE_FLOAT = 2;
        public const int FIELD_TYPE_INTEGER = 1;
        public const int FIELD_TYPE_NULL = 0;
        public const int FIELD_TYPE_STRING = 3;

        private readonly SQLiteStatement statement;
        private readonly SQLiteDataReader cursor;
        internal SQLiteCursor(SQLiteStatement statement,SQLiteDataReader cursor)
        {
            this.statement = statement;
            this.cursor = cursor;
        }

        public void Dispose()
        {
            cursor.Close();
            statement.Dispose();
        }

        public bool MoveNext()
        {
            return cursor.Read();
        }
        public int getColumnIndex(string columnName)
        {
            try
            {
                return cursor.GetOrdinal(columnName);
            }catch(IndexOutOfRangeException e)
            {
                return -1; /// Exception => -1
            }
        }

        public int getColumnIndexOrThrow(string columnName)
        {
            return cursor.GetOrdinal(columnName);
        }
        public IEnumerable<string> getColumnNames()
        {
            return Enumerable.Range(0, cursor.FieldCount).Select(cursor.GetName);
        }

        public T Get<T>(int i)
        {
            return cursor.GetFieldValue<T>(i);
        }
        public T Get<T>(string key)
        {
            int ordinal = getColumnIndex(key);
            return cursor.GetFieldValue<T>(ordinal);
        }
    }
}
