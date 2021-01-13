using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using System.Linq;


public class TwitchChatBatch : MonoBehaviour
{

	private TcpClient twitchClient;
	private StreamReader reader;
	private StreamWriter writer;
	public String username;
	public String password;
	public String channelName;

	public GameObject player;
	public float movementScale;

	//Hash map that has a string key and a int value
	public Dictionary<string,int> msgHashMap = new Dictionary<string,int>();

	public float timer;
	public float batchTimer;

	void Start ()
	{
		//Let the game run even if alt-tabbed
		Application.runInBackground = true;
		//Initial connection to twitch client
		Connect ();
		//Initializes the timer to 0 and initializes the hash map commands to 0 occurences
		timer = 0;
		msgHashMap.Add ("left", 0);
		msgHashMap.Add ("right", 0);
		msgHashMap.Add ("up", 0);
		msgHashMap.Add ("down", 0);

	}

	void Update ()
	{
		//If twitchClient is not connected: reconnect
		if (!twitchClient.Connected) {
			Connect ();
		}

		//Read the messages in chat every frame of the game
		ReadChat ();

		//Batch based movement (every batchTimer number of seconds: move the player based on commands)
		timer += Time.deltaTime;
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
		}
		if (chatMessage.ToLower ().Contains ("right")) {
			msgHashMap ["right"]++;
		}
		if (chatMessage.ToLower ().Contains ("up")) {
			msgHashMap ["up"]++;
		}
		if (chatMessage.ToLower ().Contains ("down")) {
			msgHashMap ["down"]++;
		}
	}

	private void WeighInputs (){

		//Gets the highest value associated with any key
		int frequentKeyCount = 0;
		foreach (var command in msgHashMap.OrderByDescending (pair=>pair.Value).Take(1)) {
			frequentKeyCount = command.Value;
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
			if ((msgHashMap [key] == frequentKeyCount) && (frequentKeyCount != 0)) {
				PlayerMovement (key);
			}
		}

		//Resets the hash map batch
		msgHashMap ["left"] = 0;
		msgHashMap ["right"] = 0;
		msgHashMap ["up"] = 0;
		msgHashMap ["down"] = 0;

	}


	//String based movement method
	private void PlayerMovement (String command)
	{
		if (command == "left") {
			player.transform.position = new Vector2 (player.transform.position.x - (1 * movementScale), player.transform.position.y);
		}
		if (command == "right") {
			player.transform.position = new Vector2 (player.transform.position.x + (1 * movementScale), player.transform.position.y);
		}
		if (command == "up") {
			player.transform.position = new Vector2 (player.transform.position.x, player.transform.position.y + (1 * movementScale));
		}
		if (command == "down") {
			player.transform.position = new Vector2 (player.transform.position.x, player.transform.position.y - (1 * movementScale));
		}
	}

}
