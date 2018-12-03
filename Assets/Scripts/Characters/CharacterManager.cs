using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
	{
	[SerializeField] int maxcharacters = 10;
	[SerializeField] int maxcombatcharacters = 5;
	[SerializeField] int startcharacters = 5;
	[SerializeField] GameObject[] characterprefabs = new GameObject [3];

	private List<GameObject> characters;

	void Start()
		{
		System.Random random = new System.Random();
		characters = new List<GameObject>(maxcharacters);
		for(int I = 0; I < startcharacters; ++I)
			{
			characters.Add(Instantiate(characterprefabs[random.Next(characterprefabs.Length)])); // TODO: New characters spawn at (0, 0, 0), would there be a better place?
			}
		}

	public void updateCharacters(int time)
		{
		foreach(GameObject character in characters)
			{
			character.GetComponent<Character>().updateCharacter(time);
			}
		}
	}
