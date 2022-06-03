#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using MaxyGames.Generated;

namespace MaxyGames.Generated {
	public class _Config_un : MaxyGames.RuntimeBehaviour {
		public List<KeyValuePair<string, string>> loadedFromConfig = new List<KeyValuePair<string, string>>();
		private string data_adress = "";
		private string data_content = "";
		public WWWForm _w_form = new WWWForm();

		public void Update() {
			if(Input.GetKeyUp(KeyCode.DownArrow)) {
				base.StartCoroutine(DownloadNamesAndIndexs("http://www.pogodaiklimat.ru/archive.php?id=ru"));
			}
		}

		private System.Collections.IEnumerator DownloadSummary(string url) {
			UnityWebRequest uwr = null;
			UnityWebRequest.ClearCookieCache();
			_w_form = new WWWForm();
			_w_form.AddField("submit-login", "post");
			_w_form.AddField("username", "ritor");
			_w_form.AddField("password", "Advenched1");
			uwr = UnityWebRequest.Post(UnityWebRequest.Get(url).uri, _w_form);
			uwr.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0");
			uwr.SetRequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
			uwr.SetRequestHeader("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
			yield return uwr.SendWebRequest();
			if(string.IsNullOrEmpty(uwr.error)) {
				Debug.Log(("Received: " + uwr.downloadHandler.text));
			} else {
				Debug.Log(("Error While Sending: " + uwr.error));
			}
		}

		private System.Collections.IEnumerator LoadFromConfig(string target) {
			loadedFromConfig.Clear();
			foreach(KeyValuePair<string, string> loopObject in Ini.Load("config.ini")) {
				if(loopObject.Key.StartsWith(target)) {
					loadedFromConfig.Add(loopObject);
					Debug.Log(loopObject);
				}
			}
			yield return loadedFromConfig;
		}

		/// <summary>
		/// 2011_names.sqlite
		/// </summary>
		private System.Collections.IEnumerator DownloadNamesAndIndexs(string url) {
			UnityWebRequest uwr2 = null;
			HtmlDocument htmldoc = null;
			Dictionary<string, string> variable2 = null;
			MatchCollection matchess = null;
			string _tmp_adress = "";
			string _c_item_key0 = "";
			string _2sql_groupID = "";
			List<string> _2sqlite_data = new List<string>() { "0,0,0" };
			string q = "";
			UnityWebRequest.ClearCookieCache();
			_2sqlite_data.Clear();
			data_content = "";
			base.StartCoroutine(DownloadPage("http://www.pogodaiklimat.ru/archive.php?id=" + "ru"));
			while(!((data_adress.Equals("http://www.pogodaiklimat.ru/archive.php?id=" + "ru") && (data_content != "")))) {
				yield return new WaitForSeconds(1F);
				Debug.Log("ждём");
			}
			matchess = Regex.Matches(data_content, "<a href=\"\\/archive.php\\?(?'n1'.*)\">(?'n2'.*)<\\/a>");
			data_content = "";
			foreach(Match loopObject1 in matchess) {
				_c_item_key0 = loopObject1.Groups[1].ToString();
				_2sql_groupID = _c_item_key0.Split(new char[] { '=' })[2];
				_2sqlite_data.Add(_2sql_groupID + "\", \"" + loopObject1.Groups[2].ToString() + "\", \"" + "999999999");
				_tmp_adress = "http://www.pogodaiklimat.ru/archive.php?" + _c_item_key0;
				base.StartCoroutine(DownloadPage(_tmp_adress));
				while(!((data_adress.Equals(_tmp_adress) && (data_content != "")))) {
					yield return new WaitForSeconds(1F);
					Debug.Log("ждём2");
				}
				foreach(Match loopObject2 in Regex.Matches(data_content, "<a href=\"\\/weather.php\\?id=(?'n1'.*)\">(?'n2'.*)<\\/a>")) {
					_2sqlite_data.Add(loopObject2.Groups[1].ToString() + "\", \"" + loopObject2.Groups[2].ToString() + "\", \"" + _2sql_groupID);
				}
				Debug.Log(_2sql_groupID + new sqlite() { variable4 = null }.InsertQueryTable("2011_names", q).ToString());
			}
			q = "REPLACE INTO \"" + "ru" + "\" (\"index\",\"name\",\"group\") " + "VALUES (\"" + string.Join<System.String>("\"),(\"", _2sqlite_data).Replace("|", "\",\"") + "\")";
			Debug.Log(string.Join<System.String>("\"),(\"", _2sqlite_data));
			yield break;
		}

		private System.Collections.IEnumerator DownloadPage(string url) {
			UnityWebRequest uwr1 = null;
			UnityWebRequest.ClearCookieCache();
			using(UnityWebRequest value = UnityWebRequest.Get(url)) {
				uwr1 = value;
				uwr1.timeout = 10;
				uwr1.SendWebRequest();
				while(!(uwr1.isDone)) {
					yield return new WaitForEndOfFrame();
				}
				if((uwr1.result == UnityWebRequest.Result.Success)) {
					data_adress = url;
					data_content = uwr1.downloadHandler.text;
					yield return null;
				} else {
					Debug.Log("нескачано ");
					yield return new WaitForEndOfFrame();
				}
			}
			yield return null;
		}

		public void WindowSize() {}
	}
}
