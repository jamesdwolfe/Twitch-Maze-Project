using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndpointReposition: MonoBehaviour {

	public GameObject gameManager;
	public GameObject finalPoint;

	void Update(){
		finalPoint = GameObject.Find ("Floor " +(gameManager.GetComponent<MazeLoader> ().mazeRows-1) +"," +(gameManager.GetComponent<MazeLoader> ().mazeColumns-1));
		this.transform.position = finalPoint.transform.position;
		 
	}
}
