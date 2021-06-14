#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;

namespace MaxyGames.Generated {
	public class pogodaiklimat2011 : MaxyGames.RuntimeBehaviour {
		public KeyValuePair<string, string> linkFromConfig = new KeyValuePair<string, string>();
		public UnityEngine.UI.InputField _index = null;
		public UnityEngine.UI.InputField _year = null;
		public Dictionary<string, string> linksDictArray = new Dictionary<string, string>();
		public string _switch = "";
		public string _folderAsIndex = "";
		public string _filesInFolders = "";
		public Dictionary<string, string> linksToDownload = new Dictionary<string, string>();
		public bool TryAgain = false;

		public void StartPogodaklimat2011() {
			base.StartCoroutine(_function_group());
		}

		public System.Collections.IEnumerator _function_group() {
			float timelimit = 0.2F;
			WaitForSeconds variable11 = null;
			int variable2 = 0;
			base.StartCoroutine(_filesCheck());
			yield return new WaitForSeconds(timelimit);
			base.StartCoroutine(LoadFromConfig());
			yield return new WaitForSeconds(timelimit);
			if(_switch.Equals("LoadFromConfig")) {
				base.StartCoroutine(LinkGeneration());
				yield return new WaitForSeconds(timelimit);
				if(_switch.Equals("LinkGeneration")) {
					DownloadGeneration();
					yield return new WaitForSeconds(timelimit);
					if(_switch.Equals("DownloadGeneration")) {
						do {
							_switch = "";
							if((variable2 > 3F)) {
								Debug.Log("Не скачивается!");
								yield break;
							} else {
								base.StartCoroutine(DownLoadFiles());
								yield return new WaitUntil(() => _switch == "DownLoadFiles");
								variable2 = (variable2 + 1);
								Debug.Log("итерация");
							}
						} while(TryAgain);
						Debug.Log("скачано всё");
					} else {
						Debug.Log("DownloadGeneration -> завис??");
					}
				} else {
					Debug.Log("LinkGeneration > 0.2 sec!!!");
				}
			} else {
				Debug.Log("LoadFromConfig > 0.2 sec!!!");
			}
			yield break;
		}

		public System.Collections.IEnumerator LoadFromConfig() {
			linkFromConfig = new KeyValuePair<string, string>();
			foreach(KeyValuePair<string, string> loopObject in Ini.Load("config.ini")) {
				if(loopObject.Key.StartsWith("pogodaiklimat2011")) {
					linkFromConfig = loopObject;
					break;
				}
			}
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				Debug.Log("pogodaiklimat2011 нету!");
			} else {
				Debug.Log(linkFromConfig);
				_switch = "LoadFromConfig";
			}
			yield break;
		}

