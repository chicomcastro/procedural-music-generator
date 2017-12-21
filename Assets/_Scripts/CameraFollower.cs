using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
	
	public AudioManager audioManager;
	public VisualConstructor vs;

	public float dragSpeed = 2;
	public bool followSensor = true;
	//public bool isPlaying = false; // Follow if it's playing

	private bool shouldMove = false;
	private Vector3 dragOrigin;
	private Vector3 initialPos;

	void Start ()
	{
		InitializeCamera();
	}

	void FixedUpdate()
	{
		if (followSensor)
		{
			if (shouldMove)
			{
				Move();
				return;
			}
		}

		if (Input.GetMouseButtonDown(2))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton(2)) return;

		Vector3 pos = Camera.main.ScreenToViewportPoint(-Input.mousePosition + dragOrigin);

		Vector3 move = new Vector3(pos.x * dragSpeed, 0f, 0f);

		if (transform.position.x + move.x * Time.fixedDeltaTime < initialPos.x)
		{
			return;
		}

		transform.Translate(move*Time.fixedDeltaTime, Space.World);		
	}

	public void InitializeCamera()
	{
		initialPos = transform.position;
	}

	void Move()
	{
		Vector3 aux = transform.position;
		aux.z = vs.GetSensorPosition().z;

		Vector3 desiredPosition = Vector3.Lerp(aux, vs.GetSensorPosition(), Time.fixedDeltaTime);

		transform.position = new Vector3(desiredPosition.x, transform.position.y, transform.position.z);
	}

	void OnTriggerExit2D (Collider2D other)
	{
		shouldMove = true;
	}
}