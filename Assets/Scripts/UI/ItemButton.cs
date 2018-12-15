using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Merge with CharacterButton?
public class ItemButton : MonoBehaviour
	{
	[SerializeField] GameObject itempanel;
	[SerializeField] Dropdown equipselect;
	[SerializeField] Text buttontext;

	private Item item;
	private ItemButtonManager itembuttonmanager;
	private SoundManager soundmanager;

	public void fillButton(Item item, ItemButtonManager itembuttonmanager, SoundManager soundmanager)
		{
		this.item = item;
		this.itembuttonmanager = itembuttonmanager;
		this.soundmanager = soundmanager;
		buttontext.text = this.item.getItemName();
		}

	public void fillPanel()
		{
		soundmanager.playSFX("button");
		if(itempanel.activeSelf == true)
			{
			itempanel.SetActive(false);
			}
		else
			{
			itembuttonmanager.closePanels();
			itempanel.SetActive(true);

			Text[] textfields = itempanel.GetComponentsInChildren<Text>();

			textfields[0].text = item.getItemName();

			if(item is Weapon)
				{
				textfields[1].text = "Type: " + "Weapon";
				textfields[2].text = "Attack: " + ((Weapon) item).getAttack();
				}
			else if(item is Armour)
				{
				textfields[1].text = "Type: " + "Armour";
				textfields[2].text = "Defense: " + ((Armour) item).getDefense();
				}
			else
				{
				textfields[1].text = "Unknown Item";
				}
			
			textfields[3].text = item.getDescription()[0];
			textfields[4].text = item.getDescription()[1];
			textfields[5].text = "Weight: " + (item.getWeight() / 10.0f) + "kg";
			}
		}

	public void reloadEquipSelect(List<Character> characters)
		{
		equipselect.ClearOptions();

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		foreach(Character character in characters)
			{
			options.Add(new Dropdown.OptionData(character.getCharacterName()));
			}
		equipselect.AddOptions(options);
		}

	public void updateEquipSelect()
		{
		equipselect.RefreshShownValue();		// Maybe not necessary, just keep9ing it to stay safe
		}

	public void destroyItem()
		{
		itembuttonmanager.destroyItem(item);
		}

	public void equipItem()
		{
		itembuttonmanager.equipItem(item, equipselect.value);
		}

	public void closePanel()
		{
		itempanel.SetActive(false);
		}

	public Item getItem()
		{
		return item;
		}
	}