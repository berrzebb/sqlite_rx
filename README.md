# sqlite_rx
C# SQLite Wrapper(With Rx)

# Examples

1. Make Connection String
- SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();

2. ExecuteNonQuery
SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();
string query = "blah";
var ret = db.ExecuteNonQuery(query,statement =>{ // Effected Rows Count Return
  statement.Bind("Key","Value");
}); 

3. ExecuteQuery
SQLiteDatabase db = SQLiteOpenHelper("path").getWriteableDatabase();
string query = "blah";
var ret = db.ExecuteQuery(query,statement =>{ // SQLiteCursor Return
  statement.Bind("Key","Value");
});
// Plan 1 Only One Data
IObservable observable = ret.mapToOne(cursor => {
  return cursor.Get<type>("Key");
});

// Plan 2 Many Data List
IObservable observable = ret.mapToList(cursor => {
  return cursor.Get<type>("Key");
});

// Plan3 Many Data To Observable Single Data
IObservable observable = ret.asRow(cursor =>{
  return cursor.Get<type>("Key");
});
observable.Subscribe(x => {
  //process blahblah
 });
 
 4. Utility
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
 db.Update("tableName","Item = @Item",statement =>{
 statement.Bind("@Item",1);
 });
 
 // Insert Data
 db.Insert("tableName,new []{"Item","Item2","Item3"},statement =>{
 statement.Bind("@Item",1);
 statement.Bind("@Item2","2017-03-01 02:00:00");
 statement.Bind("@Item3","Hello World");
 });
 
