#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mono.Data.SqliteClient;
using System.Data;
using System.IO;
using System;

namespace MaxyGames.Generated {
	public class sqlite : MaxyGames.RuntimeBehaviour {
		public IDbConnection dbconn;
		public IDataReader reader;
		public IDbCommand dbcmd;
		public string dbname = "";

		private bool connect(string db_switch) {
			string filepath = "";
			//Подключение разных баз. "свитч"
			if(db_switch.Contains("pogodaiklimat2011")) {
				dbname = "files/pogodaiklimat2011/pogodaiklimat2011.sqlite";
			} else {
				Debug.Log("Неправильное название базы, нету в списке");
				return false;
			}
			//Запуск на пк
			if((Application.platform != RuntimePlatform.Android)) {
				filepath = "URI=file:" + Application.dataPath + "/StreamingAssets/" + dbname;
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
			//Существует ли БД файл
			if(File.Exists("" + Application.dataPath + "/StreamingAssets/" + dbname)) {
				dbconn = new SqliteConnection(filepath);
				dbconn.Open();
				return true;
			} else {
				Debug.Log("Нету Файла Базы Данных: " + filepath);
			}
			return false;
		}

		private void closes() {
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			dbconn.Close();
			dbconn = null;
		}

		public List<string> sqlite_master_tables(string db) {
			List<string> tables = null;
			tables = new List<string>();
			if(connect1(db)) {
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

		public DateTime GetLastDate(string db, string tableName) {
			DateTime _datetime = new DateTime();
			_datetime = DateTime.ParseExact("2011010100", "yyyyMMddHH", null);
			if(connect(db)) {
				if(sqlite_master_tables(db).Contains(tableName)) {
					dbcmd = dbconn.CreateCommand();
					dbcmd.CommandText = "SELECT date FROM '" + tableName + "' ORDER BY date DESC LIMIT 1;";
					using(IDataReader value = dbcmd.ExecuteReader()) {
						reader = value;
						if(reader.Read()) {
							_datetime = DateTime.ParseExact(reader.GetValue(0).ToString(), "yyyyMMddHH", null);
						}
						closes();
					}
				}
			} else {
				CreateNewTable(db, tableName);
			}
			return _datetime;
		}

		public string GetAllDatetimeClmn(string db, string tableName) {
			string dates = "2011010100";
			if(connect(db)) {
				dbcmd = dbconn.CreateCommand();
				dbcmd.CommandText = "select group_concat(date, ',') from \"" + tableName + "\"";
				reader = dbcmd.ExecuteReader();
				reader.Read();
				if(!(reader.IsDBNull(0))) {
					dates = reader.GetValue(0) as string;
				}
				closes();
			}
			return dates;
		}

		public void CreateNewTable(string db, string tableName) {
			if(connect1(db)) {
				dbcmd = dbconn.CreateCommand();
				dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS \"" + tableName + "\"  (\"date_MMddHH\" CHAR PRIMARY KEY  NOT NULL  DEFAULT (null) ,\"wind_dir\" CHAR,\"wind_speed\" CHAR,\"vis_range\" CHAR,\"phenomena\" VARCHAR,\"cloudy\" VARCHAR,\"T\" CHAR,\"Td\" CHAR,\"f\" CHAR,\"Te\" CHAR,\"Tes\" CHAR,\"Comfort\" VARCHAR,\"P\" CHAR,\"Po\" CHAR,\"Tmin\" CHAR,\"Tmax\" CHAR,\"R\" CHAR,\"R24\" CHAR,\"S\" CHAR)";
				reader = dbcmd.ExecuteReader();
				closes();
			}
		}

		private int InsertQuerySimple(string db, string query) {
			int variable0 = 0;
			if(connect1(db)) {
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

		public int InsertQueryTable(string db, string query) {
			string t_name = "";
			t_name = query.Split(new char[] { '"' })[1];
			if(!(sqlite_master_tables(db).Contains(t_name))) {
				CreateNewTable(db, t_name);
			}
			return InsertQuerySimple(db, query);
		}

		public void Update() {
			if(Input.GetKeyUp(KeyCode.LeftArrow)) {
				Debug.Log(0);
			}
		}

		private bool connect1(string db_switch) {
			string filepath1 = "";
			//Запуск на пк
			if((Application.platform != RuntimePlatform.Android)) {
				filepath1 = "URI=file:" + Application.dataPath + "/StreamingAssets/" + "files/pogodaiklimat2011/" + db_switch + ".sqlite";
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
			dbconn = new SqliteConnection(filepath1);
			dbconn.Open();
			return true;
		}
	}
}
