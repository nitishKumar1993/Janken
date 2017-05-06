using UnityEngine;
using System.Collections;

public class Powers : MonoBehaviour
{



	// Use this for initialization
	void Start ()
	{
		DisableAllButtons ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void DisableAllButtons ()
	{
		foreach (Transform t in this.gameObject.transform) {

			t.gameObject.SetActive (false);
		}
	}


	public void ActivatePowerButton (PowerType pt)
	{
		if (pt == PowerType.None)
			DisableAllButtons ();
		else {
			foreach (Transform t in this.gameObject.transform) {
				if (t.GetComponent <PowerInfo> ().powerType == pt) {
					
					t.gameObject.SetActive (true);
					t.gameObject.transform.localRotation = Quaternion.identity;

					Tutorial.Instance.Tut_PowerUp (pt);
				} else
					t.gameObject.SetActive (false);
			}
		}
	}

}
