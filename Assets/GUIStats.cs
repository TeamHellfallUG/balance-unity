using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIStats : MonoBehaviour {

	BalanceUnity bu ;
	float deltaTime = 0.0f;

	// Use this for initialization
	void Start () {
		bu = BalanceUnity.ACCESS ();
	}
	
	// Update is called once per frame
	void Update () {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI(){
		
		int w = Screen.width;
		int h = Screen.height;

		int height = h * 2 / 70;

		GUIStyle style = new GUIStyle ();
		style.alignment = TextAnchor.UpperRight;
		style.fontSize = height;
		style.normal.textColor = new Color (200.0f, 200.0f, 220.0f);

		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;

		string text = string.Format ("{0:0.0} ms ({1:0.}) fps", msec, fps);
		Rect rect = new Rect (0, 0, w, height);
		GUI.Label (rect, text, style);

		BUStats stats = bu.GetStats ();
		string stext = string.Format ("pings: {0} ws, {1} udp", stats.WsPing, stats.UdpPing);
		Rect rect2 = new Rect (0, height, w, height);
		GUI.Label (rect2, stext, style);
	}
}
