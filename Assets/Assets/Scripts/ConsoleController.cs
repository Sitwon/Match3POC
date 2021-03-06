using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour {
	[SerializeField]
	private InputField consoleInput;
	[SerializeField]
	private Text consoleOutput;
	[SerializeField]
	private int consoleLines;

	public GameController gameController;

	void Awake()
	{

	}

	void OnEnable()
	{	
		//Debug.Log("consoleenabled");
		//consoleInput.ActivateInputField();
		consoleInput.ActivateInputField();
		consoleInput.text="";
		//EventSystem.current.SetSelectedGameObject(consoleInput.gameObject, null);
		//consoleInput.OnPointerClick(new PointerEventData(EventSystem.current));

	}
	
	// Use this for initialization
	void Start () {
		InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();
		submitEvent.AddListener(ConsoleCommand);
		consoleInput.onEndEdit = submitEvent;
		consoleInput.ActivateInputField();
		MakeInputActive();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MakeInputActive()
	{
		consoleInput.ActivateInputField();
	}

	void ConsoleCommand(string commandString)
	{
		consoleInput.text="";
		ConsoleOutputAdd(">" + commandString);
		ConsoleCommandParse(commandString);
		consoleInput.ActivateInputField();
	}

	void ConsoleOutputAdd(string outputToAdd)
	{
		if (consoleOutput.text!="" && consoleOutput.text.Split('\n').Length<consoleLines+1) consoleOutput.text+="\n";
		consoleOutput.text+=outputToAdd;

		string[] consoleText=consoleOutput.text.Split('\n');

		if (consoleText.Length>consoleLines)
		{
			consoleOutput.text="";
			for(int i=1;i<consoleText.Length;i++)
			{
				consoleOutput.text+=consoleText[i];
				if (i<consoleLines) consoleOutput.text+="\n";
			}
		}

	}

	void ConsoleCommandParse(string commandString)
	{
		string[] commandTokens=commandString.ToUpper().Split(' ');

		switch (commandTokens[0]) 
		{
			case "HELP" :
			{
				HelpCommand(commandTokens);
				break;
			}
			case "?":
			{
				HelpCommand(commandTokens);
				break;
			}
			case "CHANGEPIECE":
			{
				ChangePieceCommand(commandTokens);
				break;
			}
			case "SHOWPOSSIBLEMATCHES":
			{
				ShowPossibleMatchesCommand();
				break;
			}
			case "SHOWCURRENTMATCHES":
			{
				ShowCurrentMatchesCommand();
				break;
			}
			case "RESETBOARD":
			{
				ResetBoardCommand(commandTokens);
				break;
			}
			case "ENDGAME":
			{
				EndGameCommand();
				break;
			}
			case "SETHIGHSCORE":
			{
				SetHighScoreCommandValidate(commandTokens);
				break;
			}
			case "SHOWCURRENT4MATCHES":
			{
				ShowCurrent4MatchesCommmand();
				break;
			}
			case "SHOWSORTEDMATCHES":
			{
				ShowSortedMatchesCommand();
				break;
			}
			case "SHOWCURRENTSTRAIGHT5MATCHES":
			{
				ShowCurrentStraight5MatchesCommand();
				break;
			}
		default:
			{
				ConsoleOutputAdd("- Invalid Command: "+commandTokens[0]);
				break;
			}

		}
	}

	void ShowCurrentStraight5MatchesCommand()
	{
		List<MatchThreeOrFour> current4Matches=gameController.SortMatches(gameController.GetThreeMatches()).fourMatches;
		List<MatchThreeOrFour> current5Matches=new List<MatchThreeOrFour>();
		
		foreach (MatchThreeOrFour fourMatch in current4Matches)
		{
			if (gameController.IsStraightFiveMatch(fourMatch))
			{
				current5Matches.Add(fourMatch);
			}
		}
		
		
		if (current5Matches.Count==0)
		{
			ConsoleOutputAdd("- no current straight 5 matches");
			return;
		}
		
		foreach(MatchThreeOrFour match in current5Matches)
		{
			ConsoleOutputAdd("- current straight 5 match: "+match.MatchDisplayString());
		}
	}

	void ShowSortedMatchesCommand()
	{
		MatchesContainer sortedMatchContainer=gameController.SortMatches(gameController.GetThreeMatches());

		if (sortedMatchContainer.threeMatches.Count==0)
		{
			ConsoleOutputAdd("- no current 3 matches");
		}
		
		foreach(MatchThreeOrFour match in sortedMatchContainer.threeMatches)
		{
			ConsoleOutputAdd("- current 3 match: "+match.MatchDisplayString());
		}		

		
		if (sortedMatchContainer.fourMatches.Count==0)
		{
			ConsoleOutputAdd("- no current 4 matches");
		}
		
		foreach(MatchThreeOrFour match in sortedMatchContainer.fourMatches)
		{
			ConsoleOutputAdd("- current 4 match: "+match.MatchDisplayString());
		}	

		if (sortedMatchContainer.fiveMatches.Count==0)
		{
			ConsoleOutputAdd("- no current 5 matches");
		}

		foreach(MatchFiveOrMore match in sortedMatchContainer.fiveMatches)
		{
			string coordString="";
			foreach (Coords coords in match.matchFivePieces)
			{
				coordString+=coords.CoordString()+" ";
			}
			ConsoleOutputAdd("- current 5 match coords:");
			ConsoleOutputAdd(coordString);
		}

	}

	void ShowCurrent4MatchesCommmand()
	{
		List<MatchThreeOrFour> currentMatches=gameController.GetThreeMatches();
		List<MatchThreeOrFour> current4Matches=new List<MatchThreeOrFour>();

		foreach (MatchThreeOrFour threeMatch in currentMatches)
		{
			if (gameController.IsFourMatch(threeMatch))
			{
				current4Matches.Add(threeMatch);
			}
		}


		if (current4Matches.Count==0)
		{
			ConsoleOutputAdd("- no current 4 matches");
			return;
		}
		
		foreach(MatchThreeOrFour match in current4Matches)
		{
			ConsoleOutputAdd("- current 4 match: "+match.MatchDisplayString());
		}

	}


	void SetHighScoreCommand(int highScoreToSet)
	{
		GameController.highScore=highScoreToSet;
		ConsoleOutputAdd("Setting high score to "+highScoreToSet.ToString());
	}

	void SetHighScoreCommandValidate(string[] commandTokens)
	{
		string usageInfo="- Usage: SETHIGHSCORE <Score>";

		//validate number of arguments
		if (commandTokens.Length!=2) {
			ConsoleOutputAdd("- Invalid number of arguments to sethighscore\n"+
			                 usageInfo);
			return;

		}

		//validate int
		int tempHighScore=0;
		if (!(int.TryParse(commandTokens[1],out tempHighScore)))
		{
			ConsoleOutputAdd("- Invalid highscore value: '"+commandTokens[1]+"'\n"+usageInfo);
			return;
		}

		SetHighScoreCommand(tempHighScore);
	}

	void EndGameCommand()
	{
		gameController.EndGame();
	}

	void ResetBoardCommand(string[] commandTokens)
	{
		gameController.ResetBoard();
	}

	void ShowCurrentMatchesCommand()
	{
		List<MatchThreeOrFour> currentMatches=gameController.GetThreeMatches();

		if (currentMatches.Count==0)
		{
			ConsoleOutputAdd("- no current matches");
			return;
		}

		foreach(MatchThreeOrFour match in currentMatches)
		{
			ConsoleOutputAdd("- current match: "+match.MatchDisplayString());
		}

	}

	void ShowPossibleMatchesCommand()
	{
		List<Swap> possibleMatchList=gameController.PossibleMatches();

		if (possibleMatchList.Count==0)
		{
			ConsoleOutputAdd("- no possible matches");
			return;
		}

		foreach (Swap matchSwap in possibleMatchList)
		{
			ConsoleOutputAdd("- possible match: "+matchSwap.DisplayString());
		}
	}

	void ChangePieceCommand(string[] splitCommand)
	{
		if (ChangePieceCommandValidate(splitCommand)) {
			ConsoleOutputAdd("- Changing piece at "+splitCommand[1]+","+splitCommand[2]+" to "+splitCommand[3]);
			gameController.ChangePieceAction(int.Parse(splitCommand[1]), int.Parse(splitCommand[2]), splitCommand[3]);
		}
	}

	bool ChangePieceCommandValidate(string[] splitCommand)
	{
		string usageInfo="- Usage: CHANGEPIECE [0-7] [0-7] [piecetype]\n"+
						 "- Valid PieceTypes: CONE, CROSS, HEART, CUBE, SPHERE, STAR, TORUS";
		//invalid number of parameters
		if (splitCommand.Length!=4) {
			ConsoleOutputAdd("- Invalid number of arguments to changepiece\n"+
			                 usageInfo);
			return false;
		}
		//invalid x coord
		int tempCoordX=0;
		if (!(int.TryParse(splitCommand[1],out tempCoordX)))
		{
			ConsoleOutputAdd("- Invalid X coordinate: '"+splitCommand[1]+"'\n"+usageInfo);
			return false;
		}
		//invalid y coord
		int tempCoordY=0;
		if (!(int.TryParse(splitCommand[2],out tempCoordY)))
		{
			ConsoleOutputAdd("- Invalid Y coordinate: '"+splitCommand[2]+"'\n"+usageInfo);
			return false;
		}
		//out of range x coord
		if (tempCoordX>7 || tempCoordX<0)
		{
			ConsoleOutputAdd("- X coordinate must be between 0 and 7\n" + usageInfo);
			return false;
		}
		//out of range y coord
		if (tempCoordY>7 || tempCoordY<0)
		{
			ConsoleOutputAdd("- Y coordinate must be between 0 and 7\n" + usageInfo);
			return false;
		}
		//invalid shape
		if (splitCommand[3]!="CONE" &&
		    splitCommand[3]!="CROSS" &&
		    splitCommand[3]!="HEART" &&
		    splitCommand[3]!="CUBE" &&
		    splitCommand[3]!="SPHERE" &&
		    splitCommand[3]!="STAR" &&
		    splitCommand[3]!="TORUS")
		{
			ConsoleOutputAdd("- Invalid shape: '"+splitCommand[3]+"'\n"+usageInfo);
			return false;
		}
		return true;
	}

	void HelpCommand(string[] splitCommand)
	{
		if (splitCommand.Length==1) {
			TextAsset helpFile=Resources.Load("HelpCommand") as TextAsset;
			ConsoleOutputAdd(helpFile.text);
		}
		else 
		{
			switch (splitCommand[1])
			{
			case "HELP":
				ConsoleOutputAdd("- Type \"Help\" or \"?\" to see a list of commands");
				break;
			case "CHANGEPIECE":
				ConsoleOutputAdd("- Usage: CHANGEPIECE [0-7] [0-7] [piecetype]\n"+
				                 "- Valid PieceTypes: CONE, CROSS, HEART, CUBE, SPHERE, STAR, TORUS");
				break;
			case "SHOWPOSSIBLEMATCHES":
				ConsoleOutputAdd("- shows possible matches display the coordinates of the pieces needed to be swapped");
				break;
			case "SHOWCURRENTMATCHES":
				ConsoleOutputAdd("- shows the current matches coordinates and direction");
				break;
			case "RESETBOARD":
				ConsoleOutputAdd("- destroys the board and re-rolls the pieces\n" +
								 "- new board will have no current matches and at least 1 possible match");
				break;
			case "ENDGAME":
				ConsoleOutputAdd("- ends the game");
				break;
			case "SETHIGHSCORE":
				ConsoleOutputAdd("- sets the high score");
				break;
			case "SHOWCURRENT4MATCHES":
				ConsoleOutputAdd("- shows the current length 4 matches coordiantes and direction");
				break;
			case "SHOWSORTEDMATCHES":
				ConsoleOutputAdd("- shows all the current matches sorted by three and four matches with start coords and direction");
				break;
			case "SHOWCURRENTSTRAIGHT5MATCHES":
				ConsoleOutputAdd("- shows all the current length 5 straight matches");
				break;
			default:
				ConsoleOutputAdd("- Could not find help for '"+splitCommand[1]+"'");
				break;
			}
		}
	}
}
