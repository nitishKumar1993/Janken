using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PowersManager : NetworkBehaviour
{
	public GameObject obj_SpawnPoints;
	public List<Transform> SpawnPoints;
	public GameObject PowerUp;
	public GameObject Launcher_Freeze;
	public GameObject Launcher_Skull;
	public float spawnStartTime;
	public float spawnInterval;
	public static PowersManager Instance;

	// Use this for initialization
	void Start ()
	{
		Instance = this;
		if (isServer) {
			foreach (Transform t in obj_SpawnPoints.transform) {
				SpawnPoints.Add (t);
			}
		}

		InvokeRepeating ("Spawn", spawnStartTime, spawnInterval);

	}

	void Spawn ()
	{
		foreach (Transform t in SpawnPoints) {
			if (isServer) {
				GameObject ob = (GameObject)Instantiate (PowerUp, t.position, Quaternion.identity);
				PowerInfo info = ob.GetComponent<PowerInfo> ();

				int random = Random.Range (1, 5);
				switch (random) {
				case 1:
					info.powerType = PowerType.Buff_Speed;
					break;

				case 2:
					info.powerType = PowerType.Buff_Shield;
					break;
				case 3:
					info.powerType = PowerType.Launcher_Freeze;
					break;

				case 4:
					info.powerType = PowerType.Launcher_Skull;
					break;
				}

				//info.powerType = PowerType.Buff_Speed;
				info.selfDestroy = true;
				info.selfDestroyTime = 10.0f;

				NetworkServer.Spawn (ob);
			}
		}
		Tutorial.Instance.Tut_PowerBox ();
		
//		if (isServer) {
//			GameObject ob = (GameObject)Instantiate (PowerUp, SpawnPoints [Random.Range (0, SpawnPoints.Count)].transform.position, Quaternion.identity);
//			PowerInfo info = ob.GetComponent<PowerInfo> ();
//
//			int random = Random.Range (1, 5);
//			switch (random) {
//			case 1:
//				info.powerType = PowerType.Buff_Speed;
//				break;
//
//			case 2:
//				info.powerType = PowerType.Buff_Shield;
//				break;
//			case 3:
//				info.powerType = PowerType.Launcher_Freeze;
//				break;
//
//			case 4:
//				info.powerType = PowerType.Launcher_Skull;
//				break;
//			}
//
//			//info.powerType = PowerType.Buff_Speed;
//			info.selfDestroy = true;
//			info.selfDestroyTime = 25.0f;
//
//
//	
//			NetworkServer.Spawn (ob);
//		}
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}

public enum PowerType
{
	None,
	Buff_Speed,
	Buff_Shield,
	Buff_Freeze,
	Launcher_Freeze,
	Launcher_Skull
}
