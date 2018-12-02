using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
	{
	[SerializeField] int maxcharacters = 6;

	private Character[] characters;

	void Start()
		{
		characters = new Character[maxcharacters];
		for(int I = 0; I < characters.Length; ++I)
			{
			characters[I] = null; // TODO: Instantiate new character here
			}
		}

	void Update()
		{

		}
	}
