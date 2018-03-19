using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Grid : MonoBehaviour 
{
	//public Vector2[] gridSize;
	Vector2 gridSize;
	public int gridSizeX;
	public int gridSizeY;
	GameObject[,] grid;
	public GameObject nodeHolderPrefab;
	public Light directionalLight;
	public Camera mainCamera;
	NodeColourChanger cameraScript;
	UserInterface userInterfaceScript;
	Node startNode;
	Node endNode;
	Node startNode2;
	Node endNode2;
	Node startNode3;
	Node endNode3;
	//float timeInMilliSeconds;
	//bool timerOn;
	//List<GameObject> unwalkableList;
	//GameObject testingGameObjectPos;
	public Stopwatch sw;
	bool stopTimer;
	bool keepGoing;
	bool obstaclesBool;
	public bool mapInPlace;
	public bool turnMapGUIOff;
	public bool timeStopped;
	public int percentageInt;
	public int nodesVisied;
	public int allObstacles;


	// Use this for initialization
	void Start () 
	{
		//gridSize.x = 0;
		//gridSize.y = 0;
		//unwalkableList = null;
		cameraScript = mainCamera.GetComponent<NodeColourChanger>();  //reference to the NodeColourChangerScript attached to the camera.
		userInterfaceScript = mainCamera.GetComponent<UserInterface>(); //reference to the UserInterface script
		startNode = null;
		endNode = null;
		startNode2 = null;
		endNode2 = null;
		startNode3 = null;
		endNode3 = null;
		//createGrid();
		sw = new Stopwatch();
		//stopTimer = false;
		//timerOn = true;
		//getAllNodesInGrid();
		//testingGameObjectPos = null;
		//colourAllNodes();--THIS WORKS
		keepGoing = false;
		obstaclesBool = false;
		mapInPlace = false;
		turnMapGUIOff = false;
		timeStopped = false;
		nodesVisied = 0;
		allObstacles = 0;
	}
	
	// Update is called once per frame

	void Update () 
	{

		if(userInterfaceScript.generateMapBool)// if there is input in the initial screen
		{
			mapInPlace = true;//turn this boolean on

			gridSizeX = userInterfaceScript.mapSize; //get the parsed int from the user interface script and create grid
			gridSizeY = userInterfaceScript.mapSize;
			//UnityEngine.Debug.Log("Map size :" + gridSizeX);
			createGrid();
			//UnityEngine.Debug.Log(getAllNodesInGrid().Count);
			userInterfaceScript.generateMapBool = false;
			Invoke("MapGUIToggle",2.5f);// remove initial text
			Invoke("turnInterfaceOn",3.0f);// turn on the GUI
		}
		
		//Debug.Log("Time to find path is " + timeInMilliSeconds + " MS");
		
		if(cameraScript.numberOfClicks >= 2)
		{
				//			Getting the start and end nodes on the grid 
				
				startNode = getNodeFromWorldPosition(cameraScript.startNodeTransform);
				
				
				endNode = getNodeFromWorldPosition(cameraScript.endNodeTransform);
				
				
				
			//THIW CODE IS IRRELEVENT, WAS USED FOR MULTIPLE NPCS-failed implementation

			if (cameraScript.endPoints == 2)
			{
				startNode2 = getNodeFromWorldPosition(cameraScript.startNodeTransform2);
				endNode2 = getNodeFromWorldPosition(cameraScript.endNodeTransform2);
			}
			if (cameraScript.endPoints == 3)
			{
				startNode3 = getNodeFromWorldPosition(cameraScript.startNodeTransform3);
				endNode3 = getNodeFromWorldPosition(cameraScript.endNodeTransform3);
			}	

				
				
				
				
			//if(cameraScript.startSearchBool == true)
			obstaclesBool = true;
			
			
			
			
			
			
			
			
		}
		if (Input.GetButtonDown("Jump"))// if the space bar is pressed
		{
			
			directionalLight.enabled = false; //turn the light off while a path is being found
			
			
			//Debug.Log("Search started");


			if (userInterfaceScript.algorithmSelected == 0)
			{
				thePath = aStar(startNode,endNode);						//peform a traversal with the selcted algorithm
			}	
			else if (userInterfaceScript.algorithmSelected == 1)
			{
				thePath = dijkstra(startNode,endNode);
			}



			colourAllNodes(); //visualize the path
		}
	
	}

	void createGrid()
	{
		//Creates a number of gameObjects based on a given map size
		//The for loop then attaches an instance of the Node class to each gameObject
		//A node's grid position is based on the x and y co-ordinates of the gameObject it's attached to
		//The F, G and H costs of each node is set to 0, by default it is walkable and has no parent

		grid = new GameObject[gridSizeX,gridSizeY];
		gridSize.x = gridSizeX;
		gridSize.y = gridSizeY;
		
		for(int x = 0; x < gridSizeX; x++)
		{
			for(int y = 0; y < gridSizeY; y++)
			{

				grid[x,y] = Instantiate(nodeHolderPrefab, new Vector3(x, y, 0), Quaternion.identity)as GameObject;
				grid[x,y].GetComponent<Node>().worldPosition = new Vector3(x,y,0);
				grid[x,y].GetComponent<Node>().nodeGridPosition = new Vector2(x,y);
				grid[x,y].GetComponent<Node>().walkable = true;
				grid[x,y].GetComponent<Node>().gCost = 0;
				grid[x,y].GetComponent<Node>().hCost = 0;
				grid[x,y].GetComponent<Node>().fCost = 0;
				grid[x,y].GetComponent<Node>().parent = null;


			}

		}

	}

	public int maxSize
	{
		get{
			return gridSizeX * gridSizeY;
		}
	}

	bool checkNodeCoordinates(int nodeX,int nodeY, int otherX, int otherY)
	{
		//method to check if a node position is occupied by another. 

		return((nodeX == otherX) && nodeY == otherY);
	}

	//variable to store a path if it's found. It is then used in the colourAllNodes method
	public List<Node> thePath;

	public Node getNodeFromWorldPosition(Vector3 nodeWorldPosition)
	{

		//Returns a node based on the position of a gameObject in the world

		//Find all gameObjects in the scene
		GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag("nodePrefabTag");

		GameObject nodeObject = null;


		//If a gameObject's world position is equal to that of the specified positom
		//store it in a temporary variable and break out of the loop
		foreach(GameObject go in objectsInScene)
		{
			if (go.transform.position == nodeWorldPosition) 
			{
				nodeObject = go;
				break;
			}
		}

		//return the node attached to the temporary variable
		return nodeObject.GetComponent<Node>();
	}

	public GameObject getGameObjectPositionFromNode(Vector2 thisNodeGridPosition)
	{
		//Same logic as the previous method except it takes in a node's position and returns a gameObject if the x and y co-ordinates match
		GameObject[] objectsInScene2 = GameObject.FindGameObjectsWithTag("nodePrefabTag");

		GameObject nodeObject2 = null;

		foreach (GameObject go in objectsInScene2) 
		{
			if (thisNodeGridPosition.x == go.transform.position.x && thisNodeGridPosition.y == go.transform.position.y)
			{
				nodeObject2 = go;
				break;
			}
		}
		return nodeObject2;
	}
	
	void colourAllNodes()
	{
		//This method is used to visualize a path once it's been found

		//Create a temporary list of nodes and assign it to a method that returns all nodes in the map
		List<Node> tempListOfNodes = null;
		tempListOfNodes = getAllNodesInGrid();
		GameObject nodeToGameObject = null;
		for (int i = 0; i < tempListOfNodes.Count; i++)
		{
			//loop through the list and get a gameObject's position based on the position of it's attached node

			nodeToGameObject = getGameObjectPositionFromNode(tempListOfNodes[i].nodeGridPosition);

			if (thePath != null)// if a path is found
			{
				if (thePath.Contains(tempListOfNodes[i]))
				{
					//if the path contains a node from the list

					//visualize the gameobject the node's attached to with a cyan colour
					nodeToGameObject.renderer.material.color = Color.cyan;

					//if the path contains the end node, change it's colour to red
					if (thePath.Contains(tempListOfNodes[i]) && tempListOfNodes[i] == endNode)
					{
						nodeToGameObject.renderer.material.color = Color.red;

					}
				}

			}
			//turn the directional light back on, 1 second after the path is visualized
			Invoke("turnOnLight",1.0f);
		}
	}

	public List<Node> getNeighboursOfNode(Node centerNode)
	{
		//method returns the neighbours of a given node THE CODE FOR THIS METHOD WAS TAKEN FROM A PREVIOUS MODULE(ADVANCED GAME PROGRAMMING I)

		//a list of nodes is created and the x and y co-ordinates of the given node is stored

		List<Node> neighbours = new List<Node>();
		int nodeXCoordinate = (int)centerNode.nodeGridPosition.x;
		int nodeYCoordinate = (int)centerNode.nodeGridPosition.y;

		//a nested for loop that gets 3 neighbouring nodes in the x and y directions,
		//totalling 8 nodes(excluding the given node thats in the centre


		for (int x = nodeXCoordinate - 1; x <= nodeXCoordinate + 1; x++)
		{
			for (int y = nodeYCoordinate - 1; y <= nodeYCoordinate + 1; y++)
			{
				//if the node is on the grid and is not equal to the center node
				if ((x >= 0) && (y >= 0) && (x < gridSizeX) && (y < gridSizeY ) && !checkNodeCoordinates(nodeXCoordinate,nodeYCoordinate,x,y))
				{
					//add that node to the list
					Node neighbour = getNodeFromWorldPosition(new Vector3(x,y,0));
					neighbours.Add(neighbour);
				}
			}
		}
		//return the list
		return neighbours;
	}

	//method not used
	void retracePath(Node node1,Node node2)
	{
		List<Node> path = new List<Node>();
		Node currentNode = node2;
		//Debug.Log(node2.nodeGridPosition);

		while (currentNode != node1)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		thePath = path;
		colourAllNodes();
	}

	int getDiagonalHorizontalHeuristic(Node node1,Node node2)
	{
		int distanceX = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.x) - Mathf.RoundToInt(node2.nodeGridPosition.x));
		int distanceY = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.y) - Mathf.RoundToInt(node2.nodeGridPosition.y));
		
		if (distanceX > distanceY)
			return 14 * distanceY + 10 * (distanceX - distanceY);
		return 14 * distanceX + 10 * (distanceY - distanceX);
	}

	int getManhattanHeuristic(Node node1, Node node2)
	{
		int distanceX = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.x) - Mathf.RoundToInt(node2.nodeGridPosition.x));
		int distanceY = Mathf.Abs (Mathf.RoundToInt (node1.nodeGridPosition.y) - Mathf.RoundToInt (node2.nodeGridPosition.y));

		return 100 * (distanceX + distanceY);
	}

	int getDiagonalHeuristic(Node node1, Node node2)
	{
		int distanceX = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.x) - Mathf.RoundToInt(node2.nodeGridPosition.x));
		int distanceY = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.y) - Mathf.RoundToInt(node2.nodeGridPosition.y));

		return 10 * (distanceX + distanceY) + (14 - 2 * 10) * Mathf.Min (distanceX, distanceY);
	}

	int getEuclideanHeuristic(Node node1, Node node2)
	{
		int distanceX = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.x) - Mathf.RoundToInt(node2.nodeGridPosition.x));
		int distanceY = Mathf.Abs(Mathf.RoundToInt(node1.nodeGridPosition.y) - Mathf.RoundToInt(node2.nodeGridPosition.y));

		float euclidean = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);;
		int euc = Mathf.RoundToInt (euclidean);

		return euc;
	}
	List<Node> getAllNodesInGrid()
	{
		//method that returns a list of all nodes in the map by getting the attached node of each gameObject

		List<Node> listOfNodes = new List<Node> ();
		
		foreach (GameObject go in grid)
		{
			listOfNodes.Add(go.GetComponent<Node>());
		}
		return listOfNodes;
	}

	List<Vector2> getAllGridPositions()
	{
		//method that returns a list containing all node positions on the map

		List<Vector2> listOfPositions = new List<Vector2> ();

		foreach(GameObject go in grid)
		{
			listOfPositions.Add(go.GetComponent<Node>().nodeGridPosition);
		}

		return listOfPositions;
	}

	List<Node> aStar(Node startNodeIn,Node endNodeIn)
	{
		//Method to find a path using A*
		//start the timer and create 3 lists, a open and closed list and a temporary list to return a path if the path is found

		sw.Start();
		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();
		List<Node> tempList = new List<Node>();

		//add the start node to the open list

		openList.Add (startNodeIn);

		
		while (openList.Count > 0) //while the open list is not empty
		{
			int lowestFValue = 0; // temporary variable to get the node with the lowest f value

			for (int i = 0; i < openList.Count; i++)//loop through the open list
			{
				if (openList[i].fCost < openList[lowestFValue].fCost)// if the open list has a node with an f value lower than the temp variable
				{
					lowestFValue = i;//change the value of the temporary variable
				}
			}
			Node currentNode = openList[lowestFValue];// assign the current node to the one with the lowest f value

			if (currentNode == endNodeIn)// if the current node is the goal node
			{
				sw.Stop();//stop the timer
				print("Time in MS : " + sw.ElapsedMilliseconds);
				timeStopped = true;//turn on a boolean that will display the time and visited nodes on screen

				Node currNode = currentNode;//temp variable to get the children of the end node

				List<Node> path = new List<Node>();//create a temporary list of nodes ti visualize the path

				while (currNode.parent)//while the start node hasnt been reached
				{
					path.Add(currNode);//add the temporary node
					currNode = currNode.parent;//and get its parent
				}
				//path.Reverse();

				//store the path in the temporary list
				tempList = path;
			}

			//if the end node is not found remove the current node from the open list and add to closed list

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			//store the neighbours of the current node using the getNeighbours method
			List<Node> neighboursOfCurrentNode = getNeighboursOfNode(currentNode);

			//loop through the neighbours
			for (int i = 0; i < neighboursOfCurrentNode.Count; i++)
			{
				// store the neighbour in a temporary variable
				Node neighbour = neighboursOfCurrentNode[i];

				//if the neighbour is in the closed list or unwalkable, ignore it
				if (closedList.Contains(neighbour) || neighbour.walkable == false)
				{
					continue;
				}

				//increment the G value and store it in a temporary variable
				int gValue = currentNode.gCost++;

				//boolean to check for the lowest G value
				bool bestGValue = false;

				// if the neighbour is not on the open list
				if (!openList.Contains(neighbour))
				{
					//turn the boolean on
					bestGValue = true;

					//get the heuristic cost of the neighbour based on the heuristic selected by the user
					if (userInterfaceScript.heuristicSelected == 0)
					{
						neighbour.hCost = getDiagonalHorizontalHeuristic(neighbour,endNodeIn);
						//UnityEngine.Debug.Log("Pythagaros heuristic being used");
					}
					else if (userInterfaceScript.heuristicSelected == 1)
					{
						neighbour.hCost = getDiagonalHeuristic(neighbour,endNodeIn);
						//UnityEngine.Debug.Log("Diagonel heuristic being used");
					}
					else if(userInterfaceScript.heuristicSelected == 2)
					{
						neighbour.hCost = getEuclideanHeuristic(neighbour,endNodeIn);
						//UnityEngine.Debug.Log("Euclidean heuristic being used");
					}
					else if (userInterfaceScript.heuristicSelected == 3)
					{
						neighbour.hCost = getManhattanHeuristic(neighbour,endNode);
						//UnityEngine.Debug.Log("Manhattan  heuristic being used");
					}
					//neighbour.hCost = getManhattanHeuristic(neighbour,endNodeIn);
					//neighbour.hCost = getDiagonalHeuristic(neighbour,endNodeIn);
					//neighbour.hCost = getManhattanHeuristic(neighbour,endNodeIn);
					//neighbour.hCost = getEuclideanHeuristic(neighbour,endNodeIn);

					//add the neighbour to the openlist
					openList.Add(neighbour);
				}
				else if(gValue < neighbour.gCost)
				{
					//if the temporary g value is lower than the neighbour's leave the boolean on
					bestGValue = true;
				}
				if(bestGValue) // if the boolean is on
				{
					//make the current node the parent of the neighbour
					//assign the g value of the current node to the g value of the neighbour
					//the f cost of the neighbour is the sum of its g value and h value
					neighbour.parent = currentNode;
					neighbour.gCost = gValue;
					neighbour.fCost = neighbour.gCost + neighbour.hCost;
				}
			}
		}
		//assign the visited nodes
		nodesVisied = closedList.Count;
		//return the temporary list containing the path. It will be used by the colourAllNodes method
		return tempList;
	}




	public void generateObstaclesByPercentage(int percent)
	{
		//method to generate obstacles based on a % of the map size
		//get the number of nodes in the grid

		int allNodes = gridSizeX * gridSizeY;
		int counter = 0;
		bool keepGoing = false;

		//work out %
		float calculatePercent = allNodes / percent;

		//work out the number of nodes that will be unwalkable
		int nodesToMakeUnwalkable =(Mathf.RoundToInt(allNodes * percent / 100));
		//UnityEngine.Debug.Log("Nodes " + nodesToMakeUnwalkable);

		//create a list that will contain random node positions
		List<Vector2> randomPositions = new List<Vector2>();
		GameObject go = null;
		
		int randomX;//int to store the random x co-ordinate generated
		int randomY;//int to store the random y co-ordinate generated
		for(int i = 0; i < nodesToMakeUnwalkable; i++)
		{
			//loop through the number of nodes that will become obstacles

			//generate an x and y int

			randomX = Random.Range(0,gridSizeX);
			randomY = Random.Range(0,gridSizeY);
			Vector2 randomNodePosition = new Vector2(randomX,randomY);

			//create a random position based on the generated x and y
			
			//if (randomNodePosition == startNode.nodeGridPosition || randomNodePosition == endNode.nodeGridPosition)
			if(endNode != null)
			{
				//if the random position is equal to the position of the start or end node, ignore it

				if (randomNodePosition == startNode.nodeGridPosition || randomNodePosition == endNode.nodeGridPosition)
				{
					continue;
				}
				//continue;
			}
			if (endNode2 != null)
			{
				if (randomNodePosition == startNode2.nodeGridPosition || randomNodePosition == endNode.nodeGridPosition)
				{
					continue;
				}
			}
			if (endNode3 != null)
			{
				if (randomNodePosition == startNode3.nodeGridPosition || randomNodePosition == endNode3.nodeGridPosition)
				{
					continue;
				}
			}

			//add the position to a list
			randomPositions.Add(randomNodePosition);
		}
		//UnityEngine.Debug.Log(randomPositions.Count);

		//make every node in the list unwalkable

		foreach (Vector2 pos in randomPositions)
		{
			go = getGameObjectPositionFromNode(pos);
			go.renderer.material.color = Color.gray;
			go.GetComponent<Node>().walkable = false;
			allObstacles++;
		}
	}
	
	public void generateObstaclesByNumber(int numberOfNodes)
	{
		//same method as above but takes in a number of nodes instead of a %

		List<Vector2> randomPositions = new List<Vector2>();
		GameObject go = null;

		int randomX;
		int randomY;
		for(int i = 0; i < numberOfNodes; i++)
		{
			randomX = Random.Range(0,gridSizeX);
			randomY = Random.Range(0,gridSizeY);
			Vector2 randomNodePosition = new Vector2(randomX,randomY);

			//if (randomNodePosition == startNode.nodeGridPosition || randomNodePosition == endNode.nodeGridPosition)
			if(endNode != null)
			{
				if (randomNodePosition == startNode.nodeGridPosition || randomNodePosition == endNode.nodeGridPosition)
				{
					continue;
				}
				//continue;
			}
			if (endNode2 != null)
			{
				if (randomNodePosition == startNode2.nodeGridPosition || randomNodePosition == endNode.nodeGridPosition)
				{
					continue;
				}
			}
			if (endNode3 != null)
			{
				if (randomNodePosition == startNode3.nodeGridPosition || randomNodePosition == endNode3.nodeGridPosition)
				{
					continue;
				}
			}

			randomPositions.Add(randomNodePosition);
		}
		//UnityEngine.Debug.Log(randomPositions.Count);

		foreach (Vector2 pos in randomPositions)
		{
			go = getGameObjectPositionFromNode(pos);
			go.renderer.material.color = Color.gray;
			go.GetComponent<Node>().walkable = false;
			allObstacles++;
		}
	}

	List<Node> dijkstra(Node startNodeIn,Node endNodeIn) //same logic as AStar except it doesnt use a heuristic to help calculate a path length
	{
		sw.Start ();

		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();
		List<Node> tempList = new List<Node>();
		
		openList.Add (startNodeIn);
		
		while (openList.Count > 0)
		{
			int lowestFValue = 0;
			
			for (int i = 0; i < openList.Count; i++)
			{
				if (openList[i].fCost < openList[lowestFValue].fCost)
				{
					lowestFValue = i;
				}
			}
			Node currentNode = openList[lowestFValue];
			
			if (currentNode == endNodeIn)
			{
				sw.Stop();
				print("Time in MS : " + sw.ElapsedMilliseconds);
				timeStopped = true;

				Node currNode = currentNode;
				
				List<Node> path = new List<Node>();
				
				while (currNode.parent)
				{
					path.Add(currNode);
					currNode = currNode.parent;
				}
				//path.Reverse();
				tempList = path;
			}
			
			openList.Remove(currentNode);
			closedList.Add(currentNode);
			
			List<Node> neighboursOfCurrentNode = getNeighboursOfNode(currentNode);
			
			for (int i = 0; i < neighboursOfCurrentNode.Count; i++)
			{
				Node neighbour = neighboursOfCurrentNode[i];
				
				if (closedList.Contains(neighbour) || neighbour.walkable == false)
				{
					continue;
				}
				
				int gValue = currentNode.gCost++;
				bool bestGValue = false;
				
				if (!openList.Contains(neighbour))
				{
					bestGValue = true;
					//neighbour.hCost = 0;
					openList.Add(neighbour);
				}
				else if(gValue < neighbour.gCost)
				{
					bestGValue = true;
				}
				if(bestGValue)
				{
					neighbour.parent = currentNode;
					neighbour.gCost = gValue;
					neighbour.fCost = neighbour.gCost;
				}
			}
		}
		nodesVisied = closedList.Count;

		if (tempList == null)
			return null;
		
		return tempList;

	}

	List<Node> idaSearch(Node startNodeIn,Node endNodeIn, int depth)
	{
		//Method to find a path using IDA*
		//start the timer and create 3 lists, a open and closed list and a temporary list to return a path if the path is found
		
		sw.Start();
		List<Node> nodesProcessed = new List<Node>();
		List<Node> tempList = new List<Node>();
		
		//add the start node to the open list
		
		nodesProcessed.Add(startNodeIn);
		
		
		while (depth > 0) //while the open list is not empty
		{
			int lowestFValue = 0; // temporary variable to get the node with the lowest f value
			
			for (int i = 0; i < nodesProcessed.Count; i++)//loop through the open list
			{
				if (nodesProcessed[i].fCost < nodesProcessed[lowestFValue].fCost)// if the open list has a node with an f value lower than the temp variable
				{
					lowestFValue = i;//change the value of the temporary variable
				}
			}
			Node currentNode = nodesProcessed[lowestFValue];// assign the current node to the one with the lowest f value
			
			if (currentNode == endNodeIn)// if the current node is the goal node
			{
				sw.Stop();//stop the timer
				print("Time in MS : " + sw.ElapsedMilliseconds);
				timeStopped = true;//turn on a boolean that will display the time and visited nodes on screen
				
				Node currNode = currentNode;//temp variable to get the children of the end node
				
				List<Node> path = new List<Node>();//create a temporary list of nodes ti visualize the path
				
				while (currNode.parent)//while the start node hasnt been reached
				{
					path.Add(currNode);//add the temporary node
					currNode = currNode.parent;//and get its parent
				}
				//path.Reverse();
				
				//store the path in the temporary list
				tempList = path;
				for (int a = 0; a < tempList.Count; a++) {
					//UnityEngine.Debug.Log("F cost :" + tempList[a].fCost);
					print("F cost :" + tempList[a].fCost);
				}
			}
			else
			{
				//depth++;

				for (int i = 0; i < nodesProcessed.Count; i++)
				{
					if (depth < nodesProcessed[i].fCost)
					{
						depth = nodesProcessed[i].fCost;
						break;
					}
				}

				thePath = aStar(startNodeIn,endNodeIn);
			}
			
			//if the end node is not found remove the current node from the open list and add to closed list
			

		//assign the visited nodes
			nodesVisied = nodesProcessed.Count;
		//return the temporary list containing the path. It will be used by the colourAllNodes method
	}
		return tempList;

	}

	
	//creates the map when the generate map button is pressed
	public void MapGUIToggle()
	{
		turnMapGUIOff = true;
	}

	//turns the GUI on
	public void turnInterfaceOn()
	{
		userInterfaceScript.interfaceOn = true;
	}

	//turns the light on
	public void turnOnLight()
	{
		directionalLight.enabled = true;
	}

}