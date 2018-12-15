using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
	{
	[SerializeField] CharacterButtonManager characterbuttonmanager;
	[SerializeField] SoundManager soundmanager;
	[SerializeField] GameObject defeat;
	[SerializeField] List<GameObject> characterprefabs;
	[SerializeField] int maxcharacters = 5;
	[SerializeField] int startcharacters = 3;

	private static System.Random random = new System.Random();
	private List<Character> characters;

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
			Character character = Instantiate(characterprefabs[random.Next(characterprefabs.Count)]).GetComponent<Character>();  // TODO: New characters spawn at (0, 0, 0), would there be a better place?

			characters.Add(character);
			character.setManager(this);
			}
		}

	public void addCharacter(Character character)
		{
		if(characters.Count < maxcharacters)
			{
			Character clone = Instantiate(character).GetComponent<Character>();  // TODO: New characters spawn at (0, 0, 0), would there be a better place?

			characters.Add(clone);
			clone.setManager(this);
			}
		}

	// Destroys the given character, plays the death sound and ends the game if all group members are down
	public void killCharacter(Character character)
		{
		if(characters.Contains(character))
			{
			characters.Remove(character);
			characterbuttonmanager.reloadCharacterButtons();  // TODO: do something similar when new characters join?, move this to CharacterButtonManager?
			}
		soundmanager.playSFX("death");
		Destroy(character.gameObject, 0.5f); // Delay, to give character some time to finish his business
		if(characters.Count <= 0)
			{
			soundmanager.playTitle("Consecrated Ground");
			defeat.SetActive(true);
			}
		}

	// Kill all characters of the group
	public void killAll()
		{
		foreach(Character character in characters)
			{
			killCharacter(character);
			}
		}

	// Returns total weight the current group members could possibly carry
	public int getGroupWeightLimit()
		{
		if(characters != null)
			{
			int weightlimit = 0;
			foreach(Character character in characters)
				{
				weightlimit += character.getStat(Character.STAT_STRENGTH * 2);
				}
			return weightlimit;
			}
		else								// Game just started, characters not yet initialized
			{
			return startcharacters * 20;	// TODO: Magic Number
			}

		}

	// Returns the speed of the currently slowest group member
	public int getGroupSpeed()
		{
		if(characters != null)
			{
			int minspeed = 0;
			foreach(Character character in characters)
				{
				if(character.getStat(Character.STAT_AGILITY) < minspeed || minspeed <= 0)
					{
					minspeed = character.getStat(Character.STAT_AGILITY);
					}
				}
			return minspeed;
			}
		else								 // Game just started, characters not yet initialized
			{
			return 20;
			}
		}

	// Returns, whether at least one groupmember is still alive
	public bool isAlive()
		{
		return characters.Count > 0;
		}

	// Returns the current number of survivors in the group
	public int getGroupCount()
		{
		if(characters != null)
			{
			return characters.Count;
			}
		else								 // Game just started, characters not yet initialized
			{
			return startcharacters;
			}
		}

	// Returns all current party characters
	public List<Character> getCharacters()
		{
		return characters;
		}
	}
