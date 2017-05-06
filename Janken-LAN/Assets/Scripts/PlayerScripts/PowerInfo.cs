using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerInfo : MonoBehaviour
{

	public PowerType powerType;
	public bool selfDestroy;
	public float selfDestroyTime;
	public float gravity = -9.8f;

	public bool applyPhysics;
	Vector3 gravityUp;
	public  Transform planet;
	Rigidbody rigidbody;
	// Use this for initialization
	void Awake ()
	{
		if (applyPhysics) {
			
			planet = GameObject.FindGameObjectWithTag ("Planet").GetComponent<Transform> ();

			rigidbody = GetComponent<Rigidbody> ();
	
			rigidbody.useGravity = false;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (selfDestroy)
			Destroy (this.gameObject, selfDestroyTime);

		if (applyPhysics) {
			
		
			//Calculate gravity direction
			gravityUp = (transform.position - planet.position).normalized;
			Vector3 localUp = transform.up;



			// Align player's up axis with the centre of planet
			transform.rotation = Quaternion.FromToRotation (localUp, gravityUp) * transform.rotation;
		}
	}

	void FixedUpdate ()
	{
		if (applyPhysics) {

			// Apply downwards gravity to body
			rigidbody.AddForce (gravityUp * gravity);
		}
	}

	public void NetworkDestroy ()
	{
		NetworkServer.Destroy (this.gameObject);
	}


}
