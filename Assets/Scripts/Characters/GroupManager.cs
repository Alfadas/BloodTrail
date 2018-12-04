using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
	{
	[SerializeField] MapManager mapmanager;
	[SerializeField] GameObject targetmarker;
	[SerializeField] CharacterManager charactermanager;
	[SerializeField] RollEncounter encounterroller;
	[SerializeField] float animationtime;
	[SerializeField] GameObject victory;

	private static System.Random random = new System.Random();
	private Map map;
	private float starttime;

	void Start()
		{
		map = mapmanager.getMap();
		starttime = Time.time;

		// Spawn
		gameObject.transform.position = new Vector3(random.Next(0, map.getWidth()), 0, map.getHeight() - 1);
		targetmarker.transform.position = gameObject.transform.position;
		targetmarker.SetActive(false);
		}

	void Update()
		{
		// Get target when right mousebutton is released
		if(Input.GetMouseButton(0))
			{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
				{
				if(hit.point.x >= -0.5f && hit.point.x < map.getWidth() - 0.5f && hit.point.z >= -0.5f && hit.point.z < map.getHeight() - 0.5f)
					{
					Vector2Int targetposition = new Vector2Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
					MapTile target;

					target = map.getTile(targetposition);
					targetmarker.transform.position = new Vector3(targetposition.x, 0, targetposition.y);
					targetmarker.SetActive(true);
					}
				}
			}
		}

	void FixedUpdate()
		{
		if(gameObject.transform.position.Equals(targetmarker.transform.position))
			{
			targetmarker.SetActive(false);
			}
		else
			{
			Vector3 oldposition = gameObject.transform.position;
			MapTile currenttile = map.getTile(new Vector2Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z)));
			float tiletime = 100.0f / currenttile.getSpeed();
			float grouptime = 50.0f / charactermanager.getGroupSpeed();

			if(Time.time - starttime > animationtime * tiletime * grouptime)
				{
				starttime = Time.time;

				//Update party
				charactermanager.updateCharacters(Mathf.RoundToInt(tiletime * grouptime));

				// Move party
				Vector2 direction = new Vector2(targetmarker.transform.position.x - gameObject.transform.position.x, targetmarker.transform.position.z - gameObject.transform.position.z);
				direction = direction / direction.magnitude;

				if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
					{
					gameObject.transform.Translate(direction.x / Mathf.Abs(direction.x), 0, 0); // Move 1 unit in either positive or negative x direction
					}
				else
					{
					gameObject.transform.Translate(0, 0, direction.y / Mathf.Abs(direction.y)); // Move 1 unit in either positive or negative y direction
					}
				// gameObject.transform.position = new Vector3(Mathf.RoundToInt(gameObject.transform.position.x + direction.x), 0, Mathf.RoundToInt(gameObject.transform.position.z + direction.y)); // Diagonal Movement

				if(encounterroller.RollNewEncounter(currenttile))
					{
					targetmarker.transform.position = gameObject.transform.position;
					targetmarker.SetActive(false);
					}

				currenttile = map.getTile(new Vector2Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.z)));

				if(currenttile.getSubBiom() == SUBBIOM.Harbor)
					{
					victory.SetActive(true);
					}
				else if(currenttile.getBiom() == BIOM.Water)
					{
					gameObject.transform.position = oldposition;
					}
				}
			}
		}
	}
