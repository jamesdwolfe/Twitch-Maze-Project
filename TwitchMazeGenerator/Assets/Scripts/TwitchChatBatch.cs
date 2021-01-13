using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using UnityEngine.UI;


public class TwitchChatBatch : MonoBehaviour
{

	private TcpClient twitchClient;
	private StreamReader reader;
	private StreamWriter writer;
	public String username;
	public String password;
	public String channelName;

	public GameObject player;
	public GameObject gameManager;
	private float movementScale;

	//Hash map that has a string key and a int value
	public Dictionary<string,int> msgHashMap = new Dictionary<string,int>();

	public float timer;
	public float batchTimer;
	public Text[] twitchVotingMsg;

	void Start ()
	{
		//Let the game run even if alt-tabbed
		Application.runInBackground = true;
		//Initial connection to twitch client
		Connect ();
		//Get movementScale from the size of the maze
		movementScale = gameManager.GetComponent<MazeLoader>().size;
		//Initializes the timer to 0 and initializes the hash map commands to 0 occurences
		timer = 0;
		msgHashMap.Add ("left", 0);
		msgHashMap.Add ("right", 0);
		msgHashMap.Add ("up", 0);
		msgHashMap.Add ("down", 0);
		//Show channel name on the UI
		if (twitchClient.Connected) {
			twitchVotingMsg[5].text = ("twitch.tv/" +channelName).ToString();
		}

	}

	void Update ()
	{
		//If twitchClient is not connected: reconnect
		if (!twitchClient.Connected) {
			twitchVotingMsg[5].text = ("twitch.tv").ToString();
			Connect ();
		}

		//Read the messages in chat every frame of the game
		ReadChat ();

		//Batch based movement (every batchTimer number of seconds: move the player based on commands)
		timer += Time.deltaTime;
		//Change the color and the value of the timer on the UI
		twitchVotingMsg [0].text = (batchTimer-timer).ToString("0.0"); 
		StartCoroutine(LerpColorsOverTime (Color.green, Color.red, batchTimer));
		if (timer > batchTimer) {
			WeighInputs ();
			timer = 0;
		}

	}

	private void Connect ()
	{
		//EVERYTHING IN Connect() IS FINAL
		twitchClient = new TcpClient ("irc.chat.twitch.tv", 6667);
		reader = new StreamReader (twitchClient.GetStream ());
		writer = new StreamWriter (twitchClient.GetStream ());
		writer.WriteLine ("PASS " + password);
		writer.WriteLine ("NICK " + username);
		writer.WriteLine ("USER " + username + " 8 * :" + username);
		writer.WriteLine ("JOIN #" + channelName);
		writer.Flush ();
	}

	private void ReadChat ()
	{
		if (twitchClient.Available > 0) {
			var message = reader.ReadLine ();
			//Checks if the message was sent by a user
			if (message.Contains ("PRIVMSG")) {

				//Gets the name of the user that sent a message in chat
				var splitIndex = message.IndexOf ("!", 1);
				var chatName = message.Substring (0, splitIndex);
				chatName = chatName.Substring (1);

				//Gets the message the user sent in chat
				splitIndex = message.IndexOf (":", 1);
				var chatMessage = message.Substring (splitIndex + 1);

				//Message from twitch to console
				print (chatName.ToString () + ": " + chatMessage.ToString ());

				//Control the cube with the chat message
				GameInputs (chatMessage);
			}
			//print (message);
		}
	}

	//Input allocation for chat messages. Every known command that is called will increment the commands value in the hash map.
	private void GameInputs (String chatMessage)
	{
		if (chatMessage.ToLower ().Contains ("left")) {
			msgHashMap ["left"]++;
			twitchVotingMsg [1].text = ("Left: " +msgHashMap ["left"]).ToString(); 
		}
		if (chatMessage.ToLower ().Contains ("right")) {
			msgHashMap ["right"]++;
			twitchVotingMsg [2].text = ("Right: " +msgHashMap ["right"]).ToString(); 
		}
		if (chatMessage.ToLower ().Contains ("up")) {
			msgHashMap ["up"]++;
			twitchVotingMsg [3].text = ("Up: " +msgHashMap ["up"]).ToString(); 
		}
		if (chatMessage.ToLower ().Contains ("down")) {
			msgHashMap ["down"]++;
			twitchVotingMsg [4].text = ("Down: " +msgHashMap ["down"]).ToString(); 
		}
	}

