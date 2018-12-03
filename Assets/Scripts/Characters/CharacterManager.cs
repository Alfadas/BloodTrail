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
	[SerializeField] CharacterActivationManager activationmanager;
	/*[SerializeField] GameObject characterpanel;
	[SerializeField] ActivationManager buttonactivationmanager;
	[SerializeField] ActivationManager panelactivationmanager;*/

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
				//panels.Add(newbutton.GetComponent<CharacterPanel>().gameObject); this is bs, wont yield a panel i suspect
				activationmanager.addCharacter(newbutton, newbutton.transform.GetChild(1).gameObject);
				y += 40;
				}

			newbutton = Instantiate(closebutton, new Vector3(x, y, 0), Quaternion.identity).gameObject;
			newbutton.transform.SetParent(parentbutton.gameObject.transform, false);
			characterbuttons.Add(newbutton);
			activationmanager.setCloseButton(newbutton);

			newbutton.GetComponent<ActivationManager>().setActivatables(characterbuttons);
			/*buttonactivationmanager.setActivatables(characterbuttons);
			panelactivationmanager.setActivatables(panels);*/
			}
		else
			{
			activationmanager.changeButtonStates();
			/*buttonactivationmanager.changeState();
			panelactivationmanager.deactivate();*/
			}
		}

	public void changeStateCharacterPanel(string charactername)
		{

		}

	public void addCharacter()
		{
		System.Random random = new System.Random(); // TODO: seed?
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

	public void killCharacter(Character character)
		{
		characters.Remove(character);
		Destroy(character.gameObject);
		}

	// Returns all current party characters
	public List<Character> getCharacters()
		{
		return characters;
		}
	}
