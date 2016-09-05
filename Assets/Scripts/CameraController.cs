using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private float lookSensitivity = 2f;
	private float smoothDamp = 0.05f;
	private float xRotation;
	private float yRotation;
	private float zRotation;
	private float zRotationSpeed = 30f;
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
		xRotation -= Input.GetAxis ("Mouse Y") * lookSensitivity;
		yRotation += Input.GetAxis ("Mouse X") * lookSensitivity;

		if (Input.GetKey (KeyCode.Q)) {
			zRotation += Time.deltaTime * zRotationSpeed;
		}
		if (Input.GetKey (KeyCode.E)) {
			zRotation -= Time.deltaTime * zRotationSpeed;
		}
		currentXRotation = Mathf.SmoothDamp (currentXRotation, xRotation, ref currentXVelocity, smoothDamp);
		currentYRotation = Mathf.SmoothDamp (currentYRotation, yRotation, ref currentYVelocity, smoothDamp);
		transform.rotation = Quaternion.Euler (currentXRotation, currentYRotation, zRotation);
		transform.Translate (Input.GetAxis ("Horizontal")*1.5f, 0, Input.GetAxis ("Vertical")*1.5f);

		float terrainHeight = Terrain.activeTerrain.SampleHeight (transform.position);
		if (terrainHeight > transform.position.y-5) {
			transform.position = new Vector3 (transform.position.x, terrainHeight+5, transform.position.z);
		}

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
