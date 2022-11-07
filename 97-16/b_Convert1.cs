#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MaxyGames.Generated {
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		private Match cachedValue;
		private int index1;

		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {}
		}

		public void loadFromFiles() {
			string path = "tmp.txt";
			string file_data = "";
			System.Array tables = new string[0];
			string One_table_data = "";
			string N_table = "";
			string N_year = "";
			string N_month = "";
			path = Application.streamingAssetsPath + "/" + "tmp.txt";
			file_data = File.ReadAllText(path);
			tables = file_data.Split("Табли", System.StringSplitOptions.RemoveEmptyEntries);
			new _utillz()._2log("Количесто таблиц в файле: " + tables.Length.ToString(), false);
			One_table_data = tables.GetValue(1).ToString();
			//Get Number of table
			N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\.\\D*\\n", RegexOptions.None).Result("$1");
			cachedValue = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None);
			N_year = cachedValue.Result("$2");
			N_month = cachedValue.Result("$1");
			parseRow(One_table_data, _alllndexOfDelimeters(One_table_data), N_table, N_year, N_month);
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		private List<int> _alllndexOfDelimeters(string table_data) {
			List<int> row_indexs_delimeters = new List<int>();
			string row = "";
			row_indexs_delimeters.Clear();
			//Берёт только первые строки - шапку
			for(int index = 0; index < 20; index += 1) {
				row = (table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries).GetValue(index) as string);
				//Бегает по строке - ищет приключений
				for(index1 = row.IndexOfAny(new char[] { '╦', '┬' }); index1 > -1; index1 = row.IndexOfAny(new char[] { '┬', '╦' }, (index1 + 1))) {
					row_indexs_delimeters.Add(index1);
				}
			}
			return row_indexs_delimeters;
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		public List<List<string>> parseRow(string table_data, List<int> row_indexs_delimeters, string N_table, string N_year, string N_month) {
			string tokenToSplitBy = "|";
			int insCount = -1;
			string line = "";
			int from = 0;
			int length = 0;
			int item = 0;
			List<string> _rowParsed = new List<string>();
			string N_name = "";
			row_indexs_delimeters.Sort();
			foreach(string loopObject in table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				line = loopObject;
				if(Regex.IsMatch(line.Trim(), "^\\d{1,3}\\.")) {
					_rowParsed.Clear();
					//пихаем в начало год и месяц
					_rowParsed.Add(N_year);
					_rowParsed.Add(N_month);
					//Получаем название поста=название файла бд. с номером
					N_name = new _utillz()._ConvertStringRusLat(line.Substring(0, row_indexs_delimeters[0]).Trim());
					//1й вариант. вроде чуть быстрее ~15 секунд. против 19ти
					for(int index2 = 0; index2 < (row_indexs_delimeters.Count - 1); index2 += 1) {
						from = row_indexs_delimeters[index2];
						length = (row_indexs_delimeters[(index2 + 1)] - from);
						_rowParsed.Add(line.Substring(from, length).Trim());
					}
					//последний столбец
					_rowParsed.Add(line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
				}
			}
			return new List<List<string>>();
		}
	}
}