	private void WeighInputs (){

		//Gets the highest value associated with any key.
		List<int> keyCounts = new List<int>();
		foreach (var command in msgHashMap.OrderByDescending (pair=>pair.Value).Take(2)) {
			keyCounts.Add (command.Value);
			//print (command.Value);
		}
		//Checks if the highest value associated with any key is a duplicate of another. If it is, make sure there will be no player movement this cycle.
		bool dupHighKeyCount = false;
		if (keyCounts [0] == keyCounts [1]) {
			dupHighKeyCount = true;
		}

		//nullify impossible player movements
		if (!player.GetComponent<WallDetection> ().leftPossible) {
			msgHashMap ["left"] = 0;
		} 
		if (!player.GetComponent<WallDetection> ().rightPossible) {
			msgHashMap ["right"] = 0;
		} 
		if (!player.GetComponent<WallDetection> ().upPossible) {
			msgHashMap ["up"] = 0;
		} 
		if (!player.GetComponent<WallDetection> ().downPossible) {
			msgHashMap ["down"] = 0;
		} 

		/*
		print ("Most frequent key count: " +frequentKeyCount);
		print ("Left count: " +msgHashMap ["left"]);
		print ("Right count: " +msgHashMap ["right"]);
		print ("Up count: " +msgHashMap ["up"]);
		print ("Down count: " +msgHashMap ["down"]);
        */

		//Moves the player if the key was called as many times as the most frequent key (provided its not 0)
		foreach (string key in msgHashMap.Keys) {
			if ((msgHashMap [key] == keyCounts[0]) && (keyCounts[0] != 0) && !dupHighKeyCount) {
				PlayerMovement (key);
				break;
			}
		}

		//Resets the hash map batch
		msgHashMap ["left"] = 0;
		msgHashMap ["right"] = 0;
		msgHashMap ["up"] = 0;
		msgHashMap ["down"] = 0;
		twitchVotingMsg [1].text = ("Left: " +msgHashMap ["left"]).ToString(); 
		twitchVotingMsg [2].text = ("Right: " +msgHashMap ["left"]).ToString(); 
		twitchVotingMsg [3].text = ("Up: " +msgHashMap ["left"]).ToString(); 
		twitchVotingMsg [4].text = ("Down: " +msgHashMap ["left"]).ToString(); 

	}


	//String based movement method
	private void PlayerMovement (String command)
	{
		if (command == "left") {
			player.transform.position = new Vector3 (player.transform.position.x,0, player.transform.position.z - (1 * movementScale));
		}
		if (command == "right") {
			player.transform.position = new Vector3 (player.transform.position.x,0, player.transform.position.z + (1 * movementScale));
		}
		if (command == "up") {
			player.transform.position = new Vector3 (player.transform.position.x + (1 * movementScale),0,player.transform.position.z);
		}
		if (command == "down") {
			player.transform.position = new Vector3 (player.transform.position.x - (1 * movementScale),0,player.transform.position.z);
		}
	}

	//CONTINUOUSLY change the color of some object
	private IEnumerator LerpColorsOverTime(Color startingColor, Color endingColor, float time)
	{
		for(float step = 0.0f; step < 1.0f ; step += Time.deltaTime * (1/time))
		{
			twitchVotingMsg [0].color = Color.Lerp(startingColor, endingColor, step);
			yield return null;
		}
		yield return StartCoroutine(LerpColorsOverTime(startingColor, endingColor, time));
	}

}
