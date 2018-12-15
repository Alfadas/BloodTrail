using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Merge with CharacterButtonManager?
public class ItemButtonManager : MonoBehaviour
	{
	[SerializeField] InventoryManager inventorymanager;
	[SerializeField] CharacterManager charactermanager;
	[SerializeField] SoundManager soundmanager;
	[SerializeField] Button closebutton;
	[SerializeField] Button itembutton;

	private List<ItemButton> itembuttons;

	private void Start()
		{
		itembuttons = new List<ItemButton>();
		}

	public void openItemButtons(bool isFromButton)
		{
		if(closebutton.gameObject.activeSelf == true && isFromButton)
			{
			closeItemButtons();
			}
		else
			{
			List<Item> items = inventorymanager.getItems();
			float height = closebutton.GetComponent<RectTransform>().rect.height;
			for(int I = 0; I < items.Count; ++I)
				{
				if(I >= itembuttons.Count)
					{
					Button button = Instantiate(this.itembutton);
					button.transform.SetParent(gameObject.transform, false);
					button.transform.localPosition = new Vector3(0, (I + 1) * height, 0);

					ItemButton itembutton = button.GetComponent<ItemButton>();
					itembutton.fillButton(items[I], this, soundmanager);
					itembuttons.Add(itembutton);
					}
				else if(items[I] != itembuttons[I].getItem())
					{
					itembuttons[I].fillButton(items[I], this, soundmanager);
					itembuttons[I].closePanel();
					}
				else
					{
					itembuttons[I].gameObject.SetActive(true);
					itembuttons[I].gameObject.transform.localPosition = new Vector3(0, (I + 1) * height, 0);
					}
				itembuttons[I].reloadEquipSelect(charactermanager.getCharacters());
				}

			while(itembuttons.Count > items.Count)
				{
				ItemButton itembutton = itembuttons[itembuttons.Count - 1];
				itembuttons.Remove(itembutton);
				Object.Destroy(itembutton.gameObject);
				}

			closebutton.gameObject.SetActive(true);
			closebutton.transform.localPosition = new Vector3(0, (itembuttons.Count + 1) * height, 0);
			}
		}

	public void reloadItemButtons()
		{
		if(closebutton.gameObject.activeSelf == true)
			{
			openItemButtons(false);
			}
		}

	public void closePanels()
		{
		foreach(ItemButton itembutton in itembuttons)
			{
			itembutton.closePanel();
			}
		}

	public void closeItemButtons()
		{
		foreach(ItemButton itembutton in itembuttons)
			{
			itembutton.gameObject.SetActive(false);
			}
		closebutton.gameObject.SetActive(false);
		}

	public void reloadEquipSelects()
		{
		List<Character> characters = charactermanager.getCharacters();
		foreach(ItemButton button in itembuttons)
			{
			button.reloadEquipSelect(characters);
			}
		}

	public void destroyItem(Item item)
		{
		soundmanager.playSFX("sacrifice"); // TODO: Destroy-Sound here
		inventorymanager.destroyItem(item);
		reloadItemButtons();
		}

	public void equipItem(Item item, int characterindex)
		{
		inventorymanager.equipItem(item, characterindex);
		reloadItemButtons();
		}
	}