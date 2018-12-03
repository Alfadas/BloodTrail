using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
	{
	private Character character;

	void Start()
		{

		}

	public void updateCharacterInfo()
		{
		if(character != null)
			{
			Text[] textfields = this.gameObject.GetComponentsInChildren<Text>();

			// int[] stats = 

			textfields[1].text = character.getName();
			textfields[2].text = "specific stats";
			}
		}

	public void setCharacter(Character character)
		{
		this.character = character;
		}
	}
