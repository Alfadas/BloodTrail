using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : Item
	{
	[SerializeField] int defense = 1;

	public int getDefense()
		{
		return defense;
		}
	}