		public System.Collections.IEnumerator LinkGeneration() {
			string _base = "";
			int curMonth = 0;
			int _monthMax = 13;
			string _link_index = "";
			string _link_year = "";
			string _link_month = "";
			string _link_day_value = "";
			string _Zday = "";
			string _today = "";
			string _t_index = "";
			int _t_year = 0;
			int _t_month = 0;
			string tmps = "";
			string _link_key = "";
			int curYear = 0;
			linksDictArray.Clear();
			curYear = System.DateTime.Now.Year;
			curMonth = System.DateTime.Now.Month;
			_today = System.DateTime.Now.Day.ToString();
			_base = "";
			if(string.IsNullOrEmpty(linkFromConfig.Value)) {
				Debug.Log("pogodaiklimat2011 нету!");
			} else {
				_base = linkFromConfig.Value;
				//id=index
				foreach(string loopObject1 in _indexArray()) {
					_t_index = loopObject1;
					_link_index = _base.Replace("$id", _t_index);
					//set year
					foreach(string loopObject2 in _yearArray()) {
						_t_year = int.Parse(loopObject2);
						_link_year = _link_index.Replace("$year", _t_year.ToString());
						//set month
						if(!((_t_year < System.DateTime.Now.Year))) {
							_monthMax = (curMonth + 1);
						}
						for(int index = 1; index < _monthMax; index += 1) {
							_t_month = index;
							_link_month = _link_year.Replace("$month", _t_month.ToString());
							//set last day in month
							if(((curYear == _t_year) && (curMonth == _t_month))) {
								_Zday = _today;
							} else {
								_Zday = System.DateTime.DaysInMonth(_t_year, _t_month).ToString();
							}
							_link_day_value = _link_month.Replace("$Zday", _Zday);
							//add link to dictionary array
							linksDictArray.Add(_t_index + "_" + _t_year.ToString() + "_" + _t_month.ToString(), _link_day_value);
						}
					}
				}
			}
			_switch = "LinkGeneration";
			yield break;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _indexArray() {
			List<string> data_index = new List<string>();
			int variable1 = 0;
			data_index = new List<string>();
			foreach(string loopObject3 in Regex.Split(_index.text, "\\D", RegexOptions.Multiline)) {
				if(int.TryParse(loopObject3, out variable1)) {
					data_index.Add(loopObject3);
				}
			}
			return data_index;
		}

		/// <summary>
		/// Get data from input field
		/// </summary>
		public List<string> _yearArray() {
			List<string> data_year = null;
			string year_2_parse = "";
			int _yearPlus = 0;
			int _year_from = 0;
			int _year_to = 0;
			string[] _year_splited = new string[0];
			int _year_casual = 0;
			int curYearPlus1 = 0;
			curYearPlus1 = (System.DateTime.Now.Year + 1);
			data_year = new List<string>();
			year_2_parse = _year.text;
			if(year_2_parse.Contains("+")) {
				if(int.TryParse(Regex.Replace(year_2_parse, ".*([0-9]{4})\\+.*", "$1", RegexOptions.Multiline, System.TimeSpan.FromMilliseconds(500D)), out _yearPlus)) {
					if(((_yearPlus > 2010F) && (_yearPlus < curYearPlus1))) {
						for(int index1 = _yearPlus; index1 < curYearPlus1; index1 += 1) {
							data_year.Add(index1.ToString());
							Debug.Log(index1);
						}
						if((data_year.Capacity > 0)) {
							return data_year;
						} else {
							Debug.Log("Есть года, но не в том диапазоне");
						}
					} else {
						Debug.Log("Есть +, но нету валидного года рядом с плюсом");
					}
				} else {
					Debug.Log("Есть +, но нету 4значного числа");
				}
			} else if(year_2_parse.Contains("-")) {
				_year_splited = Regex.Replace(year_2_parse, ".*([0-9-]{9}).*", " $1 ").Split(new char[] { '-' });
				_year_from = int.Parse(_year_splited[0]);
				_year_to = int.Parse(_year_splited[1]);
				if((((_year_from > 2010F) && (_year_to < curYearPlus1)) && (_year_to > _year_from))) {
					for(int index2 = _year_from; index2 <= _year_to; index2 += 1) {
						data_year.Add(index2.ToString());
						Debug.Log(index2);
					}
					return data_year;
				} else {
					Debug.Log("from_to_Года не соответствуют: " + year_2_parse);
				}
			} else {
				foreach(string loopObject4 in Regex.Split(year_2_parse, "\\D", RegexOptions.Multiline)) {
					if(int.TryParse(loopObject4, out _year_casual)) {
						if(((_year_casual > 2010F) && (_year_casual < curYearPlus1))) {
							Debug.Log(_year_casual);
							data_year.Add(_year_casual.ToString());
						}
					}
				}
				if((data_year.Capacity > 0F)) {
					data_year.Sort();
					return data_year;
				} else {
					Debug.Log("casual_Года не соответствуют: " + year_2_parse);
				}
			}
			Debug.Log("11111");
			return data_year;
		}

		/// <summary>
		/// Создание списка ссылок действительно нуждающихся в скачивании. 
		/// </summary>
		public void DownloadGeneration() {
			string _t_index1 = "";
			string _t_year1 = "";
			string _t_month1 = "";
			string _current_index = "";
			string _key = "";
			string _value = "";
			linksToDownload.Clear();
			if((linksDictArray.Count > 0)) {
				foreach(KeyValuePair<string, string> loopObject5 in linksDictArray) {
					_value = loopObject5.Value;
					_key = loopObject5.Key;
					_t_index1 = _key.Split(new char[] { '_' })[0];
					if(_folderAsIndex.Contains(_t_index1)) {
						//если существует  папка=индекс
						if(!(_filesInFolders.Contains(_key))) {
							linksToDownload.Add(_key, _value);
						}
					} else {
						linksToDownload.Add(_key, _value);
					}
				}
			} else {
				Debug.Log("pogodaiklimat2011_DownloadGeneration ->Ссылок нету");
			}
			_switch = "DownloadGeneration";
		}

		/// <summary>
		/// Проверка на уже существующие файлы.  + перескачивание последнего из уже ранее скачанных месяцев-файлов
		/// </summary>
		public System.Collections.IEnumerator _filesCheck() {
			DirectoryInfo[] _dirs = new DirectoryInfo[0];
			FileInfo[] _files = new FileInfo[0];
			int _last_file = 0;
			string _name = "";
			_filesInFolders = "";
			_folderAsIndex = "";
			if(new DirectoryInfo(Application.streamingAssetsPath + "\\files\\pogodaiklimat2011\\").Exists) {
				_dirs = new DirectoryInfo(Application.streamingAssetsPath + "\\files\\pogodaiklimat2011\\").GetDirectories();
				foreach(DirectoryInfo loopObject6 in _dirs) {
					//Имена папок=индексов в один стринг. Что бы потом проверять, есть или нету
					_folderAsIndex = _folderAsIndex + "|" + loopObject6.Name;
					_files = new DirectoryInfo(Application.streamingAssetsPath + "\\files\\pogodaiklimat2011\\" + loopObject6.Name).GetFiles();
					foreach(FileInfo loopObject7 in _files) {
						_name = loopObject7.Name;
						//Проверка на расширение файла
						if(_name.EndsWith("html")) {
							_last_file = _name.Length;
							//Имена файлов в индексах  в один стринг. Что бы потом проверять, есть или нету
							_filesInFolders = _filesInFolders + "|" + _name;
						}
					}
					_filesInFolders = _filesInFolders.Remove((_filesInFolders.Length - _last_file));
				}
			} else {
				//затычка
				Debug.Log("_files_Check: \\files\\pogodaiklimat2011\\ НЕТУ");
			}
			Debug.Log(_folderAsIndex);
			Debug.Log(_filesInFolders);
			yield break;
		}

		/// <summary>
		/// Тут будет скачивание.
		/// </summary>
		public System.Collections.IEnumerator DownLoadFiles() {
			string _key1 = "";
			string _v_link = "";
			string folder_path = "";
			UnityWebRequest uwr = null;
			string file_path = "";
			Stream _file = null;
			int CountTry = 0;
			TryAgain = false;
			UnityWebRequest.ClearCookieCache();
			foreach(KeyValuePair<string, string> loopObject8 in linksToDownload) {
				_key1 = loopObject8.Key;
				_v_link = loopObject8.Value;
				folder_path = Application.streamingAssetsPath + "\\files\\pogodaiklimat2011\\" + _key1.Split(new char[] { '_' })[0];
				if(!(Directory.Exists(folder_path))) {
					Directory.CreateDirectory(folder_path);
				}
				using(UnityWebRequest value = UnityWebRequest.Get(_v_link)) {
					uwr = value;
					//wait up to one second to download the image
					uwr.timeout = 1;
					uwr.SendWebRequest();
					while(!(uwr.isDone)) {
						if((uwr.downloadedBytes > 0UL)) {
							yield return new WaitForEndOfFrame();
						}
					}
					yield return new WaitForSeconds(1F);
					if((uwr.result == UnityWebRequest.Result.Success)) {
						file_path = folder_path + "\\" + _key1 + ".html";
						_file = File.Open(file_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
						yield return _file;
						_file.Close();
						File.WriteAllText(file_path, uwr.downloadHandler.text);
						Debug.Log("Скачан: " + _key1);
					} else {
						Debug.Log(uwr.result.ToString() + ":! " + uwr.error + " | " + _key1);
						TryAgain = true;
						CountTry = (CountTry + 1);
						if((CountTry > 3F)) {
							_switch = "DownLoadFiles";
							Debug.Log("Много не скачанных файлов");
							yield break;
						}
					}
				}
			}
			_switch = "DownLoadFiles";
			yield break;
		}
	}
}
