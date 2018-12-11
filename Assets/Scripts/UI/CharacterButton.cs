using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
	{
	[SerializeField] GameObject characterPanel;
	[SerializeField] Text buttonText;

	private Character character;
	private CharacterButtonManager characterButtonManager;

	public void FillButton(Character buttonCharacter, CharacterButtonManager newCharacterButtonManager)
		{
		characterButtonManager = newCharacterButtonManager;
		character = buttonCharacter;
		buttonText.text = character.getCharacterName();
		}

	public void FillPanel()
		{
		if(characterPanel.activeSelf == true)
			{
			characterPanel.SetActive(false);
			}
		else
			{
			characterButtonManager.ClosePanels();
			characterPanel.SetActive(true);
			Text[] textfields = characterPanel.GetComponentsInChildren<Text>();

			textfields[1].text = character.getCharacterName();

			textfields[2].text = "Endurance: " + character.getStat(Character.STAT_ENDURANCE);
			textfields[3].text = "Strength: " + character.getStat(Character.STAT_STRENGTH);
			textfields[4].text = "Agility: " + character.getStat(Character.STAT_AGILITY);
			textfields[5].text = "Intelligence: " + character.getStat(Character.STAT_INTELLIGENCE);
			textfields[6].text = "Charisma: " + character.getStat(Character.STAT_CHARISMA);

			textfields[7].text = "Health: " + character.getHealth() + "/" + character.getMaxHealth() + "  Nutrition: " + character.getNutrition() + "/" + character.getMaxNutrition();
			textfields[8].text = "Weapon: " + "none";
			}
		}

	public void Sacrifice()
		{
		characterButtonManager.Sacrifice(character);
		}

	public void ClosePanel()
		{
		characterPanel.SetActive(false);
		}

	public Character GetCharacter()
		{
		return character;
		}
	}