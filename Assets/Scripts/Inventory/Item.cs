using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
	{
	[SerializeField] string itemname = "Noname";
	[SerializeField] string[] description = { "A mysterious item.", "Very mysterious." };
	[SerializeField] int weight = 10; // Given in hectograms (100g), to be feasable for ints

	void Start()
		{

		}

	public string getItemName()
		{
		return itemname;
		}

	public string[] getDescription()
		{
		return description;
		}

	public int getWeight()
		{
		return weight;
		}
	}
