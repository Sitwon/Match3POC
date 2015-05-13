using UnityEngine;
using System.Collections;
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
		consoleInput.ActivateInputField();
	}

	// Use this for initialization
	void Start () {
		InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();
		submitEvent.AddListener(ConsoleCommand);
		consoleInput.onEndEdit = submitEvent;
		consoleInput.ActivateInputField();
	}
	
	// Update is called once per frame
	void Update () {
	
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
			default:
			{
				ConsoleOutputAdd("- Invalid Command: "+commandTokens[0]);
				break;
			}

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
			default:
				ConsoleOutputAdd("- Could not find help for '"+splitCommand[1]+"'");
				break;
			}
		}
	}
}