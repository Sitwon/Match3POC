﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameSelectController : MonoBehaviour {

	public Text scoreDisplay;
	public Text highScoreDisplay;

	// Use this for initialization

	void Awake() 
	{
		GameController.highScore=PlayerPrefs.GetInt("Highscore");
	}

	void Start () 
	{

		scoreDisplay.text="Last Score:"+GameController.score.ToString();
		highScoreDisplay.text="High Score:"+GameController.highScore.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
