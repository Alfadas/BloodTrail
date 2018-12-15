using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
	{
	[SerializeField] int attack = 1;

	public int getAttack()
		{
		return attack;
		}
	}
