using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamReposition : MonoBehaviour {

	public GameObject gameManager;
	public GameObject center;
	// Use this for initialization

	void Update() {

		if (gameManager.GetComponent<MazeLoader> ().mazeColumns > gameManager.GetComponent<MazeLoader> ().mazeRows) {
			this.GetComponent<Camera> ().orthographicSize = (gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x * gameManager.GetComponent<MazeLoader> ().mazeColumns) / 2;
		} else if (gameManager.GetComponent<MazeLoader> ().mazeColumns < gameManager.GetComponent<MazeLoader> ().mazeRows) {
			this.GetComponent<Camera> ().orthographicSize = (gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x * gameManager.GetComponent<MazeLoader> ().mazeRows) / 2;
		} else {
			this.GetComponent<Camera> ().orthographicSize = (gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x * gameManager.GetComponent<MazeLoader> ().mazeColumns) / 2;
		}

		center = GameObject.Find ("Floor " + (int)(gameManager.GetComponent<MazeLoader> ().mazeRows / 2) + "," + (int)(gameManager.GetComponent<MazeLoader> ().mazeColumns / 2));
		if ((gameManager.GetComponent<MazeLoader> ().mazeColumns % 2 > 0) && (gameManager.GetComponent<MazeLoader> ().mazeRows % 2 > 0)) {
			this.transform.position = new Vector3 (center.transform.position.x, this.transform.position.y, center.transform.position.z);
		} else if ((gameManager.GetComponent<MazeLoader> ().mazeColumns % 2 > 0) && (gameManager.GetComponent<MazeLoader> ().mazeRows % 2 == 0)) {
			this.transform.position = new Vector3 (center.transform.position.x-(gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x/2), this.transform.position.y, center.transform.position.z);
		} else if ((gameManager.GetComponent<MazeLoader> ().mazeColumns % 2 == 0) && (gameManager.GetComponent<MazeLoader> ().mazeRows % 2 > 0)) {
			this.transform.position = new Vector3 (center.transform.position.x, this.transform.position.y, center.transform.position.z-(gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x/2));
		} else {
			this.transform.position = new Vector3 (center.transform.position.x-(gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x/2), this.transform.position.y, 
				center.transform.position.z-(gameManager.GetComponent<MazeLoader> ().wall.transform.lossyScale.x/2));
		}

	}
		
}
