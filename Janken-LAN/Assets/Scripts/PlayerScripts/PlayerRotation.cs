using UnityEngine;
using System.Collections;

public class PlayerRotation : MonoBehaviour
{

	// Use this for initialization

	
	// Update is called once per frame
	void Update ()
	{
		if (transform.parent.GetComponent<PlayerController> () != null) {
			//Pull the rotation direction from Controller script
			Vector3 rotDir = transform.parent.GetComponent<PlayerController> ().rotationDir;
			if (rotDir != Vector3.zero) {
				//Calculate amount of rotation in degrees
				Quaternion newRot = Quaternion.LookRotation (rotDir);

				//Apply Rotation and lock rotation along X and Z axis
				transform.localRotation = Quaternion.Euler (0, newRot.eulerAngles.y, 0);
			}
		}
	}
}
