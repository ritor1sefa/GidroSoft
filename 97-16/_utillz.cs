#pragma warning disable
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class _utillz : object {

	public void _2log(string text2log, bool error) {
		string time = "";
		if(error) {
			time = "<color=#ff0000>" + System.DateTime.Now.ToString("HH:mm:ss") + "</color>" + ": ";
		} else {
			time = "<size=80%>" + System.DateTime.Now.ToString("HH:mm:<u>ss<\\/u>") + "</size>" + ": ";
		}
		GameObject.Find("log_window").GetComponent<TMPro.TMP_InputField>().text = time + "" + text2log + System.Environment.NewLine + GameObject.Find("log_window").GetComponent<TMPro.TMP_InputField>().text;
	}
}
