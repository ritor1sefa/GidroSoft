#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using System.Data;
using System.Linq;
using TMPro;

namespace MaxyGames.Generated {
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		public Dictionary<string, SqliteConnection> sql_Connections = new Dictionary<string, SqliteConnection>();
		public Dictionary<string, SqliteCommand> sql_cmnds = new Dictionary<string, SqliteCommand>();
		public Dictionary<string, SqliteDataReader> sql_readers = new Dictionary<string, SqliteDataReader>();
		public string[] Files = new string[0];
		public int currentFile = 0;
		public int currentTable = 0;
		public Dictionary<string, string> bd_names = new Dictionary<string, string>();
		public List<string> sql_db_names = new List<string>();
		public string tmp_db_name = "";
		public string tmp_line = "";
		public string tmp_name = "";
		private List<int> delimetrs = new List<int>();
		public string post_name = "";
		public string db_name = "";
		public string N_year_N_month_FromHeader = "";
		public Dictionary<string, string> months = new Dictionary<string, string>() { { "ЯНВАРЬ", "1" }, { "ФЕВРАЛЬ", "2" }, { "МАРТ", "3" }, { "АПРЕЛЬ", "4" }, { "МАЙ", "5" }, { "ИЮНЬ", "6" }, { "ИЮЛЬ", "7" }, { "АВГУСТ", "8" }, { "СЕНТЯБРЬ", "9" }, { "ОКТЯБРЬ", "10" }, { "НОЯБРЬ", "11" }, { "ДЕКАБРЬ", "12" } };
		public string N_year_N_month = "";
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		private string loopObject2;
		private int index;
		private int index1;
		public GameObject objectVariable3;
		private int index4;
		private int index5;

