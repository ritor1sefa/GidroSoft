#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.IO;
using System.IO.MemoryMappedFiles;

public class sqlite : object {
	private IDbConnection dbconn;
	private IDataReader reader;
	private IDbCommand dbcmd;
	private string dbname = "";
	public Dictionary<string, System.Tuple<string, string>> variable4;

	private bool connect(string db_index) {
		string filepath = "";
		//Запуск на пк
		if((Application.platform != RuntimePlatform.Android)) {
			//отредактировать потом, когда другие слои-тагеты будут, не только погода2011
			filepath = "URI=file:" + Application.dataPath + "/StreamingAssets/" + "files/pogodaiklimat2011/bd/" + db_index + ".sqlite";
		} else {
			//			dbconn.Open(); //Open connection to the database.
			//			dbconn = (IDbConnection)new SqliteConnection("URI=file:" + Application.persistentDataPath + dbname);
			//			}
			//				File.WriteAllBytes(filepath, loadDB.bytes);
			//				while (!loadDB.isDone) { }
			//				WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + dbname);
			//			{
			//			if (!File.Exists(filepath))
			//	// если базы данных по заданному пути нет, размещаем ее там
			Debug.Log("Android");
			return false;
		}
		dbconn = new SqliteConnection(filepath);
		dbconn.Open();
		return true;
	}

	private void closes() {
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}

	public List<string> sqlite_master_tables(string db_index) {
		List<string> tables = null;
		tables = new List<string>();
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
			reader = dbcmd.ExecuteReader();
			while(reader.Read()) {
				tables.Add(reader.GetString(0));
			}
			closes();
		}
		return tables;
	}

	public System.DateTime GetLastDate(string db_index, string tableName_year) {
		System.DateTime _datetime = new System.DateTime();
		_datetime = System.DateTime.ParseExact("2011010100", "yyyyMMddHH", null);
		if(connect(db_index)) {
			if(sqlite_master_tables(db_index).Contains(tableName_year)) {
				dbcmd = dbconn.CreateCommand();
				dbcmd.CommandText = "SELECT date FROM '" + tableName_year + "' ORDER BY date DESC LIMIT 1;";
				using(IDataReader value = dbcmd.ExecuteReader()) {
					reader = value;
					if(reader.Read()) {
						_datetime = System.DateTime.ParseExact(reader.GetValue(0).ToString(), "yyyyMMddHH", null);
					}
					closes();
				}
			}
		} else {
			CreateNewTable(db_index, tableName_year);
		}
		return _datetime;
	}

	public string GetAllDatetimeClmn(string db_index, string tableName_year) {
		string dates = "2011010100";
		CreateNewTable(db_index, tableName_year);
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = "select group_concat(date, ',') from \"" + tableName_year + "\"";
			reader = dbcmd.ExecuteReader();
			reader.Read();
			if(!(reader.IsDBNull(0))) {
				dates = reader.GetValue(0) as string;
			}
			closes();
		}
		return dates;
	}

	public void CreateNewTable(string db_index, string tableName_year) {
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS \"" + tableName_year + "\"  (\"date\" CHAR PRIMARY KEY  NOT NULL  DEFAULT (null) ,\"wind_dir\" CHAR,\"wind_speed\" CHAR,\"vis_range\" CHAR,\"phenomena\" VARCHAR,\"cloudy\" VARCHAR,\"T\" CHAR,\"Td\" CHAR,\"f\" CHAR,\"Te\" CHAR,\"Tes\" CHAR,\"Comfort\" VARCHAR,\"P\" CHAR,\"Po\" CHAR,\"Tmin\" CHAR,\"Tmax\" CHAR,\"R\" CHAR,\"R24\" CHAR,\"S\" CHAR)";
			reader = dbcmd.ExecuteReader();
			closes();
		}
	}

	private int InsertQuerySimple(string db_index, string query) {
		int variable0 = 0;
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = query;
			using(IDataReader value1 = dbcmd.ExecuteReader()) {
				reader = value1;
				variable0 = reader.RecordsAffected;
				closes();
			}
		}
		return variable0;
	}

	public int InsertQueryTable(string db_index, string query) {
		string t_name = "";
		t_name = query.Split(new char[] { '"' })[1];
		if(!(sqlite_master_tables(db_index).Contains(t_name))) {
			CreateNewTable(db_index, t_name);
		}
		return InsertQuerySimple(db_index, query);
	}

	public List<List<string>> getData(string db_index, string q) {
		List<string> row = null;
		List<List<string>> table = null;
		table = new List<List<string>>();
		row = new List<string>();
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = q;
			reader = dbcmd.ExecuteReader();
			while(reader.Read()) {
				for(int index = 0; index < 19; index += 1) {
					row.Add(reader.GetString(index));
				}
				table.Add(row);
				row = new List<string>();
			}
			closes();
			Debug.Log("Строк:" + table.Count.ToString());
			return table;
		}
		closes();
		return table;
	}

	public List<List<string>> getDataUniversalLstLst(string db_index, string q) {
		List<string> row1 = null;
		List<List<string>> table1 = null;
		table1 = new List<List<string>>();
		row1 = new List<string>();
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = q;
			reader = dbcmd.ExecuteReader();
			while(reader.Read()) {
				for(int index1 = 0; index1 < reader.FieldCount; index1 += 1) {
					row1.Add(reader.GetString(index1));
				}
				table1.Add(row1);
				row1 = new List<string>();
			}
			closes();
			new _Log()._2log("Строк:" + table1.Count.ToString(), false);
			return table1;
		}
		closes();
		return table1;
	}

	public string getDataUniversalStr(string db_index, string q) {
		string a_strng = "2011010100";
		CreateNewTable(db_index, q);
		if(connect(db_index)) {
			dbcmd = dbconn.CreateCommand();
			dbcmd.CommandText = q;
			reader = dbcmd.ExecuteReader();
			reader.Read();
			if(!(reader.IsDBNull(0))) {
				a_strng = reader.GetValue(0) as string;
			}
			closes();
		}
		return a_strng;
	}

	public object Update() {
		if(Input.GetKeyUp(KeyCode.LeftArrow)) {
			Debug.Log(sqlite_master_tables("29838").Count);
		}
		return null;
	}

	public void NewFunction() {
		System.Data.Common.DbCommand command = null;
		SqliteConnection conn1 = null;
		System.Tuple<string, string, string> variable2 = null;
	}
}
