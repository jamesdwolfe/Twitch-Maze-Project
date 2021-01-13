using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{

	public bool leftPossible = true;
	public bool rightPossible = true;
	public bool upPossible = true;
	public bool downPossible = true;

	public float rayDist;
	private GameObject objectHit;
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit hit;
	
		Debug.DrawRay (this.transform.position, Vector3.left * rayDist);
		if (Physics.Raycast (this.transform.position, Vector3.left, out hit, rayDist)) {
			objectHit = hit.collider.gameObject;
			if (objectHit.tag == "Wall") {
				downPossible = false;
			} else {
				downPossible = true;
			}
		} else {
			downPossible = true;
		}

		Debug.DrawRay (this.transform.position, Vector3.right * rayDist);
		if (Physics.Raycast (this.transform.position, Vector3.right, out hit, rayDist)) {
			objectHit = hit.collider.gameObject;
			if (objectHit.tag == "Wall") {
				upPossible = false;
			} else {
				upPossible = true;
			}
		} else {
			upPossible = true;
		}

		Debug.DrawRay (this.transform.position, Vector3.forward * rayDist);
		if (Physics.Raycast (this.transform.position, Vector3.forward, out hit, rayDist)) {
			objectHit = hit.collider.gameObject;
			if (objectHit.tag == "Wall") {
				rightPossible = false;
			} else {
				rightPossible = true;
			}
		} else {
			rightPossible = true;
		}

		Debug.DrawRay (this.transform.position, Vector3.back * rayDist);
		if (Physics.Raycast (this.transform.position, Vector3.back, out hit, rayDist)) {
			objectHit = hit.collider.gameObject;
			if (objectHit.tag == "Wall") {
				leftPossible = false;
			} else {
				leftPossible = true;
			}
		} else {
			leftPossible = true;
		}
			

	}

}
