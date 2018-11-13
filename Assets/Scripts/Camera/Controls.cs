using UnityEngine;
using System.Collections;

/**
 * Used to update the Camera position and distance from the ground.
 */
public class Controls : MonoBehaviour {
    public float scrollSpeed = 0.001f;
    private Camera myCamera;
    private float zoomLevel;//x zoom and y zoom... might need to be fixed later on to account for different screen sizes
    private GameObject world;
    private World myWorld;
    private int mapSize;
    
    /**
     * Initializes variables used by the Controls class.
     */
    void Start () {
        myCamera = GetComponent<Camera>();
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        mapSize = myWorld.mapSize;
    }
	
    /**
     * Updates the camera based on the player's inputs.
     */
	void Update () {
        //Moving the camera around the x and y directions by moving the mouse to the edges of the screen
        Vector3 currentPos = transform.position;
        float cameraSize = myCamera.orthographicSize / 5;
        float cameraAdjustment = 0.1f * (cameraSize / 1.3f);
        //Scrolling up
        if (Input.mousePosition.y >= Screen.height * 0.98 && currentPos.y < mapSize - myCamera.orthographicSize - 1 + cameraAdjustment)
        {
            transform.position = new Vector3(currentPos.x, currentPos.y + scrollSpeed * Time.deltaTime * cameraSize, currentPos.z);
        }
        //Specific camera adjustments for the UI at the bottom of the screen
        float uiAdjustment = 0.1f;
        if (myCamera.orthographicSize == 1)
        {
            uiAdjustment /= (myCamera.orthographicSize / 7);
        }
        else if (myCamera.orthographicSize == 2)
        {
            uiAdjustment /= (myCamera.orthographicSize / 19);
        }
        else if (myCamera.orthographicSize == 3)
        {
            uiAdjustment /= (myCamera.orthographicSize / 36);
        }
        else if (myCamera.orthographicSize == 4)
        {
            uiAdjustment /= (myCamera.orthographicSize / 55);
        }
        else if (myCamera.orthographicSize == 5)
        {
            uiAdjustment /= (myCamera.orthographicSize / 92);
        }
        else if (myCamera.orthographicSize == 6)
        {
            uiAdjustment /= (myCamera.orthographicSize / 113);
        }
        else if (myCamera.orthographicSize == 7)
        {
            uiAdjustment /= (myCamera.orthographicSize / 158);
        }
        else if (myCamera.orthographicSize == 8)
        {
            uiAdjustment /= (myCamera.orthographicSize / 195);
        }
        //Scrolling down
        if (Input.mousePosition.y <= Screen.height * 0.02 && currentPos.y > myCamera.orthographicSize - uiAdjustment - cameraAdjustment)
        {
            transform.position = new Vector3(currentPos.x, currentPos.y - scrollSpeed * Time.deltaTime * cameraSize, currentPos.z);
        }
        //Scrolling right
        if (Input.mousePosition.x >= Screen.width * 0.98 && currentPos.x < mapSize - myCamera.aspect * myCamera.orthographicSize - 1 + cameraAdjustment)
        {
            transform.position = new Vector3(currentPos.x + scrollSpeed * Time.deltaTime * cameraSize, currentPos.y, currentPos.z);
        }
        //Scrolling left
        if (Input.mousePosition.x <= Screen.width * 0.02 && currentPos.x > myCamera.aspect * myCamera.orthographicSize - cameraAdjustment)
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
        //Top
        if (currentPos.y > mapSize - myCamera.orthographicSize - 1 + cameraAdjustment + 1)
        {
            transform.position = new Vector3(currentPos.x, mapSize - myCamera.orthographicSize - 1 + cameraAdjustment - 1.8f, currentPos.z);
        }
        //Bottom
        if (currentPos.y < myCamera.orthographicSize - uiAdjustment - cameraAdjustment - 1)
        {
            transform.position = new Vector3(currentPos.x, myCamera.orthographicSize - uiAdjustment - cameraAdjustment, currentPos.z);
        }
        //Right
        if (currentPos.x > mapSize - myCamera.aspect * myCamera.orthographicSize - 1 + cameraAdjustment + 1)
        {
            transform.position = new Vector3(mapSize - myCamera.aspect * myCamera.orthographicSize - 1 + cameraAdjustment, currentPos.y, currentPos.z);
        }
        //Left
        if (currentPos.x < myCamera.aspect * myCamera.orthographicSize - cameraAdjustment - 1)
        {
            transform.position = new Vector3(myCamera.aspect * myCamera.orthographicSize - cameraAdjustment, currentPos.y, currentPos.z);
        }
    }
}
