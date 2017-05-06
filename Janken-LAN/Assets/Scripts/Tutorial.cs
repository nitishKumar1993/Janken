using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{

	public float time;

	public GameObject JoyStick;
	public GameObject PowerUp;
	bool displayed;
	public GameObject Speed;
	bool speed;
	public GameObject Shield;
	bool shield;
	public GameObject Freeze;
	bool freeze;
	public GameObject Death;
	bool death;

	public static Tutorial Instance;
	// Use this for initialization
	void Start ()
	{
		Instance = this;
	}

	public void Tut_PowerUp (PowerType type)
	{
		switch (type) {
		case PowerType.Launcher_Freeze:
			Tut_Freeze ();
			break;

		case PowerType.Buff_Shield:
			Tut_Shield ();
			break;

		case PowerType.Buff_Speed:
			Tut_Speed ();
			break;

		case PowerType.Launcher_Skull:
			Tut_Death ();
			break;
		}
	}

	void Tut_Speed ()
	{
		if (speed)
			return;
		speed = true;
		Tut_Enable (Speed);
	}

	void Tut_Shield ()
	{
		if (shield)
			return;
		shield = true;
		Tut_Enable (Shield);
	}

	void Tut_Freeze ()
	{
		if (freeze)
			return;
		freeze = true;
		Tut_Enable (Freeze);
	}

	void Tut_Death ()
	{
		if (death)
			return;
		death = true;
		Tut_Enable (Death);
	}


	public void Tut_PowerBox ()
	{
		if (displayed)
			return;
		displayed = true;

		Tut_Enable (PowerUp);
	}

	public void Tut_JoyStick ()
	{
		Tut_Enable (JoyStick);
	}

	void Tut_Enable (GameObject obj)
	{
		obj.SetActive (true);
		StartCoroutine (Disable (obj));
	}

	IEnumerator Disable (GameObject obj)
	{
		yield return new WaitForSeconds (time);
		obj.SetActive (false);
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}
