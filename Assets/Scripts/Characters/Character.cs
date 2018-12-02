using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
	{
	[SerializeField] int defaultstatboostnumber = 2; // TODO: Secure, that this is always >= 0 and < number of stats

	public const int STAT_ENDURANCE = 0;
	public const int STAT_STRENGTH = 1;
	public const int STAT_AGILITY = 2;
	public const int STAT_INTELLIGENCE = 3;
	public const int STAT_CHARISMA = 4;

	private int health;
	private int nutrition;
	private int[] stats;

	// TODO: Implement Character Highlighter, to avoid having Alfadas touch this code

	void Start()
		{
		reRollStats();

		health = getMaxHealth();
		nutrition = Mathf.RoundToInt(getMaxNutrition() * 0.5f);
		}

	void Update()
		{

		}

	public void reRollStats()
		{
		System.Random random = new System.Random();

		int maxstatboost = 5 - defaultstatboostnumber;
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

		int statboostnumber = defaultstatboostnumber;
		if(random.NextDouble() < 0.5)
			{
			statboostnumber += statboostmodifier;
			}
		else
			{
			// Map statnumber from interval [0, maxstatboost] on [0, defaultstatnumber] to avoid getting negative statnumbers in a fair way
			statboostnumber -= Mathf.RoundToInt(((float) statboostmodifier / (float) maxstatboost) * (float) defaultstatboostnumber);
			}

		// Pull out some random stats to boost according to statboostnumber
		int[] statlist = {STAT_ENDURANCE, STAT_ENDURANCE, STAT_ENDURANCE, STAT_ENDURANCE,STAT_ENDURANCE};
		for(int I = statlist.Length - 1; I >= statlist.Length - statboostnumber; --I)
			{
			int luckyindex = random.Next(statlist.Length - I);
			int luckystat = statlist[luckyindex];

			statlist[luckyindex] = statlist[I];
			statlist[I] = luckystat;
			}
		int[] boostedstats = new int[statboostnumber];
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

	// Returns, whether the character is dead after the attack (does not imply that he was killed by the attack)
	public bool hurt(int damage)
		{
		health -= damage;

		if(health <= 0)
			{
			// TODO: Kill character
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
	private int getMaxHealth()
		{
		return Mathf.RoundToInt(getStat(STAT_ENDURANCE) * 1.2f);
		}

	// Returns the current maximum nutrition for this character based on his current endurance stat
	private int getMaxNutrition()
		{
		return 20 + Mathf.RoundToInt(getStat(STAT_ENDURANCE) * 0.2f);
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
			factor = 1 - (0.002f * (Mathf.Abs(nutrition - 1)^2));
			}
		return Mathf.Max(Mathf.FloorToInt(stats[stat] * factor), 0);
		}
	}
