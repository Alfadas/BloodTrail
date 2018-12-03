using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Kill this class with fire, rework UI-System
public class CharacterActivationManager : MonoBehaviour
	{
	private Dictionary<GameObject, GameObject> characterbuttons;
	private GameObject closebutton;

	void Start()
		{
		characterbuttons = new Dictionary<GameObject, GameObject>();
		}

	public void changeButtonStates()
		{
		deactivatePanels();

		foreach(GameObject button in characterbuttons.Keys)
			{
			button.SetActive(!button.activeSelf);
			}

		if(closebutton != null)
			{
			closebutton.SetActive(!closebutton.activeSelf);
			}
		}

	public void changePanelState(GameObject button)
		{
		deactivatePanels();
		characterbuttons[button].SetActive(!characterbuttons[button].activeSelf);
		}

	public void deactivatePanels()
		{
		foreach(GameObject panel in characterbuttons.Values)
			{
			panel.SetActive(false);
			}
		}

	public void addCharacter(GameObject characterbutton, GameObject characterpanel)
		{
		characterbuttons.Add(characterbutton, characterpanel);
		}

	public void setCloseButton(GameObject closebutton)
		{
		this.closebutton = closebutton;
		}
	}
