using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{

	public List<GameObject> Rocks;
	public List<GameObject> Papers;
	public List<GameObject> Scissors;

	public List<Sprite> sprite_Rocks;
	public List<Sprite> sprite_Papers;
	public List<Sprite> sprite_Scissors;
	[HideInInspector]
	public List<int> _Rocks;
	[HideInInspector]
	public List<int> _Papers;
	[HideInInspector]
	public List<int> _Scissors;


	public List<Sprite> death_Rocks;
	public List<Sprite> death_Papers;
	public List<Sprite> death_Scissors;

	public static CharacterManager Instance;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad (this.gameObject);
		Instance = this;
		_Rocks = new List<int> ();
		_Papers = new List<int> ();
		_Scissors = new List<int> ();
	}
	
	// Update is called once per frame
	void Update ()
	{


	}

	public int GenerateUniqueIndex (int range, List<int> indexes)
	{
		int i = 0;
		do {
			i = Random.Range (0, range);

		} while(indexes.Contains (i));

		indexes.Add (i);
		return i;
	}

	public int SelectUniqueModelID (int teamID)
	{
		if (teamID == 1) {
			return GenerateUniqueIndex (Rocks.Count, _Rocks);
		}
		if (teamID == 2) {
			return GenerateUniqueIndex (Papers.Count, _Papers);
		}
		if (teamID == 3) {
			return GenerateUniqueIndex (Scissors.Count, _Scissors);
		}
		return 0;
	}

	public GameObject ReturnCharacter (int teamID, int index)
	{
		if (teamID == 1) {
			return Rocks [index];
		}
		if (teamID == 2) {
			return Papers [index];
		}
		if (teamID == 3) {
			return Scissors [index];
		}
		return null;
	}

	public Sprite ReturnSprite_LoadingScreen (int teamID, int index)
	{
		if (teamID == 1) {
			return sprite_Rocks [index];
		}
		if (teamID == 2) {
			return sprite_Papers [index];
		}
		if (teamID == 3) {
			return sprite_Scissors [index];
		}
		return null;
	}

	public Sprite ReturnSprite_DeathScreen (int teamID, int index)
	{
		if (teamID == 1) {
			return death_Rocks [index];
		}
		if (teamID == 2) {
			return death_Papers [index];
		}
		if (teamID == 3) {
			return death_Scissors [index];
		}
		return null;
	}
}
