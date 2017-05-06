using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CountDown : MonoBehaviour
{

	public Text text;

	public float timeLeft;
	bool startCountDown;
	public Action countDownDone;
	// Use this for initialization
	void Start ()
	{
		text.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (startCountDown) {
			timeLeft -= Time.deltaTime;
			text.text = "" + (int)timeLeft;
			if (timeLeft < 1) {
				timeLeft = 4;
				startCountDown = false;
				text.enabled = false;
				countDownDone ();
			}
		}
	}

	public void Start_CountDown ()
	{
		text.enabled = true;
		startCountDown = true;

	}
}
