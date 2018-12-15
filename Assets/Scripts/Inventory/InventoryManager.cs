using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
	{
	[SerializeField] CharacterManager charactermanager;
	[SerializeField] int characteritemcapacity = 5;
	[SerializeField] Item[] startitems;

	private List<Item> items;
	private int currentweight;

	void Start()
		{
		items = new List<Item>();
		currentweight = 0;

		foreach(Item item in startitems)
			{
			addItem(item);
			}
		}

	public void addItem(Item item) // Maybe later GameObject with Item component? Who knows...
		{
		if(items.Count + 1 <= charactermanager.getGroupCount() * characteritemcapacity)
			{
			Item realitem = item; // Instantiate(item, Vector3.zero, Quaternion.identity); TODO: remove line
			if(currentweight + realitem.getWeight() <= charactermanager.getGroupWeightLimit())
				{
				items.Add(realitem);
				// TODO: Reload Item Buttons?
				}
			else
				{
				// TODO: too much weight, what to do?
				}
			}
		else
			{
			// TODO: too many items, what to do?
			}
		}

	public void destroyItem(Item item)
		{
		if(items.Remove(item))
			{
			currentweight -= item.getWeight();
			}
		}

	public void equipItem(Item item, int characterindex)
		{
		destroyItem(item);
		Item olditem = charactermanager.getCharacters()[characterindex].setWeapon((Weapon) item);
		if(olditem != null)
			{
			addItem(olditem);
			}
		}

	public List<Item> getItems()
		{
		return items;
		}
	}
