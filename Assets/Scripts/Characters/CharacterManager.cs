using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
	{
	[SerializeField] int maxcharacters = 10;
	[SerializeField] int maxcombatcharacters = 5;
	[SerializeField] int startcharacters = 5;
	[SerializeField] GameObject[] characterprefabs = new GameObject [3];
	[SerializeField] int maleprefabcount = 2;
	[SerializeField] Button parentbutton;
	[SerializeField] Button characterbutton;
	[SerializeField] Button closebutton;

	private List<GameObject> characters;
	private List<Button> characterbuttons;

	void Start()
		{
		characters = new List<GameObject>(maxcharacters);
		for(int I = 0; I < startcharacters; ++I)
			{
			addCharacter();
			}
		}

	public void updateCharacters(int time)
		{
		foreach(GameObject character in characters)
			{
			character.GetComponent<Character>().updateCharacter(time);
			}
		}

	public void generateCharacterSelection()
		{
		int x = 80;
		int y = 60;
		Button button;

		foreach(GameObject character in characters)
			{
			button = Instantiate(characterbutton, new Vector3(x, y, 0), Quaternion.identity);
			button.gameObject.transform.SetParent(parentbutton.gameObject.transform, false);
			button.GetComponentInChildren<Text>().text = character.GetComponent<Character>().getName();
			y += 40;
			}

		button = Instantiate(closebutton, new Vector3(x, y, 0), Quaternion.identity);
		button.gameObject.transform.SetParent(parentbutton.gameObject.transform, false);
		}

	public void addCharacter()
		{
		System.Random random = new System.Random(); // TODO: seed?
		int prefabindex = random.Next(characterprefabs.Length);
		GameObject character = Instantiate(characterprefabs[prefabindex]);  // TODO: New characters spawn at (0, 0, 0), would there be a better place?

		characters.Add(character);
		if(prefabindex < maleprefabcount)
			{
			character.GetComponent<Character>().rollName(true);
			}
		else
			{
			character.GetComponent<Character>().rollName(false);
			}
		character.GetComponent<Character>().setManager(this);
		}

	public void killCharacter(GameObject character)
		{
		characters.Remove(character);
		Destroy(character);
		}
	}
