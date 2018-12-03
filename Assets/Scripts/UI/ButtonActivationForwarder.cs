using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivationForwarder : MonoBehaviour
	{
	public void forwardActivation()
		{
		this.gameObject.transform.parent.gameObject.GetComponent<CharacterActivationManager>().changePanelState(this.gameObject);
		}
	}
