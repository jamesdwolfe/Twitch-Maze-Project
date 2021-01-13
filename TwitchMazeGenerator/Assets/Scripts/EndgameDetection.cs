using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameDetection : MonoBehaviour {

	public Canvas endMsg;
	public Canvas twitchUI;

	//The collider will check if the player is in range
	public void OnTriggerEnter(Collider col){
		//If the player is found: the game is over
		if (col.tag == "Player") {
			//disables the player's ability to move since the game is over
			col.gameObject.GetComponent<WallDetection> ().rayDist = 10000f;
			//Coroutine for a time delay
			StartCoroutine (Delay ());
		}
	}

	IEnumerator Delay()
	{
		//Activates the endgame message
		endMsg.gameObject.SetActive(true);
		twitchUI.gameObject.SetActive (false);
		//Waits for 5 seconds
		yield return new WaitForSeconds(5f);
		//Reloads the current scene (creates a new maze)
		int scene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);

	}
}
