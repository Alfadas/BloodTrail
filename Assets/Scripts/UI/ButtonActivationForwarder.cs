using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Kill this class with fire, rework UI-System
public class ButtonActivationForwarder : MonoBehaviour
	{
	public void forwardActivation()
		{
		this.gameObject.transform.parent.gameObject.GetComponent<CharacterActivationManager>().changePanelState(this.gameObject);
		}

	public void sacrifice()
		{
		this.gameObject.GetComponent<CharacterPanel>().getCharacter().die();
		}
	}
