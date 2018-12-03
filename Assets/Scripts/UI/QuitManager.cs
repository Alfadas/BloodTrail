using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitManager : MonoBehaviour
	{
	[SerializeField] ActivationManager activationmanager;

	void Update()
		{
		if(Input.GetKeyDown(KeyCode.Escape))
			{
			activationmanager.changeState();
			}
		}

	public void quit()
		{
		Application.Quit();
		}
	}
