using UnityEngine;
using System.Collections;

public class NodeColourChanger : MonoBehaviour 
{
	Ray ray;
	RaycastHit hit;
	public int numberOfClicks;
	public Vector3 startNodeTransform;
	public Vector3 endNodeTransform;
	public Vector3 startNodeTransform2;
	public Vector3 endNodeTransform2;
	public Vector3 startNodeTransform3;
	public Vector3 endNodeTransform3;
	private float cameraMoveSpeed;
	private float scroll;
	public int buttonX;
	public int buttonY;
	[HideInInspector]
	public bool startSearchBool;
	int per;
	public GameObject gridHolder;
	public int numOfNPCS;
	public bool generateObsBool;
	public int startPoints;
	public int endPoints;
	
	void Start()
	{
		numOfNPCS = 1;
		numberOfClicks = 0;
		cameraMoveSpeed = 4.0f;
		//scroll = Input.GetAxis("Mouse ScrollWheel");
		//scroll = Input.GetAxisRaw("Mouse ScrollWheel");
		buttonX = 400;
		buttonY = 150;
		startSearchBool = false;
		generateObsBool = false;
	}
	
	void Update()
	{

//				if (scroll < 0)
//					{
//					camera.orthographicSize -= .1f;
//				}
//				if (scroll > 0)
//				{
//					camera.orthographicSize += .1f;
//				}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			camera.orthographicSize -= .1f;			//CAMERA ZOOM
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			camera.orthographicSize += .1f;
		}

//		float scroll = Input.GetAxis("Mouse ScrollWheel");
//
//		transform.Translate (0, scroll * zoomSpeed, scroll * zoomSpeed, Space.World);

		if (Input.GetKey(KeyCode.D))
		{
			transform.position += Vector3.right * cameraMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.position += Vector3.left * cameraMoveSpeed * Time.deltaTime;		//CAMERA MOVEMENT
		}
		if (Input.GetKey(KeyCode.W))
		{
			transform.position += Vector3.up * cameraMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.position += Vector3.down * cameraMoveSpeed * Time.deltaTime;
		}

		ray = Camera.main.ScreenPointToRay(Input.mousePosition);//use a raycast to check if a mouse click collides with a game object
		if(Physics.Raycast(ray, out hit))
		{
			//Debug.Log("The number of clicks is " + numberOfClicks);
			if(Input.GetMouseButtonDown(0))
			{
				numberOfClicks++;
				
				//if (numberOfClicks == 1)
				if(numberOfClicks % 2 == 1)
				{
					startPoints++;//This was used to attempt multiple NPCs
					hit.collider.renderer.material.color = Color.green; //select start nodes

					if (startPoints == 1)
					{
						startNodeTransform = hit.collider.gameObject.transform.position;
					}
					if (startPoints == 2)
					{
						startNodeTransform2 = hit.collider.gameObject.transform.position;
					}
					if (startPoints == 3)
					{
						startNodeTransform3 = hit.collider.gameObject.transform.position;
					}
				}
				//if(numberOfClicks == 2)
				if(numberOfClicks % 2 == 0)
				{
					endPoints++;
					hit.collider.renderer.material.color = Color.red; //select endnodes
					//Debug.Log("The world position of the target node is " + hit.collider.gameObject.transform.position);
					//endNodeTransform = hit.collider.gameObject.transform.position;

					if (endPoints == 1)
					{
						endNodeTransform = hit.collider.gameObject.transform.position; //store the transforms of the nodes
					}
					if (startPoints == 2)
					{
						endNodeTransform2 = hit.collider.gameObject.transform.position;
					}
					if (startPoints == 3)
					{
						endNodeTransform3 = hit.collider.gameObject.transform.position;
					}
				}
				//after selecting the start and end nodes, the user can choose obstacles
				if (numberOfClicks > 2 * numOfNPCS && startSearchBool == false)
				{
					hit.collider.renderer.material.color = Color.gray;
					hit.collider.gameObject.GetComponent<Node>().walkable = false;
				}
			}
		}
	}

	void OnGUI()
	{
//		if(GUI.Button (new Rect (100, 200, 100, 100), "Start Search"))
//		{
//			startSearchBool = true;
//		}
//		if (GUI.Button(new Rect(100,300,100,100),"Generate Obstacles"))
//		{
//			generateObsBool = true;
//		}
	}

	
}