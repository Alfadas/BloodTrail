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

			textfields[2].text = character.getName();

			textfields[3].text = "Endurance: " + character.getStat(Character.STAT_ENDURANCE);
			textfields[4].text = "Strength: " + character.getStat(Character.STAT_STRENGTH);
			textfields[5].text = "Agility: " + character.getStat(Character.STAT_AGILITY);
			textfields[6].text = "Intelligence: " + character.getStat(Character.STAT_INTELLIGENCE);
			textfields[7].text = "Charisma: " + character.getStat(Character.STAT_CHARISMA);

			textfields[8].text = "Health: " + character.getHealth() + "/" + character.getMaxHealth() + "  Nutrition: " + character.getNutrition() + "/" + character.getMaxNutrition();
			textfields[9].text = "Weapon: " + "none";
			}
		}

	public Character getCharacter()
		{
		return character;
		}

	public void setCharacter(Character character)
		{
		this.character = character;
		}
	}
