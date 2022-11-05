#pragma warning disable
using UnityEngine;
using System.Collections.Generic;

public class _Log : object {

	public void _2log(string text2log, bool error) {
		string time = "";
		if(error) {
			time = "<color=#ff0000>" + System.DateTime.Now.ToString("HH:mm:ss") + "</color>" + ": ";
		} else {
			time = "<size=80%>" + System.DateTime.Now.ToString("HH:mm:<u>ss<\\/u>") + "</size>" + ": ";
		}
		GameObject.Find("Log").GetComponent<TMPro.TMP_InputField>().text = time + "" + text2log + System.Environment.NewLine + GameObject.Find("Log").GetComponent<TMPro.TMP_InputField>().text;
		Debug.Log(text2log);
	}

	public System.Collections.IEnumerator _log2file() {
		return null;
	}
}
