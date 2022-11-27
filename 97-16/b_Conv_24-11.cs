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
	public class b_Conv_24-11 : MaxyGames.RuntimeBehaviour {
		public string[] Files = new string[0];
		public Dictionary<string, string> bd_names = new Dictionary<string, string>();
		public List<string> rows_unparsed = new List<string>();
		public List<int> delimetrs = new List<int>();
		public string NameOfDB = "";
		public List<string> bd_names_array = new List<string>();
		public string Year = "";
		public string Month = "";
		public Dictionary<string, string> n1 = new Dictionary<string, string>();
		public Dictionary<string, string> n2 = new Dictionary<string, string>();
		public Dictionary<string, Dictionary<string, string>> listOfDicTables = new Dictionary<string, Dictionary<string, string>>();
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		private int index2;
		private List<int> delimetrs1 = new List<int>();

		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {
				Debug.Log(n1.ToString());
			}
		}

		public void button() {
			n1 = new Dictionary<string, string>();
			//Собираем названия для бд
			bd_names();
			Files = Directory.GetFiles("Z:\\", "*.txt");
			objectVariable.gameObject.GetComponent<TMPro.TMP_Text>().text = Files.Length.ToString();
			base.StartCoroutine(convert_mainLoop());
		}

		public void bd_names() {
			bd_names.Clear();
			//Список на замену названий с "русского" на Русский
			foreach(string loopObject in File.ReadAllText(Application.streamingAssetsPath + "/" + "tmp.txt").Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				if(!(bd_names.ContainsKey(loopObject.Split(new char[] { '=' })[1].ToLower()))) {
					bd_names.Add(loopObject.Split(new char[] { '=' })[1].ToLower(), loopObject.Split(new char[] { '=' })[0]);
				}
			}
		}

		public System.Collections.IEnumerator convert_mainLoop() {
			string fPath = "";
			string q1 = "";
			string tmp_q = "";
			bd_names_array = new List<string>();
			for(int index = 0; index < Files.Length; index += 1) {
				fPath = Files[index];
				objectVariable1.gameObject.GetComponent<TMPro.TMP_Text>().text = fPath;
				objectVariable2.gameObject.GetComponent<TMPro.TMP_Text>().text = (index + 1).ToString();
				yield return selector_loop(fPath);
				Debug.Log("След файл");
			}
			Debug.Log("Закончена обработка файлов.");
			listOfDicTables.Clear();
			listOfDicTables.Add("n1", n1);
			listOfDicTables.Add("n2", n2);
			//перебор названий постов-файлов бд
			foreach(string loopObject1 in bd_names_array) {
				n1_sql(loopObject1);
			}
		}

		public System.Collections.IEnumerator selector_loop(string path) {
			string file_data = "";
			string One_table_data = "";
			List<string> _rowUnparsed = new List<string>();
			List<string> _rowUnparsed0 = new List<string>();
			List<string> tables = new List<string>();
			List<List<string>> Qtable = new List<List<string>>();
			Dictionary<string, string> months = new Dictionary<string, string>() { { "ЯНВАРЬ", "1" }, { "ФЕВРАЛЬ", "2" }, { "МАРТ", "3" }, { "АПРЕЛЬ", "4" }, { "МАЙ", "5" }, { "ИЮНЬ", "6" }, { "ИЮЛЬ", "7" }, { "АВГУСТ", "8" }, { "СЕНТЯБРЬ", "9" }, { "ОКТЯБРЬ", "10" }, { "НОЯБРЬ", "11" }, { "ДЕКАБРЬ", "12" } };
			string N_year_N_month_FromHeader = "";
			string tmp_tblName = "";
			if(Regex.IsMatch(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)")) {
				Year = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$3");
				Month = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$2");
				//название поста, добавить сюда же потом проход через bd_names
				NameOfDB = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$1").Trim();
				//Проверка наличия в списке. если уже есть, то не добавлять, что бы потом лишний раз не бегать
				if(!(bd_names_array.Contains(NameOfDB))) {
					//Добавление имени бд для последующего sql инжекта
					bd_names_array.Add(NameOfDB);
				}
				file_data = File.ReadAllText(path);
				tables = Enumerable.ToList<System.String>(file_data.Trim().Split("Станция", System.StringSplitOptions.RemoveEmptyEntries));
				foreach(string loopObject2 in tables) {
					One_table_data = loopObject2;
					if(One_table_data.Contains("Атм. давление,")) {
						Debug.Log("=n1");
						yield return n1(One_table_data);
					} else if(One_table_data.Contains("С У Т О Ч Н Ы Е   Д А Н Н Ы Е")) {
						Debug.Log(One_table_data);
						yield return n2(One_table_data);
					} else if(One_table_data.Contains("А Т М О С Ф Е Р Н Ы Е   Я В Л Е Н И Я")) {
						Debug.Log(One_table_data);
					} else if(One_table_data.Contains("М Е С Я Ч Н Ы Е   В Ы В О Д Ы")) {
						Debug.Log(One_table_data);
					} else if(One_table_data.Contains("ТЕМПЕРАТУРА ПОЧВЫ НА ГЛУБ. ЗА СУТКИ, град")) {
						Debug.Log(One_table_data);
					} else if(One_table_data.Contains("СТИХИЙНЫЕ Г/М ЯВЛЕНИЯ, СНЕГОСЪЕМКИ, Г/И ОТЛОЖЕНИЯ")) {
						Debug.Log(One_table_data);
					}
				}
			}
		}

		/// <summary>
		/// подключение к бд, если не подключено. 
		/// </summary>
		private bool sql_dbExists(string db_name) {
			string path = "";
			SqliteConnection connection2 = default(SqliteConnection);
			path = Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			//Копирование пустой бд в новый файл
			if(!(File.Exists(path))) {
				//Копирование пустой бд в новый файл
				File.Copy(Application.streamingAssetsPath + "/" + "files/" + "_emptyNew" + ".sqlite", path, false);
			}
			return File.Exists(path);
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// </summary>
		private List<string> sql_master_tables(string db_name) {
			List<string> tables1 = new List<string>();
			SqliteCommand cmnd = default(SqliteCommand);
			SqliteDataReader reader = default(SqliteDataReader);
			SqliteConnection connection = new SqliteConnection();
			string path1 = "";
			tables1.Clear();
			path1 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value = new SqliteConnection("URI=file:" + path1)) {
					connection = value;
					connection.Open();
					cmnd = connection.CreateCommand();
					cmnd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
					reader = cmnd.ExecuteReader();
					while(reader.Read()) {
						tables1.Add(reader.GetString(0));
					}
				}
			}
			return tables1;
		}

		/// <summary>
		/// Получение списка заголовков в бд
		/// </summary>
		private List<string> sql_headers(string db_name) {
			SqliteCommand cmnd2 = new SqliteCommand();
			SqliteDataReader reader1 = default(SqliteDataReader);
		}

		public void sql_log(string parameter, string parameter2) {}

		public System.Collections.IEnumerator getDelims(string one_table_data) {
			string delim_row = "";
			int delim_rowCount = 10;
			HashSet<int> delim_hash_ints = new HashSet<int>();
			rows_unparsed.Clear();
			//Распилка
			foreach(string loopObject3 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				rows_unparsed.Add(loopObject3);
			}
			yield return new WaitForEndOfFrame();
			//Разделители-столбцы
			if((rows_unparsed.Count < 10)) {
				delim_rowCount = rows_unparsed.Count;
			}
			for(int index1 = 0; index1 < delim_rowCount; index1 += 1) {
				delim_row = rows_unparsed[index1];
				//Бегает по строке - ищет приключений
				for(index2 = delim_row.IndexOfAny(new char[] { 'I', '|', '║', '│' }); index2 > -1; index2 = delim_row.IndexOfAny(new char[] { 'I', '|', '║', '│' }, (index2 + 1))) {
					delim_hash_ints.Add((index2 + 1));
				}
			}
			delimetrs = Enumerable.ToList<System.Int32>(delim_hash_ints);
			delimetrs.Sort();
			yield return new WaitForEndOfFrame();
		}

		public System.Collections.IEnumerator n1(string one_table_data) {
			string row_line = "";
			string row_1th = "";
			string row_2th = "";
			int row_day = 0;
			int row_dayPrev = 0;
			bool row_2th_bool = false;
			string row_cHour = "";
			List<string> row_parsed = new List<string>();
			string row_bd_name = "";
			int row_from = 0;
			int row_length = 0;
			string tmp_4sql_value = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject4 in rows_unparsed) {
				row_line = loopObject4;
				//Получение часа для строк (первая строчка)
				if(Regex.IsMatch(row_line, "поясное.*\\((\\d+)\\D*(\\d+)")) {
					row_1th = Regex.Match(row_line, "поясное.*\\((\\d+)\\D*(\\d+)", RegexOptions.None).Result("$1");
					row_2th = Regex.Match(row_line, "поясное.*\\((\\d+)\\D*(\\d+)", RegexOptions.None).Result("$2");
				} else if(Regex.IsMatch(row_line, "^ *(\\d+)")) {
					row_day = int.Parse(Regex.Match(row_line, "^ *(\\d+)", RegexOptions.None).Result("$1"));
					//поиск текущего часа (первая и вторая половины таблицы)
					if((row_day > row_dayPrev)) {
						row_dayPrev = row_day;
						row_cHour = row_1th;
					} else {
						row_cHour = row_2th;
					}
					//день месяца-первый столбец
					tmp_4sql_value = "" + "y20" + Year + "_m" + Month + "_d" + row_day.ToString() + "_h" + row_cHour;
					//основное тело распарса строки
					for(int index3 = 0; index3 < (delimetrs.Count - 1); index3 += 1) {
						row_from = delimetrs[index3];
						row_length = (delimetrs[(index3 + 1)] - row_from);
						//Проверка на неполную строчку. заполнение @
						if((row_line.Length > row_from)) {
							if((row_line.Length >= (row_from + row_length))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(row_from, row_length).Trim();
							} else {
								//Если нехватает символов в ячейке, но что то есть
								tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(row_from, (row_line.Length - row_from)).Trim();
							}
						} else {
							//если совсем ничего нету
							tmp_4sql_value = tmp_4sql_value + "','" + "@";
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line.Length >= (delimetrs[delimetrs.Count - 1] + (row_line.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim();
						} else {
							//last. если нехватает некоторых символов
							tmp_4sql_value = tmp_4sql_value + "','" + row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim();
						}
					} else {
						//если совсем ничего нету
						tmp_4sql_value = tmp_4sql_value + "','" + "@";
					}
					if(n1.ContainsKey(tmp_4sql_value)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_4sql_value);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						n1.Add(NameOfDB + "&" + Year + "_" + Month + "_" + row_day.ToString() + "_" + row_cHour, tmp_4sql_value);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// sql вставка n1 таблицы в файлы.
		/// </summary>
		private System.Collections.IEnumerator n1_sql(string db_name) {
			int sql_writed = 0;
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteConnection connection1 = new SqliteConnection();
			string path2 = "";
			string q = "";
			int add_new = 0;
			string t_key = "";
			Dictionary<string, string> t_dic = new Dictionary<string, string>();
			string Ntable = "";
			path2 = UnityEngine.Device.Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			if(sql_dbExists(db_name)) {
				using(SqliteConnection value1 = new SqliteConnection("URI=file:" + path2)) {
					connection1 = value1;
					connection1.Open();
					//перебор первой особой таблицы
					foreach(KeyValuePair<string, Dictionary<string, string>> loopObject5 in listOfDicTables) {
						t_dic = loopObject5.Value;
						Ntable = loopObject5.Key;
						//перебор первой особой таблицы
						foreach(string loopObject6 in Enumerable.ToList<System.String>(t_dic.Keys)) {
							t_key = loopObject6;
							sql_writed = 0;
							//поиск совпадений ключей с текущим названием
							if(t_key.Contains(db_name)) {
								q = "INSERT OR IGNORE INTO '" + Ntable + "' " + "" + " VALUES ('" + t_dic[t_key] + "" + "" + "')";
								//Удаляет из библиотеки найденное и совпадённое
								t_dic.Remove(t_key);
								using(SqliteCommand value2 = connection1.CreateCommand()) {
									cmnd1 = value2;
									cmnd1.CommandText = q;
									add_new = cmnd1.ExecuteReader().RecordsAffected;
									sql_writed = (add_new + sql_writed);
									if((add_new == 0)) {
										//воткнуть проверку - на "такие же значения как в бд?"
										Debug.Log("Повтор:" + db_name + "=" + q);
									}
								}
								cmnd1.Cancel();
							}
						}
						Debug.Log("Файл=" + t_key + "=" + "Таблица=" + Ntable + "=, внесено=" + sql_writed.ToString());
					}
				}
				//Пишет что база закрыта, но файл удалить не закрывая редактор всё равно не получается, что за фигня..
				Debug.Log(connection1.State.ToString());
				connection1 = null;
				SqliteConnection.ClearAllPools();
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
			}
		}

		public System.Collections.IEnumerator n2(string one_table_data) {
			string row_line1 = "";
			int row_day1 = 0;
			int row_dayPrev1 = 0;
			bool row_2th_bool1 = false;
			string row_cHour1 = "";
			List<string> row_parsed1 = new List<string>();
			string row_bd_name1 = "";
			int row_from1 = 0;
			int row_length1 = 0;
			string tmp_4sql_value1 = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject7 in rows_unparsed) {
				row_line1 = loopObject7;
				//break on
				if(row_line1.Contains("Средние")) {
					break;
				} else if(Regex.IsMatch(row_line1, "^ *(\\d+)")) {
					row_day1 = int.Parse(Regex.Match(row_line1, "^ *(\\d+)", RegexOptions.None).Result("$1"));
					//день месяца-первый столбец
					tmp_4sql_value1 = "" + "y20" + Year + "_m" + Month + "_d" + row_day1.ToString();
					//основное тело распарса строки
					for(int index4 = 0; index4 < (delimetrs.Count - 1); index4 += 1) {
						row_from1 = delimetrs[index4];
						row_length1 = (delimetrs[(index4 + 1)] - row_from1);
						//Проверка на неполную строчку. заполнение @
						if((row_line1.Length > row_from1)) {
							if((row_line1.Length >= (row_from1 + row_length1))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(row_from1, row_length1).Trim();
							} else {
								//Если нехватает символов в ячейке, но что то есть
								tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(row_from1, (row_line1.Length - row_from1)).Trim();
							}
						} else {
							//если совсем ничего нету
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + "@";
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line1.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line1.Length >= (delimetrs[delimetrs.Count - 1] + (row_line1.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(delimetrs[delimetrs.Count - 1], (row_line1.Length - delimetrs[delimetrs.Count - 1])).Trim();
						} else {
							//last. если нехватает некоторых символов
							tmp_4sql_value1 = tmp_4sql_value1 + "','" + row_line1.Substring(delimetrs[delimetrs.Count - 1], (row_line1.Length - delimetrs[delimetrs.Count - 1])).Trim();
						}
					} else {
						//если совсем ничего нету
						tmp_4sql_value1 = tmp_4sql_value1 + "','" + "@";
					}
					if(n2.ContainsKey(tmp_4sql_value1)) {
						//Если ключ есть. Возможно воткнуть сюда потом генерацию альтернативы, в этом случае
						Debug.Log("Ключ уже есть:" + tmp_4sql_value1);
					} else {
						//Засовываем сразу почти готовую строчку для sql
						n2.Add(NameOfDB + "&" + Year + "_" + Month + "_" + row_day1.ToString(), tmp_4sql_value1);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		/// <summary>
		/// sql вставка n1 таблицы в файлы.
		/// </summary>
		private int n1_sql1(string db_name, string Ntable) {
			int sql_writed1 = 0;
			SqliteCommand cmnd3 = new SqliteCommand();
			SqliteConnection connection3 = new SqliteConnection();
			string path3 = "";
			string q2 = "";
			int add_new1 = 0;
			string t_key1 = "";
		}
	}
}
