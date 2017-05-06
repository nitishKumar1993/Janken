using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RespawnScreen : MonoBehaviour
{

	public Text KillerName;
	public Image KillerImage;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void SetKillerDetails (PlayerInfo killer)
	{
		
		KillerName.text = killer.Name;
		KillerImage.sprite = CharacterManager.Instance.ReturnSprite_DeathScreen (killer.TeamID, killer.ModelID);
	}
}
