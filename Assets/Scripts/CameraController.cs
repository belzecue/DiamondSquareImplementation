using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public float lookSensitivity = 2f;
	public float smoothDamp = 0.05f;
	private float xRotation;
	private float yRotation;
	private float zRotation;
	public float zRotationSpeed = 30f;
	public float movementSpeed = 2f;
	private float currentXRotation;
	private float currentYRotation;

	private float currentXVelocity;
	private float currentYVelocity;


	// Use this for initialization
	void Start () {
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		// use mouse for rotation of the camera
		xRotation -= Input.GetAxis ("Mouse Y") * lookSensitivity;
		yRotation += Input.GetAxis ("Mouse X") * lookSensitivity;

		// Q and E to control yaw
		if (Input.GetKey (KeyCode.Q)) {
			zRotation += Time.deltaTime * zRotationSpeed;
		}
		if (Input.GetKey (KeyCode.E)) {
			zRotation -= Time.deltaTime * zRotationSpeed;
		}
		currentXRotation = Mathf.SmoothDamp (currentXRotation, xRotation, ref currentXVelocity, smoothDamp);
		currentYRotation = Mathf.SmoothDamp (currentYRotation, yRotation, ref currentYVelocity, smoothDamp);
		transform.rotation = Quaternion.Euler (currentXRotation, currentYRotation, zRotation);

		// Control position of the camera using wsad kets
		transform.Translate (Input.GetAxis ("Horizontal")*movementSpeed, 0, Input.GetAxis ("Vertical")*movementSpeed);

		// Avoid going under the terrain
		float terrainHeight = Terrain.activeTerrain.SampleHeight (transform.position);
		// check if height is within 5 units of height
		if (terrainHeight > transform.position.y-5) {
			transform.position = new Vector3 (transform.position.x, terrainHeight+5, transform.position.z);
		}

		// Avoid camera going out of the terrain boundary
		Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;
		if (transform.position.x >= terrainSize.x) {
			transform.position = new Vector3 (terrainSize.x, transform.position.y, transform.position.z);
		}
		if (transform.position.z >= terrainSize.z) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, terrainSize.z);
		}
		if (transform.position.x <= 0) {
			transform.position = new Vector3 (0, transform.position.y, transform.position.z);
		}
		if (transform.position.z <= 0) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
		}


	}

}
