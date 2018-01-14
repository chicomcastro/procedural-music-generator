using System.Collections;
using UnityEngine;

public class CameraFollower : MonoBehaviour {
	
	public AudioManager audioManager;
	public VisualConstructor vs;

	public float dragSpeed = 2;
	public bool followSensor = true;
	//public bool isPlaying = false; // Follow if it's playing

	public float panSpeed = 30f;
	public float panBoardThickness = 10f;
	public float zoomSpeed = 5f;
	public float scrollMaxSensibility = 1f;

	public float maxZoom;
	public float minZoom;

	public Transform boundSup;
	public Transform boundInf;

	private bool shouldFollowSensor = false;
	private Vector3 dragOrigin;
	private Vector3 initialPos;
	private Vector3 target;
	private float moveToSpeed = 2.0f;

	void Start ()
	{
		InitializeCamera();
	}

	void Update ()
	{
		if (ShouldMove())
			Move();

		//Zoom();
	}

	void FixedUpdate()
	{
		if (followSensor)
		{
			moveToSpeed = 60f / audioManager.bpm;

			if (shouldFollowSensor)
			{
				FollowSensor();
				return;
			}
		}

		if (moveToSpeed != 2.0f)
			moveToSpeed = 2.0f;

		#region Drag and move system (Unabled)
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
		#endregion
	}

	public void InitializeCamera()
	{
		initialPos = transform.position;
	}

	private void FollowSensor()
	{
		target = vs.GetSensorPosition();
		target.y = transform.position.y;
		StartCoroutine("MoveTo");
	}

	public void GoHome()
	{
		target = initialPos;
		StartCoroutine("MoveTo");
	}

	IEnumerator MoveTo()
	{
		while (true)
		{
			Vector3 aux = transform.position;

			Vector3 desiredPosition = Vector3.Lerp(aux, target, Time.fixedDeltaTime * moveToSpeed);
			
			if (Vector3.Magnitude(transform.position - target) < 0.1f || !ShouldMove())
			{
				StopCoroutine("MoveTo");

				yield return null;
			}

			transform.position = new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z);

			yield return null;
		}
	}

	private void Move()
	{/*
		if (
			Input.GetKey("w") ||
			(Input.mousePosition.y >= (Screen.height - panBoardThickness) &&
			 Input.mousePosition.y <= Screen.height)
			|| Input.GetKey(KeyCode.UpArrow)
		   )
		{
			GoUp();
		}
		if (Input.GetKey("s") ||
			(Input.mousePosition.y <= panBoardThickness &&
			 Input.mousePosition.y >= 0f) ||
			Input.GetKey(KeyCode.DownArrow)
		   )
		{
			GoDown();
		}*/
		if (
			Input.GetKey("d") ||
			(Input.mousePosition.x >= Screen.width - panBoardThickness && Input.mousePosition.x <= Screen.width) ||
			Input.GetKey(KeyCode.RightArrow)
		   )
		{
			GoRight();
		}
		if (
			Input.GetKey("a") ||
			(Input.mousePosition.x <= panBoardThickness && Input.mousePosition.x >= 0f) ||
			Input.GetKey(KeyCode.LeftArrow)
		)
		{
			GoLeft();
		}
	}

	private void Zoom ()
	{
		float scroll = LimitScrollSensibility(-Input.GetAxis("Mouse ScrollWheel"));

		float desiredZoom = Camera.main.orthographicSize * (1 + scroll * zoomSpeed);

		if (desiredZoom < maxZoom && desiredZoom > minZoom)
			Camera.main.orthographicSize = desiredZoom;
	}

	#region Movement methods
	private void GoUp()
	{
		transform.Translate(Vector3.up * panSpeed * Time.deltaTime, Space.Self);
	}

	private void GoDown()
	{
		transform.Translate(Vector3.down * panSpeed * Time.deltaTime, Space.Self);
	}

	private void GoLeft()
	{
		transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.Self);
	}

	private void GoRight()
	{
		transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.Self);
	}

	private float LimitScrollSensibility(float scroll)
	{
		return Mathf.Clamp(scroll, (-1f) * scrollMaxSensibility, scrollMaxSensibility);
	}

	private bool ShouldMove()
	{
		if (!(transform.position.x - 1f >= boundSup.position.x))
		{
			transform.position = new Vector3(boundSup.position.x + 1f, transform.position.y, transform.position.z);
			return false;
		}

		if (!(transform.position.x + 1f <= boundInf.position.x))
		{
			transform.position = new Vector3(boundInf.position.x - 1f, transform.position.y, transform.position.z);
			return false;
		}

		if (!(transform.position.y + 1f <= boundSup.position.y))
		{
			transform.position = new Vector3(transform.position.x, boundSup.position.y - 1f, transform.position.z);
			return false;
		}

		if (!(transform.position.y - 1f >= boundInf.position.y))
		{
			transform.position = new Vector3(transform.position.x, boundInf.position.y + 1f, transform.position.z);
			return false;
		}

		return true;
	}
	#endregion

	void OnTriggerExit2D (Collider2D other)
	{
		shouldFollowSensor = true;
	}
}