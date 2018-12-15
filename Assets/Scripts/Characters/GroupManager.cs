using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Either merge this class with CharacterManager or name both of them more distinct
public class GroupManager : MonoBehaviour
	{
	[SerializeField] MapManager mapmanager;
	[SerializeField] GameObject targetmarker;
	[SerializeField] CharacterManager charactermanager;
	[SerializeField] RollEncounter encounterroller;
	[SerializeField] SoundManager soundmanager;
	[SerializeField] GameObject victory;
	[SerializeField] float animationtime;
	[SerializeField] float defaulttilespeed = 100.0f;
	[SerializeField] float defaultgroupspeed = 50.0f;

	private static System.Random random = new System.Random();
	private Map map;
	private float starttime;
	private bool moving;
	private bool encounter;

	void Start()
		{
		map = mapmanager.getMap();
		starttime = Time.time;
		moving = false;
		encounter = false;

		// Spawn
		gameObject.transform.position = new Vector3(random.Next(0, map.getWidth()), 0, map.getHeight() - 1);
		resetTarget();
		}

	void Update()
		{
        // Get target when left mousebutton is released
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && !encounter && charactermanager.isAlive())
			{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
				{
				if(hit.point.x >= -0.5f && hit.point.x < map.getWidth() - 0.5f && hit.point.z >= -0.5f && hit.point.z < map.getHeight() - 0.5f)
					{
					soundmanager.playSFX("move");
					moving = true;

					Vector2Int targetposition = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
					targetmarker.transform.position = new Vector3(targetposition.x, 0, targetposition.y);
					targetmarker.SetActive(true);
					}
				}
			}
		}

	void FixedUpdate()
		{
		if(targetmarker.activeSelf && gameObject.transform.position.Equals(targetmarker.transform.position))
			{
			if(moving)
				{
				soundmanager.playSFX("target");
				moving = false;
				}
			resetTarget();
			}
		else if(targetmarker.activeSelf && charactermanager.isAlive())
			{
			Vector3 oldposition = gameObject.transform.position;
			MapTile currenttile = map.getTile(new Vector2Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z)));
			float tiletime = currenttile.getSpeed();			// Set tiletime to tilespeed
			float grouptime = charactermanager.getGroupSpeed();	// Set grouptime to groupspeed
			if(tiletime > 0)									// Check if speed is > 0
				{
				tiletime = defaulttilespeed / tiletime;			// Get inverse to calculate time from speed
				}
			else
				{
				Debug.Log("Tile.getSpeed() returned a number <= 0, that should not happen.");
				}
			if(grouptime > 0)									// Check if speed is > 0
				{
				grouptime = defaultgroupspeed / grouptime;		// Get inverse to calculate time from speed
				}
			else
				{
				Debug.Log("CharacterManager.getGroupSpeed() returned a number <= 0, that should not happen.");
				}

			if(Time.time - starttime > animationtime * tiletime * grouptime)
				{
				starttime = Time.time;

				//Update party
				charactermanager.updateCharacters(Mathf.RoundToInt(tiletime * grouptime));

				// Move party
				Vector2 direction = new Vector2(targetmarker.transform.position.x - gameObject.transform.position.x, targetmarker.transform.position.z - gameObject.transform.position.z);
				direction = direction / direction.magnitude;
				if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Prefer Y if both are equal, because the plague comes from below
					{
					gameObject.transform.Translate(direction.x / Mathf.Abs(direction.x), 0, 0); // Move 1 unit in either positive or negative x direction
					}
				else
					{
					gameObject.transform.Translate(0, 0, direction.y / Mathf.Abs(direction.y)); // Move 1 unit in either positive or negative y direction
					}

				// Check tile on which the round ends
				currenttile = map.getTile(new Vector2Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z)));

				if(encounterroller.RollNewEncounter(currenttile))
					{
					encounter = true;
					resetTarget();
					}

				if(currenttile.getSubBiom() == SUBBIOM.Harbor)
					{
					soundmanager.playTitle("Amazing Grace");
					victory.SetActive(true);
					}
				else if(currenttile.getBiom() == BIOM.Water)
					{
					gameObject.transform.position = oldposition;
					resetTarget();
					}
				}
			}
		}

	public void endEncounter()
		{
		encounter = false;
		}

	private void resetTarget()
		{
		targetmarker.transform.position = gameObject.transform.position;
		targetmarker.SetActive(false);
		}
	}
