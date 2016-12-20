using UnityEngine;
using System.Collections;

/**
 * Used to update the Camera position and distance from the ground.
 */
public class Controls : MonoBehaviour {
    public float scrollSpeed = 0.001f;
    public ConstructionNetwork constructNetwork;
    private Camera myCamera;
    private float mapSize;
    private float zoomLevel;//x zoom and y zoom... might need to be fixed later on to account for different screen sizes

    /**
     * Creates an instance of BuildingNetwork when a level is loaded.
     */
    void Awake()
    {
        constructNetwork = new ConstructionNetwork();
    }

    /**
     * Initializes variables used by the Controls class.
     */
    void Start () {
        myCamera = GetComponent<Camera>();
        mapSize = 39f;//will need to pass in a variable here from a level manager when I get it in
	}
	
    /**
     * Updates the camera based on the player's inputs.
     */
	void Update () {
        //Moving the camera around the x and y directions by moving the mouse to the edges of the screen
        Vector3 currentPos = transform.position;
        float cameraSize = myCamera.orthographicSize / 5;
        if (Input.mousePosition.y >= Screen.height * 0.98 && currentPos.y < mapSize - myCamera.orthographicSize)
        {
            transform.position = new Vector3(currentPos.x, currentPos.y + scrollSpeed * Time.deltaTime * cameraSize, currentPos.z);
        }
        if (Input.mousePosition.y <= Screen.height * 0.02 && currentPos.y > myCamera.orthographicSize)
        {
            transform.position = new Vector3(currentPos.x, currentPos.y - scrollSpeed * Time.deltaTime * cameraSize, currentPos.z);
        }
        if (Input.mousePosition.x >= Screen.width * 0.98 && currentPos.x < mapSize - myCamera.aspect * myCamera.orthographicSize)
        {
            transform.position = new Vector3(currentPos.x + scrollSpeed * Time.deltaTime * cameraSize, currentPos.y, currentPos.z);
        }
        if (Input.mousePosition.x <= Screen.width * 0.02 && currentPos.x > myCamera.aspect * myCamera.orthographicSize)
        {
            transform.position = new Vector3(currentPos.x - scrollSpeed * Time.deltaTime * cameraSize, currentPos.y, currentPos.z);
        }

        //Zooming in and out with the mouse scrollwheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f && myCamera.orthographicSize > 1)
        {
            myCamera.orthographicSize -= 1;
        }
        else if (scroll < 0f && myCamera.orthographicSize < 8)
        {
            myCamera.orthographicSize += 1;
        }

        //keeps the camera from showing anything outside the map area when zooming out at the edges
        if (currentPos.y > mapSize - myCamera.orthographicSize)
        {
            transform.position = new Vector3(currentPos.x, mapSize - myCamera.orthographicSize, currentPos.z);
        }
        if (currentPos.y < myCamera.orthographicSize)
        {
            transform.position = new Vector3(currentPos.x, myCamera.orthographicSize, currentPos.z);
        }
        if (currentPos.x > mapSize - myCamera.aspect * myCamera.orthographicSize + 1)
        {
            transform.position = new Vector3(mapSize - myCamera.aspect * myCamera.orthographicSize, currentPos.y, currentPos.z);
        }
        if (currentPos.x < myCamera.aspect * myCamera.orthographicSize - 1)
        {
            transform.position = new Vector3(myCamera.aspect * myCamera.orthographicSize, currentPos.y, currentPos.z);
        }
    }
}
