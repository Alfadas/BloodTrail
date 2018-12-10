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
    [SerializeField] CharacterButtonManager characterButtonManager;
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
        if (characters.Contains(character))
        {
            characters.Remove(character);
            characterButtonManager.ReloadCharacterButtonsAfterKill(character);
        }
		Destroy(character.gameObject, 0.5f); // Delay, to give character some time to finish his business
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
