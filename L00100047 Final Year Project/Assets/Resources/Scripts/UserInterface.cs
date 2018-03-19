using UnityEngine;
using System.Collections;
using System;

public class UserInterface : MonoBehaviour 
{
	
	private string gridSizeTextString = "Grid Size Cubed";
	private string enterTextString = "Enter";
	public int gridSizeX;
	public int gridSizeY;
	private string enterObstacles = "insert";
	
	public int algorithmSelected = 0;
	//public string[] selectionOfAlgorithms = new string[]{"A*","IDA*","Dijkstra"};


	public int heuristicSelected = 5;
	public string[] selectionOfHeuristics = new string[]{"Pythagoras","Diagonal","Euclidean","Manhatten"};

	public int xPlacement = 0;
	public int yPlacement = 0;
	public int guiElementWidth = 0;		//<------ For Testing
	public int guiElementHeight = 0;

	private string numberOfNPCs = "Number of NPCS";
	private string obstaclesText = "";
	private string obstaclesText2 = "";
	public int enterNumberOfNPCS;
	//public bool generateMapBool;
	//public bool mapSizeGUI;
	public int mapSize;
	public int obstaclesToPlace;
	public int obstaclesToPlace2;

	public int percentageEntered;


	public bool interfaceOn;
	public bool generateObstaclesBool;

	public Grid gridReference;

	[HideInInspector]
	public bool generateMapBool;
	public string[] selectionOfAlgorithms = new string[]{"A*","Dijkstra"};
	

	
	// Use this for initialization
	void Start () 
	{
		generateMapBool = false;
		interfaceOn = false;
		generateObstaclesBool = false;		//set of booleans that interact with the user input to generate a map
		mapSize = 0;
		//mapSizeGUI = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (gridReference.mapInPlace)
		{
			int.TryParse (enterTextString, out mapSize); //parse the map size
			//generateMapBool = false;
		}
		if(generateObstaclesBool && obstaclesText2 == "") // if the generate obstacles button is pressed and the other textfield is empty
		{
			int.TryParse(obstaclesText,out obstaclesToPlace); //parse the input
			gridReference.generateObstaclesByPercentage(obstaclesToPlace); //generate based on the % entered
			generateObstaclesBool = false;
		}
		if (generateObstaclesBool && obstaclesText == "")// do the same for the generateObstaclesByNumber method
		{
			int.TryParse(obstaclesText2,out obstaclesToPlace2);
			gridReference.generateObstaclesByNumber(obstaclesToPlace2);
			generateObstaclesBool = false;
		}

	}
	
	void OnGUI()
	{

			if (!gridReference.turnMapGUIOff)
			{
			GUI.Label(new Rect(619,11,198,56),"L00100047 Final Year Project");
				
				GUI.Label (new Rect (609,52,242,60), "Please enter the map size (squared) \n e.g. Entering 32 = 32x32");
				
			GUI.Label(new Rect(625,200,242,60),"Click the button TWICE");
					
				enterTextString = GUI.TextField (new Rect (677, 104, 53, 21), enterTextString, 5);
					
				if (GUI.Button (new Rect (650, 150, 97, 44), "Generate Map"))
				{
					generateMapBool = true;
				}
			}


		//Grid Size,X and Y labels
		
		if (interfaceOn) //if the GUI is on display everything in these brackest
		{

			GUI.Box(new Rect(860,160,234,70),"\nUse WASD to move camera position \n Up/Down Arrow keys for zoom");

			GUI.Box(new Rect(860,40,490,100),"\n Use the mouse to select start and end node \n You can choose obstacles by clicking on a node or entering a number in one of the \n two textfields and clicking the GUI button \n Spacebar to find a path");

			if (gridReference.timeStopped) //if the algorithm has found a path display these stats
			{
				GUI.Label(new Rect(700,6,127,86),"Path found in " + gridReference.sw.ElapsedMilliseconds + " Milliseconds");
				GUI.Label(new Rect(820,6,127,86),"Nodes visited : " + gridReference.nodesVisied);
				GUI.Label(new Rect(940,6,127,86),"Obstacle nodes : " + gridReference.allObstacles);
				GUI.Label(new Rect(500,6,127,86),"Total Nodes : " + gridReference.gridSizeX * gridReference.gridSizeY);
			}
			
			//Algorithm selection box
			GUI.Box (new Rect (0, 0, 153, 133), "Algorithms");
			algorithmSelected = GUI.SelectionGrid (new Rect (26, 24, 95, 104), algorithmSelected, selectionOfAlgorithms, 1); //Constructor(Rect position, selected button, number of buttons,how many buttons should appear on the horizontal)
			
			if (algorithmSelected == 0) 
			{

			}
			else if (algorithmSelected == 1) 
			{

			}
			else 
			{

			}
			//Heuristics selection box
			GUI.Box (new Rect (15, 150, 118, 136), "Heuristics");
			heuristicSelected = GUI.SelectionGrid (new Rect (23,175,96,97), heuristicSelected, selectionOfHeuristics, 1);
			
			if (heuristicSelected == 0) 
			{

			} 
			else if (heuristicSelected == 1) 
			{

			} 
			else if (heuristicSelected == 2) 
			{

			}
			else if (heuristicSelected == 3)
			{
				
			}
			else 
			{

			}

			obstaclesText = GUI.TextField (new Rect (164,311,52,29), obstaclesText, 10);
			obstaclesText2 = GUI.TextField(new Rect(249,310,52,29),obstaclesText2,10);
			GUI.Label(new Rect(164,252,253,238),"Left : Percentage OR Right : Numbers Press Generate Obstacles Button when done");


			

			
			//Button to generate obstacles, turns on a boolean that is used in the Grid class 
			if (GUI.Button (new Rect (14,301,135,44), "Generate Obstacles"))
			{
				generateObstaclesBool = true;


			}
		}

	}
}