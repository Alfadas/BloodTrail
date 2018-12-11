using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
	{
	[SerializeField] float speed = 1;
	[SerializeField] float zoomspeed = 10;

	public bool active = true; // TODO: Make private with Getter/Setter, maybe rename ("mapactive"?)

	private float currentspeed = 5;
	private float currentzoomspeed = 5;
	private Vector3 direction;

	void Update()
		{
		if(active)
			{
			// Boost on Shift
			if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
				currentspeed = speed * 5;
				currentzoomspeed = zoomspeed * 5;
				}
			else
				{
				currentspeed = speed;
				currentzoomspeed = zoomspeed;
				}

			// Hide and lock cursor on MMB down
			if(Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
				{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				}

			// Show and unlock cursor on MMB release
			if(Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
				{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				}

			// Drag map with mouse
			if(Input.GetMouseButton(1) || Input.GetMouseButton(2))
				{
				transform.position -= transform.rotation * new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
				}

			direction = new Vector3();
			// Get input directions
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
				{
				direction += Vector3.up;
				}
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				{
				direction += Vector3.left;
				}
			if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				{
				direction += Vector3.down;
				}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				{
				direction += Vector3.right;
				}

			// Translate and zoom camera
			transform.position += (transform.rotation * Vector3.forward) * Input.GetAxis("Mouse ScrollWheel") * currentzoomspeed; // Zoom with ScrollWheel

			currentspeed *= transform.position.y * 0.02f;
			transform.position += (transform.rotation * direction) * currentspeed;

			// Lock camera to table
			float x = transform.position.x;
			float y = transform.position.y;
			float z = transform.position.z;

			y = Mathf.Clamp(y, 2, 50);
			x = Mathf.Clamp(x, (-(100 - y) * 0.4f) + 25, ((100 - y) * 0.4f) + 25);
			z = Mathf.Clamp(z, (-(50 - y) * 0.8f) + 25, ((50 - y) * 0.8f) + 25);

			transform.position = new Vector3(x, y, z);
			}
		}
	}
