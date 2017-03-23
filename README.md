# sqlite_rx
C# SQLite Wrapper(With Rx)
=========
# Examples

1. Make Connection String
<pre>
<code>
SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();
</code>
</pre>

2. ExecuteNonQuery
<pre>
<code>
SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();
string query = "blah";
var ret = db.ExecuteNonQuery(query,statement =>{ // Effected Rows Count Return
  statement.Bind("Key","Value");
}); 
</code>
</pre>

3. ExecuteQuery
<pre>
<code>
> SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();
> string query = "blah";
> var ret = db.ExecuteQuery(query,statement =>{ // SQLiteCursor Return
>  statement.Bind("Key","Value");
> });
</code>
</pre>

> Plan 1 Only One Data
<pre>
<code>
IObservable observable = ret.mapToOne(cursor => {
  return cursor.Get<type>("Key");
});
</code>
</pre>
> Plan 2 Many Data List
<pre>
<code>
IObservable observable = ret.mapToList(cursor => {
  return cursor.Get<type>("Key");
});
</code>
</pre>

> Plan3 Many Data To Observable Single Data
<pre>
<code>
IObservable observable = ret.asRow(cursor =>{
  return cursor.Get<type>("Key");
});
observable.Subscribe(x => {
  //process blahblah
 });
 </code>
</pre>

 4. Utility
 <pre>
<code>
 SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();
 // Select All From Table
 db.Select("*","tableName",null,null);
 
 // Select item,item2,item3 From Table
 db.Select("item,item2,item3","tableName",null,null).mapToList(cursor => Tuple.Create(cursor.get<int>("item"),cursor.get<DateTime>("item2"),cursor.get<string>("item3"));
 
 // Select Item == Value All From Table
 db.Select("*","tableName","Item = @Item,statement => {
  statement.Bind("@Item",1);
 });
 
 // Update Data
 db.Update("tableName",new [] {"Item2","Item3"},"Item = @Item",statement =>{
 statement.Bind("@Item",1);
 statement.Bind("@Item2","2017-03-01 02:00:00");
 statement.Bind("@Item3","Hello World");
 });
 
// Delete Data
 db.Delete("tableName","Item = @Item",statement =>{
 statement.Bind("@Item",1);
 });
 
 // Insert Data
 db.Insert("tableName,new []{"Item","Item2","Item3"},statement =>{
 statement.Bind("@Item",1);
 statement.Bind("@Item2","2017-03-01 02:00:00");
 statement.Bind("@Item3","Hello World");
 });
 </code>
</pre>
