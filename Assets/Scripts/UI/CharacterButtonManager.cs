using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonManager : MonoBehaviour {

    [SerializeField] Button characterButton;
    [SerializeField] Button closeButton;
    [SerializeField] CharacterManager characterManager;
    [SerializeField] int height = 40;

    List<CharacterButton> characterButtons;

    private void Start()
    {
        characterButtons = new List<CharacterButton>();
    }

    public void OpenCharacterButtons()
    {
        List<Character> characters = characterManager.getCharacters();
        int i;

        for (i = 0; i<characters.Count; i++)
        {
            if (characterButtons.Count-1 < i)
            {
                CreateNewButton(characters, i);
            }
            else if (characterButtons[i].GetCharacter() != characters[i])
            {
                characterButtons.Remove(characterButtons[i]);
                Object.Destroy(characterButtons[i].gameObject);
                CreateNewButton(characters, i);
            }
            else
            {
                characterButtons[i].gameObject.SetActive(true);
            }
        }
        closeButton.gameObject.SetActive(true);
        closeButton.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + ((i+1) * height), 0);
    }

    public void RelodeCharacterButtonsAfterKill(Character character)
    {
        foreach (CharacterButton characterButton in characterButtons)
        {
            if (characterButton.GetCharacter() == character)
            {
                characterButtons.Remove(characterButton);
                Object.Destroy(characterButton.gameObject); ;
            }
        }
        if (closeButton.gameObject.activeSelf == true)
        {
            OpenCharacterButtons();
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
        foreach (CharacterButton characterButton in characterButtons)
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