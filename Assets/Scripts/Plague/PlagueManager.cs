using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueManager : MonoBehaviour
	{
	[SerializeField] GameObject plague;
	[SerializeField] float speed = 1;

	void FixedUpdate()
		{
		//spreadPlague(0.1f);
		}

	public void spreadPlague(float time)
		{
		plague.transform.localScale = new Vector3(plague.transform.localScale.x + speed, plague.transform.localScale.y, plague.transform.localScale.z + speed);
		}
	}
