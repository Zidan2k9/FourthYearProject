using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
	public Vector3 worldPosition; // what is the worldPosition of this Node instance in the engine
	//public float nodeGridXPosition;//what is the X co-ordinate of this Node in grid form
	//public float nodeGridYPosition;//what is the Y co-ordinate of this Node in grid form
	public bool walkable;
	public Vector2 nodeGridPosition;
	public int thisFCost;
	int heapIndex;

	//[HideInInspector]
	public int gCost; //The G cost of the node
	//[HideInInspector]
	public int hCost;//The H cost of the node
	public Node parent;//What is the parrent node of this node
	
	public Node(Vector3 thisWorldPosition,int nodeX,int nodeY,bool isWalkable) 
	{
		worldPosition = thisWorldPosition;
		nodeGridPosition.x = nodeX;
		nodeGridPosition.y = nodeY;
		walkable = isWalkable;
	}

	public Node(Vector3 thisWorldPositon,int nodeX,int nodeY)
	{
		worldPosition = thisWorldPositon;
		nodeGridPosition.x = nodeX;
		nodeGridPosition.y = nodeY;
	}
	public int fCost //method to return the fCost of a node, this will never be edited so only a get method is required
	{
		get
		{
			return thisFCost = gCost + hCost;
		}

		set
		{

		}
	}

	public List<Node> getChildren()// method to get the children of a node. Used in the failed implementation of IDA*
	{
		List<Node> children = new List<Node>();
		Node curNode = this;

		if (curNode.parent != null)
		{
			while (curNode.parent)
			{
				children.Add(curNode);
				curNode = curNode.parent;
			}
		}
		return children;
	}
	

	public int CompareTo(Node nodeToCompare)// method to compare 2 nodes for fCost. Unused
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);

		if (compare == 0){
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
