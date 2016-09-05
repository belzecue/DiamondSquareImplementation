using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

	public Color color;
	private Vector3 ratateAroundPoint;

	public Vector3 GetWorldPosition()
	{
		return this.transform.position;
	}

	void Start () {
		ratateAroundPoint = new Vector3 (transform.position.x, 0f, transform.position.z);
	}

	void Update () {
		transform.RotateAround (ratateAroundPoint, Vector3.right, 10f * Time.deltaTime);
	}
}

