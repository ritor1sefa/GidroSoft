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
		public string Year = "";
		public string Month = "";
		public Dictionary<string, List<string>> n1 = new Dictionary<string, List<string>>();
		public GameObject objectVariable;
		public GameObject objectVariable1;
		public GameObject objectVariable2;
		private int index2;
		private List<int> delimetrs1 = new List<int>();

		private void Update() {
			string variable0 = "";
		}

		public void button() {
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
			for(int index = 0; index < Files.Length; index += 1) {
				fPath = Files[index];
				objectVariable1.gameObject.GetComponent<TMPro.TMP_Text>().text = fPath;
				objectVariable2.gameObject.GetComponent<TMPro.TMP_Text>().text = (index + 1).ToString();
				yield return selector_loop(fPath);
				Debug.Log("След файл");
			}
			Debug.Log("!Закончено!");
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
				//название поста, добавить сюда же потом прохд через bd_names
				NameOfDB = Regex.Match(Path.GetFileName(path), "(^\\D*)(\\d+)-(\\d+)").Result("$1").Trim();
				file_data = File.ReadAllText(path);
				tables = Enumerable.ToList<System.String>(file_data.Trim().Split("Станция", System.StringSplitOptions.RemoveEmptyEntries));
				foreach(string loopObject1 in tables) {
					One_table_data = loopObject1;
					if(One_table_data.Contains("Атм. давление,")) {
						Debug.Log("=1");
						//11
						yield return n1(One_table_data);
					} else if(One_table_data.Contains("С У Т О Ч Н Ы Е   Д А Н Н Ы Е")) {
						Debug.Log(One_table_data);
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
		private bool sql_connect(string db_name) {
			string path = "";
			SqliteConnection connection = default(SqliteConnection);
			path = Application.streamingAssetsPath + "/" + "files/bd/" + db_name + ".sqlite";
			//Копирование пустой бд в новый файл
			if(!(File.Exists(path))) {
				File.Copy(Application.streamingAssetsPath + "/" + "files/" + "_emptyNew" + ".sqlite", path, false);
			}
			connection = new SqliteConnection("URI=file:" + path);
			connection.Open();
			return File.Exists(path);
		}

		/// <summary>
		/// нифига не работает почему то, на большом количестве разных таблиц.
		/// </summary>
		public void sql_close() {}

		/// <summary>
		/// Получение списка таблиц в бд
		/// Ненужен?
		/// </summary>
		private List<string> sql_master_tables(string db_name) {
			List<string> tables1 = new List<string>();
		}

		/// <summary>
		/// Выполняет одну вставку
		/// </summary>
		private int sql_insertQ(string db_name, string q) {
			int sql_writed = 0;
			SqliteCommand cmnd = new SqliteCommand();
		}

		/// <summary>
		/// Получение списка таблиц в бд
		/// </summary>
		private List<string> sql_headers(string db_name) {
			SqliteCommand cmnd1 = new SqliteCommand();
			SqliteDataReader reader = default(SqliteDataReader);
		}

		public void sql_log(string parameter, string parameter2) {}

		public System.Collections.IEnumerator getDelims(string one_table_data) {
			string delim_row = "";
			int delim_rowCount = 10;
			HashSet<int> delim_hash_ints = new HashSet<int>();
			rows_unparsed.Clear();
			//Распилка
			foreach(string loopObject2 in one_table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				rows_unparsed.Add(loopObject2);
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
			List<string> sql_row = new List<string>();
			string sql_bd_name = "";
			string sql_q = "";
			yield return getDelims(one_table_data);
			//построчная обработка
			foreach(string loopObject3 in rows_unparsed) {
				row_line = loopObject3;
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
					row_parsed.Add(row_day.ToString());
					//основное тело распарса строки
					for(int index3 = 0; index3 < (delimetrs.Count - 1); index3 += 1) {
						row_from = delimetrs[index3];
						row_length = (delimetrs[(index3 + 1)] - row_from);
						//Проверка на неполную строчку. заполнение @
						if((row_line.Length > row_from)) {
							if((row_line.Length >= (row_from + row_length))) {
								//Если совсем всё в порядке и вся ячейка что то имеет
								row_parsed.Add(row_line.Substring(row_from, row_length).Trim());
							} else {
								//Если нехватает символов в ячейке, но что то есть
								row_parsed.Add(row_line.Substring(row_from, (row_line.Length - row_from)).Trim());
							}
						} else {
							//если совсем ничего нету
							row_parsed.Add("@");
						}
					}
					//Проверка на неполную строчку. заполнение @
					if((row_line.Length > delimetrs[delimetrs.Count - 1])) {
						if((row_line.Length >= (delimetrs[delimetrs.Count - 1] + (row_line.Length - delimetrs[delimetrs.Count - 1])))) {
							//last. если все символы на месте.
							row_parsed.Add(row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim());
						} else {
							//last. если нехватает некоторых символов
							row_parsed.Add(row_line.Substring(delimetrs[delimetrs.Count - 1], (row_line.Length - delimetrs[delimetrs.Count - 1])).Trim());
						}
					} else {
						//last. если ячейка совсем пустая
						row_parsed.Add("@");
					}
					//Добавление в Дикт.
					n1.Add(NameOfDB + "_" + Year + "_" + Month + "_" + row_day.ToString() + "_" + row_cHour, row_parsed);
					row_parsed = new List<string>();
					//еслипрямь и дубликата дубликат есть, жжесть
					Debug.Log(n1.Count);
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
