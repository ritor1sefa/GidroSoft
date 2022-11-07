#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MaxyGames.Generated {
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		private int index1;

		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {
				new _utillz()._2log(new _utillz()._ConvertStringRusLat("проВе ро+++=Чка"), false);
			}
		}

		public void loadFromFiles() {
			string path = "tmp.txt";
			string file_data = "";
			System.Array tables = new string[0];
			string One_table_data = "";
			string N_table = "";
			string N_year_N_month = "";
			path = Application.streamingAssetsPath + "/" + "tmp.txt";
			file_data = File.ReadAllText(path);
			tables = file_data.Split("Табли", System.StringSplitOptions.RemoveEmptyEntries);
			new _utillz()._2log("Количесто таблиц в файле: " + tables.Length.ToString(), false);
			One_table_data = tables.GetValue(1).ToString();
			//Get Number of table
			N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\.\\D*\\n", RegexOptions.None).Result("$1");
			N_year_N_month = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None).Result("$2_$1");
			parseRow(One_table_data, _alllndexOfDelimeters(One_table_data));
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
		public List<List<string>> parseRow(string table_data, List<int> row_indexs_delimeters) {
			string tokenToSplitBy = "|";
			int insCount = -1;
			string line = "";
			int from = 0;
			int length = 0;
			int item = 0;
			List<List<string>> oneTableParsed = new List<List<string>>();
			List<string> _rowParsed = new List<string>();
			row_indexs_delimeters.Sort();
			oneTableParsed.Clear();
			foreach(string loopObject in table_data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				line = loopObject;
				if(Regex.IsMatch(line.Trim(), "^\\d{1,3}\\.")) {
					_rowParsed.Clear();
					_rowParsed.Add(line.Substring(0, row_indexs_delimeters[0]).Trim());
					//1й вариант. вроде чуть быстрее ~15 секунд. против 19ти
					for(int index2 = 0; index2 < (row_indexs_delimeters.Count - 1); index2 += 1) {
						from = row_indexs_delimeters[index2];
						length = (row_indexs_delimeters[(index2 + 1)] - from);
						_rowParsed.Add(line.Substring(from, length).Trim());
					}
					_rowParsed.Add(line.Substring(row_indexs_delimeters[row_indexs_delimeters.Count - 1], (line.Length - row_indexs_delimeters[row_indexs_delimeters.Count - 1])).Trim());
					oneTableParsed.Add(_rowParsed);
				}
			}
			return oneTableParsed;
		}
	}
}
