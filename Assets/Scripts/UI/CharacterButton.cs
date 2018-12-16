using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
	{
	[SerializeField] GameObject characterPanel;
	[SerializeField] Text buttonText;

	private Character character;
	private CharacterButtonManager characterbuttonmanager;
	private SoundManager soundmanager;

	public void fillButton(Character character, CharacterButtonManager characterbuttonmanager, SoundManager soundmanager)
		{
		this.character = character;
		this.characterbuttonmanager = characterbuttonmanager;
		this.soundmanager = soundmanager;
		buttonText.text = this.character.getCharacterName();
		}

	public void fillPanel()
		{
		soundmanager.playSFX("button");
		if(characterPanel.activeSelf == true)
			{
			characterPanel.SetActive(false);
			}
		else
			{
			characterbuttonmanager.closePanels();
			characterPanel.SetActive(true);

			Text[] textfields = characterPanel.GetComponentsInChildren<Text>();

			textfields[1].text = character.getCharacterName();

			textfields[2].text = "Endurance: " + character.getStat(Character.STAT_ENDURANCE);
			textfields[3].text = "Strength: " + character.getStat(Character.STAT_STRENGTH);
			textfields[4].text = "Agility: " + character.getStat(Character.STAT_AGILITY);
			textfields[5].text = "Intelligence: " + character.getStat(Character.STAT_INTELLIGENCE);
			textfields[6].text = "Charisma: " + character.getStat(Character.STAT_CHARISMA);

			textfields[7].text = "Health: " + character.getHealth() + "/" + character.getMaxHealth() + "  Nutrition: " + character.getNutrition() + "/" + character.getMaxNutrition();
			textfields[8].text = "Weapon: " + character.getWeaponName();
			}
		}

	public void sacrifice()
		{
		characterbuttonmanager.sacrifice(character);
		}

	public void closePanel()
		{
		characterPanel.SetActive(false);
		}

	public Character getCharacter()
		{
		return character;
		}

	public Text getText()
		{
		return buttonText;
		}
	}