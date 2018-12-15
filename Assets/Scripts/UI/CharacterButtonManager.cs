using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonManager : MonoBehaviour
	{
	[SerializeField] CharacterManager characterManager;
	[SerializeField] ItemButtonManager itembuttonmanager;
	[SerializeField] SoundManager soundmanager;
	[SerializeField] Button closeButton;
	[SerializeField] Button characterButton;

	private List<CharacterButton> characterbuttons;

	private void Start()
		{
		characterbuttons = new List<CharacterButton>();
		}

	public void openCharacterButtons(bool isFromButton)
		{
		if(closeButton.gameObject.activeSelf == true && isFromButton)
			{
			closeCharacterButtons();
			}
		else
			{
			List<Character> characters = characterManager.getCharacters();
			float height = closeButton.GetComponent<RectTransform>().rect.height;
			for(int I = 0; I < characters.Count; ++I)
				{
				if(I >= characterbuttons.Count)
					{
					Button button = Instantiate(characterButton);
					button.transform.SetParent(gameObject.transform, false);
					button.transform.localPosition = new Vector3(0, (I + 1) * height, 0);

					CharacterButton characterbutton = button.GetComponent<CharacterButton>();
					characterbutton.fillButton(characters[I], this, soundmanager);
					characterbuttons.Add(characterbutton);
					}
				else if(characters[I] != characterbuttons[I].getCharacter())
					{
					characterbuttons[I].fillButton(characters[I], this, soundmanager);
					characterbuttons[I].closePanel();
					}
				else
					{
					characterbuttons[I].gameObject.SetActive(true);
					characterbuttons[I].gameObject.transform.localPosition = new Vector3(0, (I + 1) * height, 0);
					}
				}

			while(characterbuttons.Count > characters.Count)
				{
				CharacterButton characterbutton = characterbuttons[characterbuttons.Count - 1];
				characterbuttons.Remove(characterbutton);
				Object.Destroy(characterbutton.gameObject);
				}

			closeButton.gameObject.SetActive(true);
			closeButton.transform.localPosition = new Vector3(0, (characterbuttons.Count + 1) * height, 0);

			itembuttonmanager.reloadEquipSelects();
			}
		}

	public void reloadCharacterButtons()
		{
		if(closeButton.gameObject.activeSelf == true)
			{
			openCharacterButtons(false);
			}
		}

	public void closePanels()
		{
		foreach(CharacterButton characterButton in characterbuttons)
			{
			characterButton.closePanel();
			}
		}

	public void closeCharacterButtons()
		{
		foreach(CharacterButton characterButton in characterbuttons)
			{
			characterButton.gameObject.SetActive(false);
			}
		closeButton.gameObject.SetActive(false);
		}

	public void sacrifice(Character character)
		{
		soundmanager.playSFX("sacrifice");
		characterManager.killCharacter(character);
		}
	}