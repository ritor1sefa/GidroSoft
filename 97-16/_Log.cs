#pragma warning disable
using UnityEngine;
using System.Collections.Generic;

public class _Log : object {

	public void _2log(object text2log, object error) {
		object time = "";
		if(error) {
			time = "<color=#ff0000>" +  + "</color>" + ": ";
		} else {
			time = "<size=80%>" +  + "</size>" + ": ";
		}
	}

	public void _log2file() {
		return null;
	}
}
