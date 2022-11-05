#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MaxyGames.Generated {
	public class b_Convert1 : MaxyGames.RuntimeBehaviour {
		private int index;

		private void Update() {
			string variable0 = "";
			if(Input.GetKeyUp(KeyCode.UpArrow)) {}
		}

		public void loadFromFiles() {
			string path = "tmp.txt";
			System.Array file_dataL = default(System.Array);
			string file_data = "";
			System.Array tables = new string[0];
			string One_table_data = "";
			string N_table = "";
			string N_month = "";
			string N_year = "";
			path = Application.streamingAssetsPath + "/" + "tmp.txt";
			file_data = File.ReadAllText(path);
			tables = file_data.Split("Табли", System.StringSplitOptions.RemoveEmptyEntries);
			new _Log()._2log("Количесто таблиц в файле: " + tables.Length.ToString(), true);
			One_table_data = tables.GetValue(1).ToString();
			//Get Number of table
			N_table = Regex.Match(One_table_data, "ца\\D*(\\d+)\\.\\D*\\n", RegexOptions.None).Result("$1");
			N_month = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None).Result("$1");
			N_year = Regex.Match(One_table_data, "Месяц\\D*(\\d+)\\D*Год\\D*(\\d+)", RegexOptions.None).Result("$2");
			_allIndexof(One_table_data);
		}

		/// <summary>
		/// Extract all indexes of clmn delimiters
		/// </summary>
		public List<int> _allIndexof(string data) {
			List<int> indexs = default(List<int>);
			int i = 0;
			int finded = 0;
			List<int> result = new List<int>();
			//Делит таблицу на строки
			foreach(string loopObject in data.Split(System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)) {
				//Бегает по строке - ищет приключений
				for(index = loopObject.IndexOfAny(new char[] { '╦', '┬' }); index > -1; index = loopObject.IndexOfAny(new char[] { '┬', '╦' }, (index + 1))) {
					new _Log()._2log(index.ToString(), false);
				}
			}
			return result;
		}
	}
}
