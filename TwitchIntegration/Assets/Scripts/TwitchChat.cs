using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;


public class TwitchChat : MonoBehaviour
{

	private TcpClient twitchClient;
	private StreamReader reader;
	private StreamWriter writer;
	public String username;
	public String password;
	public String channelName;

	public GameObject player;
	public float movementScale;

	void Start ()
	{
		//Let the game run even if alt-tabbed
		Application.runInBackground = true;
		//Initial connection to twitch client
		Connect ();
	
	}

	void Update ()
	{
		//If twitchClient is not connected: reconnect
		if (!twitchClient.Connected) {
			Connect ();
		}
		//Read the messages in chat every frame of the game
		ReadChat ();

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

	//text based movement system for ReadChat()
	private void GameInputs (String chatMessage)
	{
		if (chatMessage.ToLower ().Contains ("left")) {
			player.transform.position = new Vector2 (player.transform.position.x - (1 * movementScale), player.transform.position.y);
		}
		if (chatMessage.ToLower ().Contains ("right")) {
			player.transform.position = new Vector2 (player.transform.position.x + (1 * movementScale), player.transform.position.y);
		}
		if (chatMessage.ToLower ().Contains ("up")) {
			player.transform.position = new Vector2 (player.transform.position.x, player.transform.position.y + (1 * movementScale));
		}
		if (chatMessage.ToLower ().Contains ("down")) {
			player.transform.position = new Vector2 (player.transform.position.x, player.transform.position.y - (1 * movementScale));
		}
	}
		
}
