using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
	{
	[SerializeField] int maxcharacters = 10;
	[SerializeField] int maxcombatcharacters = 5;
	[SerializeField] int startcharacters = 5;
	[SerializeField] List<GameObject> characterprefabs;
	[SerializeField] int maleprefabcount = 2;
	[SerializeField] Button parentbutton;
	[SerializeField] Button characterbutton;
	[SerializeField] Button closebutton;
	[SerializeField] CharacterActivationManager characteractivator;
	[SerializeField] GameObject defeat;

	private static System.Random random = new System.Random();
	private List<Character> characters;
	private List<GameObject> characterbuttons;

	void Start()
		{
		characters = new List<Character>(maxcharacters);
		for(int I = 0; I < startcharacters; ++I)
			{
			addCharacter();
			}
		}

	public void updateCharacters(int time)
		{
		foreach(Character character in characters)
			{
			character.updateCharacter(time);
			}
		}

	public void changeStateCharacterSelection()
		{
		if(characterbuttons == null || characterbuttons.Count != (characters.Count + 1)) // + 1 for "Close"-Button
			{
			if(characterbuttons != null)
				{
				characteractivator.removeCharacters();

				foreach(GameObject oldbutton in characterbuttons)
					{
					Destroy(oldbutton);
					}
				}

			characterbuttons = new List<GameObject>(characters.Count + 1);
			int x = 80;
			int y = 60;
			GameObject newbutton;
			List<GameObject> panels = new List<GameObject>(characters.Count);
			foreach(Character character in characters)
				{
				newbutton = Instantiate(characterbutton, new Vector3(x, y, 0), Quaternion.identity).gameObject;
				newbutton.transform.SetParent(parentbutton.gameObject.transform, false);
				newbutton.GetComponentInChildren<Text>().text = character.getName();
				newbutton.GetComponent<CharacterPanel>().setCharacter(character);
				characterbuttons.Add(newbutton);
				characteractivator.addCharacter(newbutton, newbutton.transform.GetChild(1).gameObject);
				y += 40;
				}

			newbutton = Instantiate(closebutton, new Vector3(x, y, 0), Quaternion.identity).gameObject;
			newbutton.transform.SetParent(parentbutton.gameObject.transform, false);
			characterbuttons.Add(newbutton);
			characteractivator.setCloseButton(newbutton);

			newbutton.GetComponent<ActivationManager>().setActivatables(characterbuttons);
			}
		else
			{
			characteractivator.changeButtonStates();
			}
		}

	public void addCharacter()
		{
		if(characters.Count < maxcharacters)
			{
			int prefabindex = random.Next(characterprefabs.Count);
			Character character = Instantiate(characterprefabs[prefabindex]).GetComponent<Character>();  // TODO: New characters spawn at (0, 0, 0), would there be a better place?

			characters.Add(character);
			if(prefabindex < maleprefabcount)
				{
				character.rollName(true);
				}
			else
				{
				character.rollName(false);
				}

			character.setManager(this);
			}
		}

	public void killCharacter(Character character)
		{
		characters.Remove(character);
		Destroy(character.gameObject, 0.5f); // Delay, to give character some time to finish his business
		if(characteractivator.isActive())
			{
			characteractivator.changeButtonStates();
			}

		if(characters.Count <= 0)
			{
			defeat.SetActive(true);
			}
		}

	public void killAll()
		{
		foreach(Character character in characters)
			{
			killCharacter(character);
			}
		}

	// Returns all current party characters
	public List<Character> getCharacters()
		{
		return characters;
		}

	public int getGroupSpeed()
		{
		int minspeed = characters[0].getStat(Character.STAT_AGILITY);
		foreach(Character character in characters)
			{
			if(character.getStat(Character.STAT_AGILITY) < minspeed)
				{
				minspeed = character.getStat(Character.STAT_AGILITY);
				}
			}

		return minspeed;
		}
	}
