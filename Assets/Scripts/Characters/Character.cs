﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
	{
	[SerializeField] int defaultstatboost = 2; // TODO: Secure, that this is always >= 0 and < number of stats
	[SerializeField] GameObject marker;
	[SerializeField] Material friendcolor;
	[SerializeField] Material foecolor;

	public const int STAT_ENDURANCE = 0;
	public const int STAT_STRENGTH = 1;
	public const int STAT_AGILITY = 2;
	public const int STAT_INTELLIGENCE = 3;
	public const int STAT_CHARISMA = 4;

	private CharacterManager manager;
	private string name;
	private int health;
	private int nutrition;
	private int[] stats;

	void Start()
		{
		reRollStats();

		health = getMaxHealth();
		nutrition = Mathf.RoundToInt(getMaxNutrition() * 0.5f);
		}

	// Makes the character lose nutrition and, if he is not starving, regenerate health based on passed time.
	public void updateCharacter(int time)
		{
		hunger(time * 2);
		if(nutrition > 0)
			{
			heal(time * 2);
			}
		}

	public string rollName(bool male)
		{
		const int VOCAL = 0;
		const int CONSONANT = 1;

		string[] capitals = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		string[] vocals = { "a", "e", "i", "o", "u" };
		string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "r", "s", "t", "v", "w", "x", "y", "z" };
		string[] maleending = { "o", "u", "b", "c", "d", "f", "g", "k", "l", "m", "n", "p", "r", "s", "t", "v", "w", "x", "z" };
		string[] femaleending = { "a", "e", "i", "y" };

		System.Random random = new System.Random(Random.Range(0, 100000));

		name = capitals[random.Next(capitals.Length)];

		int previous = CONSONANT;
		bool consonantchain = false;
		for(int I = 0; I < 4; ++I)
			{
			if(previous == VOCAL)
				{
				if(random.NextDouble() < 0.05)
					{
					name += vocals[random.Next(vocals.Length)];
					previous = VOCAL;
					}
				else
					{
					name += consonants[random.Next(consonants.Length)];
					previous = CONSONANT;
					}
				}
			else if(previous == CONSONANT)
				{
				if(random.NextDouble() < 0.05 && !consonantchain)
					{
					name += consonants[random.Next(consonants.Length)];
					previous = CONSONANT;
					consonantchain = true;
					}
				else
					{
					name += vocals[random.Next(vocals.Length)];
					previous = VOCAL;
					consonantchain = false;
					}
				}

			if(random.NextDouble() < 0.2)
				{
				break;
				}
			}

		if(male)
			{
			name += maleending[random.Next(maleending.Length)];
			}
		else
			{
			name += femaleending[random.Next(femaleending.Length)];
			}

		return name;
		}

	// Generates new random stats for this character.
	public void reRollStats()
		{
		System.Random random = new System.Random();
		int maxstatboost = 5 - defaultstatboost;
		int statboostmodifier = 0;
		for(int I = 0; I < maxstatboost; ++I)
			{
			if(random.NextDouble() < 0.5)
				{
				++statboostmodifier;
				}
			else
				{
				break;
				}
			}

		int statboostcount = defaultstatboost;
		if(random.NextDouble() < 0.5)
			{
			statboostcount += statboostmodifier;
			}
		else
			{
			// Map statboostcount from interval [0, maxstatboost] on [0, defaultstatcount] to avoid getting negative statcounts in a fair way
			statboostcount -= Mathf.RoundToInt(((float)statboostmodifier / (float)maxstatboost) * (float)defaultstatboost);
			}

		// Pull out some random stats to boost according to statboostcount
		int[] statlist = { STAT_ENDURANCE, STAT_ENDURANCE, STAT_ENDURANCE, STAT_ENDURANCE, STAT_ENDURANCE };
		for(int I = statlist.Length - 1; I >= statlist.Length - statboostcount; --I)
			{
			int luckyindex = random.Next(statlist.Length - I);
			int luckystat = statlist[luckyindex];

			statlist[luckyindex] = statlist[I];
			statlist[I] = luckystat;
			}
		int[] boostedstats = new int[statboostcount];
		for(int I = 0; I < boostedstats.Length; ++I)
			{
			// TODO: Optimization: sort entries during copy
			boostedstats[I] = statlist[statlist.Length - I - 1];
			}

		stats = new int[5];
		for(int I = STAT_ENDURANCE; I <= STAT_CHARISMA; ++I)
			{
			bool boosted = false;
			for(int U = 0; U < boostedstats.Length; ++U)
				{
				if(I == boostedstats[U])
					{
					boosted = true;
					break;
					}
				}

			if(boosted)
				{
				stats[I] = random.Next(61, 101);
				}
			else
				{
				stats[I] = random.Next(21, 61);
				}
			}
		}

	// Displays a marker below a character to mark him as active friend or targeted foe.
	public void markCharacter(bool foe)
		{
		if(foe)
			{
			marker.GetComponent<Renderer>().material = foecolor;
			}
		else
			{
			marker.GetComponent<Renderer>().material = friendcolor;
			}
		marker.SetActive(true);
		}

	// Deactivates the marker below the character.
	public void unmarkCharacter()
		{
		marker.SetActive(false);
		}

	// Returns, whether the character is dead after the attack (does not imply that he was killed by the attack)
	public bool hurt(int damage)
		{
		health -= damage;

		if(health <= 0)
			{
			if(manager != null)
				{
				manager.killCharacter(this);
				}

			return false;
			}

		return false;
		}

	// Consumes the given amount of calories from the nutrition value. Returns, whether the character is starving after consuming the given amount
	public bool hunger(int consumption)
		{
		nutrition -= consumption;
		heal(0); // Reduces health to maxhealth, if necessary
		hurt(0); // Kills the character, if his health is below 1

		if(nutrition <= 0)
			{
			return true;
			}

		return false;
		}

	// Restores the given amount of health for the characters current health value
	public int heal(int amount)
		{
		health += amount;

		int maxhealth = getMaxHealth();
		if(health > maxhealth)
			{
			health = maxhealth;
			}

		return health;
		}

	// Restores the given amount of calories for the characters nutrition value
	public int eat(int amount)
		{
		nutrition += amount;

		int maxnutrition = getMaxNutrition();
		if(nutrition > maxnutrition)
			{
			nutrition = maxnutrition;
			}

		return nutrition;
		}

	// Returns the current maximum health for this character based on his current endurance stat
	public int getMaxHealth()
		{
		return Mathf.RoundToInt(getStat(STAT_ENDURANCE) * 1.2f);
		}

	// Returns the current maximum nutrition for this character based on his current endurance stat
	public int getMaxNutrition()
		{
		return 20 + Mathf.RoundToInt(getStat(STAT_ENDURANCE) * 0.2f);
		}

	public string getName()
		{
		if(name != null)
			{
			return name;
			}
		else
			{
			return "Noname";
			}
		}

	// Returns the current health
	public int getHealth()
		{
		return health;
		}

	// Returns the current nutrition
	public int getNutrition()
		{
		return nutrition;
		}

	// Returns the requested stat multiplied by the current hunger-factor
	public int getStat(int stat)
		{
		float factor = 1;
		if(nutrition <= 0)
			{
			// SCIENCE!!
			factor = 1 - (0.002f * (Mathf.Abs(nutrition - 1) ^ 2));
			}
		return Mathf.Max(Mathf.FloorToInt(stats[stat] * factor), 0);
		}

	public void setManager(CharacterManager manager)
		{
		this.manager = manager;
		}
	}
