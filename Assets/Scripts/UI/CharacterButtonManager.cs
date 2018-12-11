using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonManager : MonoBehaviour
	{
	[SerializeField] Button characterButton;
	[SerializeField] Button closeButton;
	[SerializeField] CharacterManager characterManager;
	[SerializeField] int height = 40;

	private List<CharacterButton> characterButtons;

	private void Start()
		{
		characterButtons = new List<CharacterButton>();
		}

	public void OpenCharacterButtons(bool isFromButton)
		{
		List<Character> characters = characterManager.getCharacters();
		if(closeButton.gameObject.activeSelf == true && isFromButton)
			{
			CloseCharacterButtons();
			}
		else
			{
			List<CharacterButton> obsoleteButtons = new List<CharacterButton>();
			int i;

			for(i = 0; i < characters.Count; i++)
				{
				if(characterButtons.Count - 1 < i)
					{
					CreateNewButton(characters, i);
					}
				else if(characterButtons[i].GetCharacter() != characters[i])
					{
					obsoleteButtons.Add(characterButtons[i]);
					obsoleteButtons.Add(characterButtons[i + 1]);
					CreateNewButton(characters, i);
					}
				else
					{
					characterButtons[i].gameObject.SetActive(true);
					characterButtons[i].gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + ((i + 1) * height), 0);
					}
				}
			foreach(CharacterButton obsoleteButton in obsoleteButtons)
				{
				characterButtons.Remove(obsoleteButton);
				Object.Destroy(obsoleteButton.gameObject);
				}
			if(characterButtons.Count > characters.Count)
				{
				for(int ii = characters.Count; i < characterButtons.Count; i++)
					{
					if(!obsoleteButtons.Contains(characterButtons[i]))
						{
						obsoleteButtons.Add(characterButtons[i]);
						}
					}
				}
			foreach(CharacterButton obsoleteButton in obsoleteButtons)
				{
				characterButtons.Remove(obsoleteButton);
				Object.Destroy(obsoleteButton.gameObject);
				}
			closeButton.gameObject.SetActive(true);
			closeButton.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + ((characters.Count + 1) * height), 0);
			}
		}

	public void ReloadCharacterButtonsAfterKill(Character character)
		{
		if(closeButton.gameObject.activeSelf == true)
			{
			OpenCharacterButtons(false);
			}
		}

	public void ClosePanels()
		{
		foreach(CharacterButton characterButton in characterButtons)
			{
			characterButton.ClosePanel();
			}
		}

	public void CloseCharacterButtons()
		{
		foreach(CharacterButton characterButton in characterButtons)
			{
			characterButton.gameObject.SetActive(false);
			}
		closeButton.gameObject.SetActive(false);
		}

	public void Sacrifice(Character character)
		{
		characterManager.killCharacter(character);
		}

	private void CreateNewButton(List<Character> characters, int i)
		{
		Button newButton = Instantiate(characterButton, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + ((i + 1) * height), 0), Quaternion.identity);
		newButton.transform.SetParent(gameObject.transform, true);
		CharacterButton newCharacterButton = newButton.GetComponent<CharacterButton>();
		newCharacterButton.FillButton(characters[i], this);
		characterButtons.Add(newCharacterButton);
		}
	}