		/// <summary>
		/// sqlite запрос на выборку столбца данных по году+месяцу
		/// SELECT * FROM "2"  Where "2th" LIKE "%2001" AND "2th" LIKE "02%";
		/// SELECT * FROM "2"  Where "2th" LIKE "%y2001%" AND "2th" LIKE "%m02%";
		/// </summary>
		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {}
		}

		public System.Collections.IEnumerator loadFromFiles() {
			string path = "tmp.txt";
			string file_data = "";
			string One_table_data = "";
			string N_table = "";
			List<string> _rowUnparsed = new List<string>();
			List<string> table = new List<string>();
			List<List<string>> Qtable = new List<List<string>>();
			string tmp_tblName = "";
			foreach(string loopObject in Files) {
				path = loopObject;
				file_data = new Regex("^(.*.*).+$", RegexOptions.Multiline).Replace(File.ReadAllText(path), "");
				File.Delete(path);
				table = Enumerable.ToList<System.String>(file_data.Trim().Split("Табли", System.StringSplitOptions.RemoveEmptyEntries));
				objectVariable.gameObject.GetComponent<TMPro.TMP_Text>().text = path;
				currentFile = (currentFile + 1);
				objectVariable1.gameObject.GetComponent<TMPro.TMP_Text>().text = currentFile.ToString();
				foreach(string loopObject1 in table) {
					yield return new WaitForEndOfFrame();
					One_table_data = loopObject1;
					//Пропускаем "таблицы" где много точек = меню в начале файла
					if((!(One_table_data.Contains(".....")) && One_table_data.StartsWith("ца"))) {
						//Get Number of table
						N_table = Regex.Match(One_table_data, "ца\\D*(\\w+)", RegexOptions.None).Result("$1");
						switch(N_table) {
							case "11": {
								//Get subNumber of table N11&N21
								N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*(\\d+)\\).*\\n", RegexOptions.None).Result("$1_$2");
							}
							break;
							case "21": {
								//Get subNumber of table N11&N21
								N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\..*(\\d+)\\).*\\n", RegexOptions.None).Result("$1_$2");
							}
							break;
						}
						//Пропускаемые таблицы
						switch(N_table) {
							case "3": {
							}
							break;
							case "5": {
							}
							break;
							case "9": {
							}
							break;
							case "10": {
							}
							break;
							case "18": {
							}
							break;
							case "19": {
							}
							break;
							case "": {
							}
							break;
							case "4a": {
							}
							break;
							case "7a": {
							}
							break;
							case "": {
							}
							break;
							case "11": {
							}
							break;
							case "23": {
							}
							break;
							default: {
								objectVariable2.gameObject.GetComponent<TMPro.TMP_Text>().text = currentTable.ToString();
								_rowUnparsed = splitTable(One_table_data, N_table);
								yield return new WaitForEndOfFrame();
								delimetrs = _alllndexOfDelimeters(_rowUnparsed, N_table);
								yield return new WaitForEndOfFrame();
								N_year_N_month_FromHeader = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None).Result("y$2_m$1");
								if(!(N_table.Equals(tmp_tblName))) {
									N_year_N_month = "";
									tmp_tblName = N_table;
								}
								//22 таблица
								if(N_table.Contains("22")) {
									yield return parseRow22t(delimetrs, _rowUnparsed, N_table, N_year_N_month_FromHeader, "");
								} else {
									//получатель месяца-года для новых файлов 2016+
									foreach(string tempVar in _rowUnparsed) {
										loopObject2 = tempVar;
										if(Regex.IsMatch(loopObject2, "^\\s*(\\S+) *(\\d{4})")) {
											N_year_N_month = "y" + Regex.Match(loopObject2, "^\\s*(\\S+) *(\\d{4})", RegexOptions.None).Result("$2") + "_m" + months[Regex.Match(loopObject2, "^\\s*(\\S+) *(\\d{4})", RegexOptions.None).Result("$1")];
										} else {
											if(string.IsNullOrWhiteSpace(N_year_N_month)) {
												N_year_N_month = N_year_N_month_FromHeader;
											}
											Qtable = parseRow(delimetrs, new List<string>(), N_table, N_year_N_month, loopObject2);
											yield return sql_insertTables(Qtable, N_table, N_year_N_month);
										}
									}
									yield return new WaitForEndOfFrame();
								}
							}
							break;
						}
					}
					currentTable = (currentTable + 1);
				}
				Debug.Log("След файл");
				sql_close();
				currentTable = 0;
				yield return new WaitForEndOfFrame();
			}
			currentFile = 0;
			Debug.Log("End");
		}

		/// <summary>
		/// Пилим таблицы, при необходимости
		/// </summary>
		public List<string> splitTable(string One_table_data, string N_table) {
			List<string> _rowsUnparsed = new List<string>();
			List<string> _2thPart = default(List<string>);
			string tmp_row_raw = "";
			string tmp_ifNextMonth = "";
			int tmp_row_maxLenght = 0;
			int tmp_row_hulfLenght = 0;
			int year = 0;
			_rowsUnparsed.Clear();
			_rowsUnparsed = new List<string>();
			_2thPart = new List<string>();
			tmp_row_raw = "";
			tmp_row_maxLenght = 0;
			switch(N_table) {
				case "12": {
					//Если шапка с пробелами, то 2 столбца
					if(Regex.IsMatch(One_table_data, "(═|=) ")) {
						//пилим 12 таблицу пополам
						foreach(string loopObject3 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
							if(!(Regex.IsMatch(loopObject3.Trim(), "^ца\\D*(\\d+)\\."))) {
								tmp_row_raw = loopObject3;
								if((tmp_row_raw.Length > tmp_row_maxLenght)) {
									//Тут всегда самая большая длина строки (из шапки)
									tmp_row_maxLenght = tmp_row_raw.Length;
								}
								//Поиск старта второй половины
								if((Regex.IsMatch(tmp_row_raw, "(═|=) ") && Regex.IsMatch(tmp_row_raw, "(\\S+\\s+)\\S+"))) {
									tmp_row_hulfLenght = new Regex("(\\S+\\s+)\\S+", RegexOptions.None).Match(tmp_row_raw).Result("$1").Length;
								}
								//если первая неполная
								if((tmp_row_hulfLenght > tmp_row_raw.Length)) {
									//Если текущая строка меньше половины максимальной
									_rowsUnparsed.Add(tmp_row_raw.Substring(0, tmp_row_raw.Length));
								} else {
									//Если текущая строка больше половины максимальной
									_rowsUnparsed.Add(tmp_row_raw.Substring(0, tmp_row_hulfLenght));
									//второй столбец. засовываем оставшуюся строку
									_2thPart.Add(tmp_row_raw.Substring(tmp_row_hulfLenght, (tmp_row_raw.Length - tmp_row_hulfLenght)));
								}
							}
						}
						foreach(string loopObject4 in _2thPart) {
							if(Regex.IsMatch(loopObject4.TrimStart(), "^\\d{1,3}\\.")) {
								//второго столбца добавляем только строки
								_rowsUnparsed.Add(loopObject4);
							}
						}
					} else {
						tmp_ifNextMonth = "";
						//непилимые таблицы
						foreach(string loopObject5 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
							tmp_row_raw = loopObject5;
							if(loopObject5.Trim().ToLower().Contains("Переход на следующий месяц".ToLower())) {
								//если есть в строке "Переход на следующий месяц"
								tmp_ifNextMonth = "";
							} else {
								_rowsUnparsed.Add(tmp_ifNextMonth + tmp_row_raw);
							}
						}
					}
				}
				break;
				default: {
					tmp_ifNextMonth = "";
					//непилимые таблицы
					foreach(string loopObject5 in One_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.None)) {
						tmp_row_raw = loopObject5;
						if(loopObject5.Trim().ToLower().Contains("Переход на следующий месяц".ToLower())) {
							//если есть в строке "Переход на следующий месяц"
							tmp_ifNextMonth = "";
						} else {
							_rowsUnparsed.Add(tmp_ifNextMonth + tmp_row_raw);
						}
					}
				}
				break;
			}
			return _rowsUnparsed;
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		private List<int> _alllndexOfDelimeters(List<string> _rowsUnparsed, string N_table) {
			string row = "";
			string[] table_data_splited = new string[0];
			HashSet<int> tmp_hash_ints = new HashSet<int>();
			switch(N_table) {
				case "12": {
					foreach(string loopObject6 in _rowsUnparsed) {
						if(Regex.IsMatch(loopObject6.Trim(), "^ *\\d{1,3}\\.")) {
							return Enumerable.ToList<System.Int32>(tmp_hash_ints);
						} else {
							row = loopObject6;
							//Бегает по строке - ищет приключений
							for(index = row.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index > -1; index = row.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index + 1))) {
								tmp_hash_ints.Add((index + 1));
							}
						}
					}
				}
				break;
				case "": {
				}
				break;
				case "": {
				}
				break;
				default: {
					foreach(string loopObject7 in _rowsUnparsed) {
						if(Regex.IsMatch(loopObject7, "^ *\\d{1,3}\\.")) {
							return Enumerable.ToList<System.Int32>(tmp_hash_ints);
						} else {
							row = loopObject7;
							//Бегает по строке - ищет приключений
							for(index1 = row.IndexOfAny(new char[] { '╦', '┬', '|', '¦' }); index1 > -1; index1 = row.IndexOfAny(new char[] { '┬', '╦', '|', '¦' }, (index1 + 1))) {
								tmp_hash_ints.Add(index1);
							}
						}
					}
					return Enumerable.ToList<System.Int32>(tmp_hash_ints);
				}
				break;
			}
			foreach(int loopObject8 in tmp_hash_ints) {
				Debug.Log(loopObject8);
			}
			return Enumerable.ToList<System.Int32>(tmp_hash_ints);
		}

		/// <summary>
		/// Пролучаем массив строк-ячеек из таблицы, чистые и обработанные
		/// </summary>
		public List<List<string>> parseRow(List<int> row_indexs_delimeters, List<string> _rowsUnparsed, string N_table, string N_year_N_month, string unParsedLine) {
			string tokenToSplitBy = "|";
			int insCount = -1;
			string line = "";
			int from = 0;
			int length = 0;
			int item = 0;
			List<List<string>> _tableParsed = new List<List<string>>();
			List<string> _rowsParsed = new List<string>();
			int tmp_startLine = 0;
			List<string> _22_tmp_row1 = new List<string>();
			List<string> _22_final_row = new List<string>();
			string _22_c_row_line = "";
			int _22_7th1 = 0;
			int _22_8th1 = 0;
			int _22_tmp1 = 0;
			row_indexs_delimeters.Sort();
			_tableParsed = new List<List<string>>();
			line = unParsedLine;
			if(Regex.IsMatch(line, "^ +═")) {
				//Сдвиг строки для кривой таблицы N12, второй её половины
				tmp_startLine = (line.Length - line.TrimStart().Length);
			}
			//Пропускаем шапку и мусор из 22й таблицы
			if(!(Regex.IsMatch(line.Trim(), "^ца|Месяц|Переход|[║╟╦╢├┬┤│|I═=]"))) {
				_rowsParsed = new List<string>();
				//Проверка на конец таблицы
				if(!(((row_indexs_delimeters[0] > line.Length) || string.IsNullOrEmpty(line.Substring(row_indexs_delimeters[0], (line.Length - row_indexs_delimeters[0])).Trim())))) {
					if((row_indexs_delimeters[0] >= line.Length)) {
						Debug.Log("===== Только название или вообще пустая строчка====BD=" + tmp_db_name + "==" + "Table=" + N_table + "==YM=" + N_year_N_month + "===" + line.Trim());
					} else {
						if(string.IsNullOrEmpty(line.Substring(0, (row_indexs_delimeters[0] - 1)).TrimStart())) {
							//добавляем данные в первый столбец
							tmp_line = tmp_db_name + line.Substring(tmp_db_name.Length, (line.Length - tmp_db_name.Length));
						} else {
							//если не "Переход на следующий месяц", т.е. обычный
							tmp_name = Regex.Match(" " + line.Substring(0, (row_indexs_delimeters[0] - 1)).TrimStart(), "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1");
							//сохранение имени бд, на случай пустой следующей строки
							tmp_db_name = tmp_name.Trim().Replace(",", "_");
							tmp_line = line;
						}
						_rowsParsed.Add(tmp_db_name);
						for(int index2 = 0; index2 < (row_indexs_delimeters.Count - 1); index2 += 1) {
							from = (row_indexs_delimeters[index2] + tmp_startLine);
							length = (row_indexs_delimeters[(index2 + 1)] - from);
							//Проверка на неполную строчку. заполнение @
							if((tmp_line.Length > from)) {
								if((tmp_line.Length >= (from + length))) {
									//Если совсем всё в порядке и вся ячейка что то имеет
									_rowsParsed.Add(tmp_line.Substring(from, length).Trim());
								} else {
									//Если нехватает символов в ячейке, но что то есть
									_rowsParsed.Add(tmp_line.Substring(from, (tmp_line.Length - from)).Trim());
								}
							} else {
								//если совсем ничего нету
								_rowsParsed.Add("");
							}
						}
						//Проверка на неполную строчку. заполнение @
						if((tmp_line.Length > row_indexs_delimeters[row_indexs_delimeters.Count - 1])) {
							if((tmp_line.Length >= (row_indexs_delimeters[row_indexs_delimeters.Count - 1] + (tmp_line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])))) {
								//last. если все символы на месте.
								_rowsParsed.Add(tmp_line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (tmp_line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
							} else {
								//last. если нехватает некоторых символов
								_rowsParsed.Add(tmp_line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (tmp_line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
							}
						} else {
							//last. если ячейка совсем пустая
							_rowsParsed.Add("");
						}
						_tableParsed.Add(_rowsParsed);
					}
				}
			}
			return _tableParsed;
		}

		/// <summary>
		/// подключение к бд, если не подключено. 
		/// </summary>
		private bool sql_connect(string db_name) {
			string path1 = "";
			SqliteConnection connection = default(SqliteConnection);
			path1 = "" + "" + "Z:/" + db_name + ".sqlite";
			if(sql_Connections.ContainsKey(db_name)) {
				return true;
			} else {
				//Копирование пустой бд в новый файл
				if(!(File.Exists(path1))) {
					File.Copy(Application.streamingAssetsPath + "/" + "files/" + "_empty" + ".sqlite", path1, false);
				}
				connection = new SqliteConnection("URI=file:" + path1);
				connection.Open();
				//добавление в общий список открытых подключений
				sql_Connections.Add(db_name, connection);
			}
			return File.Exists(path1);
		}

		/// <summary>
		/// нифига не работает почему то, на большом количестве разных таблиц.
		/// </summary>
		public void sql_close() {
			foreach(KeyValuePair<string, SqliteCommand> loopObject9 in sql_cmnds) {
				loopObject9.Value.Dispose();
			}
			foreach(KeyValuePair<string, SqliteConnection> loopObject10 in sql_Connections) {
				loopObject10.Value.Dispose();
			}
			SqliteConnection.ClearAllPools();
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			SqliteConnection.ClearAllPools();
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			sql_Connections = new Dictionary<string, SqliteConnection>();
			sql_readers = new Dictionary<string, SqliteDataReader>();
			sql_cmnds = new Dictionary<string, SqliteCommand>();
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// Ненужен?
		/// </summary>
		private List<string> sql_master_tables(string db_name) {
			List<string> tables = new List<string>();
			tables.Clear();
			if(sql_connect(db_name)) {
				using(SqliteCommand value = sql_Connections[db_name].CreateCommand()) {
					sql_cmnds.Add(db_name, value);
					sql_cmnds[db_name].CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
					sql_readers.Add(db_name, sql_cmnds[db_name].ExecuteReader());
					while(sql_readers[db_name].Read()) {
						tables.Add(sql_readers[db_name].GetString(0));
					}
				}
			}
			return tables;
		}

		/// <summary>
		/// Выполняет одну вставку
		/// </summary>
		private int sql_insertQ(string db_name, string q) {
			int sql_writed = 0;
			SqliteCommand cmnd = new SqliteCommand();
			if(sql_connect(db_name)) {
				using(SqliteCommand value1 = sql_Connections[db_name].CreateCommand()) {
					cmnd = value1;
					cmnd.CommandText = q;
					sql_writed = cmnd.ExecuteReader().RecordsAffected;
				}
			} else {
				new _utillz()._2log("sql_insertQ.connect.error", true);
			}
			return sql_writed;
		}

		/// <summary>
		/// Основной вставщик Таблиц в бд.
		/// Сделать под разные таблицы тут? => пробую зафигачить безшапочный вариант
		/// </summary>
		public System.Collections.IEnumerator sql_insertTables(List<List<string>> q_table, string N_table, string N_year_N_month) {
			List<string> row1 = new List<string>();
			string db_name1 = "";
			string q = "";
			string tmp_1thCLMN = "";
			foreach(List<string> loopObject11 in q_table) {
				row1 = loopObject11;
				db_name1 = bd_names[row1[0].ToLower()];
				//убираем название бд из строки. ненужно
				row1.RemoveAt(0);
				//set Q in N11 table
				switch(N_table) {
					case "0": {
					}
					break;
					case "1": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "2": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "3": {
					}
					break;
					case "4": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "5": {
					}
					break;
					case "6": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "7": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "8": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + "7" + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "9": {
					}
					break;
					case "10": {
					}
					break;
					case "11_1": {
						tmp_1thCLMN = N_year_N_month;
						//N11_1
						q = "INSERT OR IGNORE INTO '" + "11" + "' " + "('Year_Month','ДЛ','ДЖ','МР','ЛД','ЖО','С','СЛ','ЗС','КС','КЛ','ТО','СМ','СЛМ','ТОМ','ГД','ИЛ','Р','И','ГЛ','ИЗМ','ГЛЦ','ДМ','Т','ТП')" + " VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "11_2": {
						tmp_1thCLMN = N_year_N_month;
						//N11_2
						q = "INSERT OR IGNORE INTO '" + "11" + "' " + "('Year_Month','ТЛ','ТЛП','ТЗ','ТЛЗ','ТОС','ТЗО','ТТ','ТТО','МГС','П','МС','МО','МН','ММ','МГ','ПП','ПБ','ПЫЛ','Г','ПС','Ш','В','СЧ','МЖ')" + " VALUES ('" + tmp_1thCLMN + "_p2','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "12": {
						//======================
						tmp_1thCLMN = N_year_N_month;
						switch(row1.Count) {
							case 9: {
								//N12 для половинных(двойных) таблиц. в N11
								q = "INSERT OR IGNORE INTO '" + "11" + "' " + "('Year_Month','ДЖ','С','СМ','ТТ','ИЗМ','ГЛ','ММ','ГД','Г')" + " VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
							}
							break;
							case 15: {
								//N12 для нормальных таблиц. в N11
								q = "INSERT OR IGNORE INTO '" + "11" + "' " + "('Year_Month','ДЖ','С','СМ','ТТ','ИЗМ','ГЛ','ММ','ГД','Г','ДМ','ПМ','ПБ','МГ','СЧ','Ш')" + " VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
							}
							break;
						}
					}
					break;
					case "13": {
						tmp_1thCLMN = N_year_N_month;
						//N13
						q = "INSERT OR IGNORE INTO '" + "13" + "' ('Year_Month','" + "ДЛ','ЖО','ТО','ТОМ','ИЗМ','ГЛ','Р','И','ГЛЦ','ДМ','ТТ','ТТО','П','МН','ММ','МГ','ПЫЛ','Г" + "') VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "14": {
						tmp_1thCLMN = N_year_N_month + "_d" + row1[0] + "_h:m=" + Regex.Replace(row1[1], " +", ":");
						//N14&N15
						q = "INSERT OR IGNORE INTO '" + "14" + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "15": {
						tmp_1thCLMN = N_year_N_month + "_d" + row1[0] + "_h:m=" + Regex.Replace(row1[1], " +", ":");
						//N14&N15
						q = "INSERT OR IGNORE INTO '" + "14" + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "16": {
						tmp_1thCLMN = N_year_N_month + "_d" + row1[6] + "_trace:" + row1[5];
						//N16&N17
						q = "INSERT OR IGNORE INTO '" + "16" + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "17": {
						tmp_1thCLMN = N_year_N_month + "_d" + row1[6] + "_trace:" + row1[5];
						//N16&N17
						q = "INSERT OR IGNORE INTO '" + "16" + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "20": {
						tmp_1thCLMN = N_year_N_month;
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "21_2": {
						tmp_1thCLMN = N_year_N_month;
						//N21_2
						q = "INSERT OR IGNORE INTO '" + "21" + "' " + "('N_year_N_month','020_mid', '020_max', '020_min', '040_mid', '040_max', '040_min', '080_mid', '080_max', '080_min', '120_mid', '120_max', '120_min')" + " VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "21_3": {
						tmp_1thCLMN = N_year_N_month;
						//N21_2
						q = "INSERT OR IGNORE INTO '" + "21" + "' " + "('N_year_N_month','160_mid', '160_max', '160_min', '240_mid', '240_max', '240_min', '320_mid', '320_max', '320_min', 'dayFrz_002', 'dayFrz_005', 'dayFrz_010', 'dayFrz_015', 'dayFrz_02', 'dayFrz_04', 'dayFrz_08', 'dayFrz_12', 'dayFrz_16', 'dayFrz_24', 'dayFrz_32')" + " VALUES ('" + tmp_1thCLMN + "_p2','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					case "7a": {
					}
					break;
					case "22": {
						row1.RemoveAt(0);
						tmp_1thCLMN = N_year_N_month + "_key" + string.Join<System.String>("", row1);
						//N22
						q = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + tmp_1thCLMN + "','" + string.Join<System.String>("','", row1) + "')";
					}
					break;
					default: {
						q = "";
					}
					break;
				}
				if((q.Length > 10)) {
					//если повтор
					if(sql_insertQ(db_name1, q).Equals(0)) {
						yield return new WaitForEndOfFrame();
						//Если есть несовпадения значений то дублируем
						if(!(sql_doublers(q, db_name1))) {
							//еслипрямь и дубликата дубликат есть, жжесть
							Debug.Log("1Повтор в бд:===" + db_name1 + "===" + N_table + "===" + q);
							//если повтор
							if(sql_insertQ(db_name1, q.Replace(N_year_N_month, N_year_N_month + "_double")).Equals(0)) {
								//еслипрямь и дубликата дубликат есть, жжесть
								Debug.Log("2Повтор в бд:===" + db_name1 + "===" + N_table + "===" + q);
							}
						}
					}
				}
			}
			return 0;
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// not need?
		/// </summary>
		private string sql_headers(string db_name, string N_table) {
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteDataReader reader = default(SqliteDataReader);
			string _1thCLMN = "";
			if(sql_connect(db_name)) {
				cmnd1 = sql_Connections[db_name].CreateCommand();
				cmnd1.CommandText = "SELECT name FROM pragma_table_info('" + N_table + "') ";
				reader = cmnd1.ExecuteReader();
				_1thCLMN = (reader.GetValue(0) as string);
			}
			return _1thCLMN;
		}

		public void button() {
			bd_names();
			Files = Directory.GetFiles("Z:\\", "*.txt");
			objectVariable3.gameObject.GetComponent<TMPro.TMP_Text>().text = Files.Length.ToString();
			base.StartCoroutine(loadFromFiles());
		}

		public void bd_names() {
			bd_names.Clear();
			//Список на замену названий с "русского" на Русский
			foreach(string loopObject12 in File.ReadAllText(Application.streamingAssetsPath + "/" + "tmp.txt").Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				if(!(bd_names.ContainsKey(loopObject12.Split(new char[] { '=' })[1].ToLower()))) {
					bd_names.Add(loopObject12.Split(new char[] { '=' })[1].ToLower(), loopObject12.Split(new char[] { '=' })[0]);
				}
			}
		}

		private void Start() {}

		/// <summary>
		/// Получение таблицы по запросу
		/// </summary>
		private List<List<string>> sql_getListList(string db_name, string q) {
			List<string> row2 = new List<string>();
			List<List<string>> table1 = new List<List<string>>();
			string variable2 = "";
			table1 = new List<List<string>>();
			if(sql_connect(db_name)) {
				using(SqliteCommand value2 = sql_Connections[db_name].CreateCommand()) {
					sql_cmnds.Add(db_name, value2);
					sql_cmnds[db_name].CommandText = q;
					sql_readers.Add(db_name, sql_cmnds[db_name].ExecuteReader());
					while(sql_readers[db_name].Read()) {
						for(int index3 = 0; index3 < sql_readers[db_name].FieldCount; index3 += 1) {
							//попадаются null, поэтому вручную пустой стринг назначаем
							if(sql_readers[db_name].IsDBNull(index3)) {
								row2.Add("");
							} else {
								row2.Add(sql_readers[db_name].GetString(index3));
							}
						}
						table1.Add(row2);
						row2 = new List<string>();
					}
				}
			}
			sql_close();
			return table1;
		}

		/// <summary>
		/// Удаление повторов из бд
		/// </summary>
		private bool sql_doublers(string q, string db_name) {
			SqliteCommand cmnd2 = new SqliteCommand();
			SqliteDataReader reader1 = default(SqliteDataReader);
			string num_table = "";
			List<string> newValues = new List<string>();
			List<string> Ns_Table = new List<string>();
			string _1thHead = "";
			List<List<string>> tmp_oldList = new List<List<string>>();
			string value_data = "";
			string value_clmns = "";
			if(Regex.IsMatch(q, "'([A-я0-9_.-]+)' *VALUES *\\('([A-я0-9_.:= -]+)'")) {
				num_table = new Regex("'([A-я0-9_.-]+)' *VALUES *\\('([A-я0-9_.:= -]+)'", RegexOptions.Multiline).Match(q).Result("$1");
				value_data = new Regex("'([A-я0-9_.-]+)' *VALUES *\\('([A-я0-9_.:= -]+)'", RegexOptions.Multiline).Match(q).Result("$2");
				//set Q in N11 table
				switch(num_table) {
					case "0": {
					}
					break;
					case "1": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "2": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "3": {
					}
					break;
					case "4": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "5": {
					}
					break;
					case "6": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "7": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "8": {
					}
					break;
					case "9": {
					}
					break;
					case "10": {
					}
					break;
					case "11_1": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "11_2": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "12": {
					}
					break;
					case "13": {
						_1thHead = "Year_Month";
					}
					break;
					case "14": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "15": {
					}
					break;
					case "16": {
						_1thHead = "N_year_N_month_N_day_trace";
					}
					break;
					case "17": {
					}
					break;
					case "20": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "21_2": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "21_3": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "7a": {
					}
					break;
					case "22": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "11": {
						_1thHead = "Year_Month";
					}
					break;
					case "21": {
						_1thHead = "N_year_N_month";
					}
					break;
					default: {
						Debug.Log("Надо добавить названий первых столбцов!==" + value_data);
						_1thHead = value_data;
					}
					break;
				}
				newValues = Enumerable.ToList<System.String>(new Regex("VALUES \\(('.+')\\)", RegexOptions.None).Match(q).Result("$1").Replace("'", "").Split(new char[] { ',' }));
				foreach(List<string> loopObject13 in sql_getListList(db_name, "SELECT * FROM '" + num_table + "' WHERE  \"" + _1thHead + "\" LIKE '%" + value_data + "%'")) {
					for(index4 = 1; index4 < loopObject13.Count; index4 += 1) {
						if(!(loopObject13[index4].Equals(newValues[index4]))) {
							//Есть несовпадение
							return false;
						}
					}
					//Всё совпадает
					return true;
				}
			} else if(Regex.IsMatch(q, "'([A-я0-9_.-]+)' *\\(([A-я0-9_.,':= -]+)\\) *VALUES *\\(([A-я0-9_.,':= -]+)")) {
				num_table = new Regex("'([A-я0-9_.-]+)' *\\(([A-я0-9_.,':= -]+)\\) *VALUES *\\(([A-я0-9_.,':= -]+)", RegexOptions.Multiline).Match(q).Result("$1");
				value_clmns = new Regex("'([A-я0-9_.-]+)' *\\(([A-я0-9_.,':= -]+)\\) *VALUES *\\(([A-я0-9_.,':= -]+)", RegexOptions.Multiline).Match(q).Result("$2").Replace("'", "\"");
				value_data = new Regex("'([A-я0-9_.-]+)' *\\(([A-я0-9_.,':= -]+)\\) *VALUES *\\(([A-я0-9_.,':= -]+)", RegexOptions.Multiline).Match(q).Result("$3").Replace("'", "");
				//set Q in N11 table
				switch(num_table) {
					case "0": {
					}
					break;
					case "1": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "2": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "3": {
					}
					break;
					case "4": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "5": {
					}
					break;
					case "6": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "7": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "8": {
					}
					break;
					case "9": {
					}
					break;
					case "10": {
					}
					break;
					case "11_1": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "11_2": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "12": {
					}
					break;
					case "13": {
						_1thHead = "Year_Month";
					}
					break;
					case "14": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "15": {
					}
					break;
					case "16": {
						_1thHead = "N_year_N_month_N_day_trace";
					}
					break;
					case "17": {
					}
					break;
					case "20": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "21_2": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "21_3": {
						_1thHead = "N_year_N_month";
					}
					break;
					case "7a": {
					}
					break;
					case "22": {
						_1thHead = "Year_Month_Key";
					}
					break;
					case "11": {
						_1thHead = "Year_Month";
					}
					break;
					case "21": {
						_1thHead = "N_year_N_month";
					}
					break;
					default: {
						Debug.Log("Надо добавить названий первых столбцов!==" + value_data);
						_1thHead = value_data;
					}
					break;
				}
				newValues = Enumerable.ToList<System.String>(value_data.Split(new char[] { ',' }));
				foreach(List<string> loopObject14 in sql_getListList(db_name, "SELECT " + value_clmns + " FROM '" + num_table + "' WHERE \"" + _1thHead + "\" LIKE '%" + value_data.Split(new char[] { ',' })[0] + "%'")) {
					for(index5 = 1; index5 < loopObject14.Count; index5 += 1) {
						if(!(loopObject14[index5].Equals(newValues[index5]))) {
							//Есть несовпадение
							return false;
						}
					}
					//Всё совпадает
					return true;
				}
			} else {
				Debug.Log("error_doublers:" + q);
			}
		}

		public List<string> parseRow2List(string row) {
			List<string> ret = new List<string>();
			Regex regex_cl = default(Regex);
			string row_l = "";
			int r_from = 0;
			int r_l = 0;
			delimetrs.Sort();
			regex_cl = new Regex("[║╟╦╢├┬┤│|I═=]*");
			row_l = row;
			ret.Add(regex_cl.Replace(row_l.Substring(0, delimetrs[0]), ""));
			//основное тело распарса строки
			for(int index6 = 0; index6 < (delimetrs.Count - 1); index6 += 1) {
				r_from = delimetrs[index6];
				r_l = (delimetrs[(index6 + 1)] - r_from);
				//Проверка на неполную строчку. заполнение @
				if((row_l.Length > r_from)) {
					if((row_l.Length >= (r_from + r_l))) {
						ret.Add(regex_cl.Replace(row_l.Substring(r_from, r_l), ""));
					} else {
						ret.Add(regex_cl.Replace(row_l.Substring(r_from, (row_l.Length - r_from)), ""));
					}
				} else {
					ret.Add("");
				}
			}
			//Проверка на неполную строчку. заполнение @
			if((row_l.Length > delimetrs[delimetrs.Count - 1])) {
				if((row_l.Length >= (delimetrs[delimetrs.Count - 1] + (row_l.Length - delimetrs[delimetrs.Count - 1])))) {
					ret.Add(regex_cl.Replace(row_l.Substring(delimetrs[delimetrs.Count - 1], (row_l.Length - delimetrs[delimetrs.Count - 1])), ""));
				} else {
					ret.Add(regex_cl.Replace(row_line.Substring(delimetrs[delimetrs.Count - 1], (row_l.Length - delimetrs[delimetrs.Count - 1])), ""));
				}
			} else {
				ret.Add("");
			}
			return ret;
		}

		/// <summary>
		/// Пролучаем массив строк-ячеек из таблицы, чистые и обработанные
		/// </summary>
		public System.Collections.IEnumerator parseRow22t(List<int> row_indexs_delimeters, List<string> _rowsUnparsed, string N_table, string N_year_N_month, string unParsedLine) {
			string tokenToSplitBy1 = "|";
			int insCount1 = -1;
			string line1 = "";
			int from1 = 0;
			int length1 = 0;
			int item1 = 0;
			List<List<string>> _tableParsed1 = new List<List<string>>();
			List<string> _rowsParsed1 = new List<string>();
			int tmp_startLine1 = 0;
			List<string> _22_tmp_row = new List<string>();
			List<string> _22_final_row1 = new List<string>();
			string _22_c_row_line1 = "";
			int _22_7th = 0;
			int _22_8th = 0;
			int _22_9th = 0;
			int _22_tmp = 0;
			List<List<string>> _22_final_table = default(List<List<string>>);
			List<string> lastListRow = default(List<string>);
			string _4sqlValue = "";
			string key = "";
			string tmpKey = "";
			string q1 = "";
			string tmp_name_bd = "";
			List<string> variable23 = new List<string>() { "", "", "", "", "" };
			_tableParsed1 = new List<List<string>>();
			_rowsParsed1 = new List<string>();
			//Очистка от лишнего
			foreach(string loopObject15 in _rowsUnparsed) {
				line1 = loopObject15;
				//новый список строк. чистый
				if(!((((line1.Contains("ГОЛОЛЕДНО") || string.IsNullOrWhiteSpace(line1)) || Regex.IsMatch(line1, "[║╟╦╢├┬┤│|I═=]")) || line1.Contains("Месяц")))) {
					_rowsParsed1.Add(line1);
				}
			}
			yield return new WaitForEndOfFrame();
			_22_final_table = new List<List<string>>();
			//построчная обработка
			foreach(string loopObject16 in _rowsParsed1) {
				_22_tmp_row = parseRow2List(loopObject16);
				if(string.Join("", _22_tmp_row).Contains("Переход")) {
					variable23[0] = _22_tmp_row[0];
					_22_final_table.Add(variable23);
				} else if(string.IsNullOrWhiteSpace(_22_tmp_row[1].Trim())) {
					//Если строчка начинается с пустоты=продолжение предыдущей
					if(int.TryParse(_22_tmp_row[7], out _22_tmp)) {
						//6й стобец
						if((_22_7th < _22_tmp)) {
							_22_7th = _22_tmp;
							_22_final_table[_22_final_table.Count - 1][7] = _22_7th.ToString();
						}
					} else {
						Debug.Log("7й столбей не парсанулся! " + _22_tmp_row[7] + "=" + string.Join("+", _22_tmp_row));
					}
					//Если строчка начинается с пустоты=продолжение предыдущей
					if(int.TryParse(_22_tmp_row[8], out _22_tmp)) {
						//7й столбец
						if((_22_8th < _22_tmp)) {
							_22_8th = _22_tmp;
							_22_final_table[_22_final_table.Count - 1][8] = _22_8th.ToString();
						}
					} else {
						Debug.Log("8й столбей не парсанулся! " + _22_tmp_row[8] + "=" + string.Join("+", _22_tmp_row));
					}
					//Если строчка начинается с пустоты=продолжение предыдущей
					if(int.TryParse(_22_tmp_row[9], out _22_tmp)) {
						//7й столбец
						if((_22_9th < _22_tmp)) {
							_22_9th = _22_tmp;
							_22_final_table[_22_final_table.Count - 1][9] = _22_9th.ToString();
						}
					}
				} else {
					int.TryParse(_22_tmp_row[7], out _22_7th);
					int.TryParse(_22_tmp_row[8], out _22_8th);
					int.TryParse(_22_tmp_row[9], out _22_9th);
					_22_final_table.Add(_22_tmp_row);
				}
			}
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			//формирование строк для/в dict`s
			foreach(List<string> loopObject17 in _22_final_table) {
				lastListRow = loopObject17;
				//Поднимаем буквы, потому что в другом наборе данных - они почему то подняты.
				lastListRow[2] = lastListRow[2].ToUpper();
				if(!(lastListRow[0].Equals(post_name))) {
					if(string.IsNullOrWhiteSpace(lastListRow[0])) {
						//Поднимаем буквы, потому что в другом наборе данных - они почему то подняты.
						lastListRow[0] = post_name;
					} else {
						post_name = lastListRow[0].Trim();
					}
				}
				if(!((lastListRow.Count < 10))) {
					//кусок для 2016+ годов
					if(Regex.IsMatch(post_name, "^\\s*(\\S+) *(\\d{4})")) {
						N_year_N_month = "y" + Regex.Match(post_name, "^\\s*(\\S+) *(\\d{4})", RegexOptions.None).Result("$2") + "_m" + months[Regex.Match(post_name, "^\\s*(\\S+) *(\\d{4})", RegexOptions.None).Result("$1")];
					} else {
						if(string.IsNullOrWhiteSpace(N_year_N_month)) {
							N_year_N_month = N_year_N_month_FromHeader;
						}
						if(string.IsNullOrWhiteSpace(lastListRow[0])) {
							Debug.Log("Пустая линия в 22й таблице!");
						} else {
							tmp_name_bd = Regex.Match(lastListRow[0], "\\D*\\d+\\.(.+)", RegexOptions.None).Result("$1").Trim().Replace(',', '_').ToLower();
							if(bd_names.ContainsKey(tmp_name_bd)) {
								db_name = bd_names[tmp_name_bd];
								lastListRow.RemoveAt(1);
								lastListRow.RemoveAt(0);
								//N22
								q1 = "INSERT OR IGNORE INTO '" + N_table + "' " + "VALUES ('" + "" + "" + N_year_N_month + "" + "" + "_k" + string.Join("", Enumerable.Select<System.String, System.String>(lastListRow, (string parameterValues) => {
									return parameterValues.Trim();
								})) + "','" + string.Join("','", Enumerable.Select<System.String, System.String>(lastListRow, (string parameterValues) => {
									return parameterValues.Trim();
								})) + "')";
								if((q1.Length > 10)) {
									//если повтор
									if(sql_insertQ(db_name, q1).Equals(0)) {
										//Если есть несовпадения значений то дублируем
										if(!(sql_doublers(q1, db_name))) {
											//еслипрямь и дубликата дубликат есть, жжесть
											Debug.Log("1Повтор в бд:===" + db_name + "===" + N_table + "===" + q1);
											//если повтор
											if(sql_insertQ(db_name1, q1.Replace(N_year_N_month, N_year_N_month + "_double")).Equals(0)) {
												//еслипрямь и дубликата дубликат есть, жжесть
												Debug.Log("2Повтор в бд:===" + db_name + "===" + N_table + "===" + q1);
											}
										}
									}
								}
							} else {
								Debug.Log(tmp_name_bd);
							}
						}
						yield return new WaitForEndOfFrame();
					}
				}
			}
			return _tableParsed1;
		}
	}
}
