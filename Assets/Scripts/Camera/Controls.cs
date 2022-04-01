using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Used to update the Camera position and distance from the ground.
/// </summary>
public class Controls : MonoBehaviour {
    public float scrollSpeed = 0.001f;
    private Camera myCamera;
    private float zoomLevel;//x zoom and y zoom... might need to be fixed later on to account for different screen sizes
    private GameObject world;
    private World myWorld;
    private int mapSize;

    /// <summary>
    /// Initializes variables used by the Controls class.
    /// </summary>
    void Start () {
        myCamera = GetComponent<Camera>();
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        mapSize = myWorld.mapSize;
    }

    /// <summary>
    /// Updates the camera based on the player's inputs.
    /// </summary>
    void Update () {
        //Moving the camera around the x and y directions by moving the mouse to the edges of the screen
        Vector3 currentPos = transform.position;
        float cameraSize = myCamera.orthographicSize / 5;
        float cameraAdjustment = 0.1f * (cameraSize / 1.3f);
        int borderAdjustment = 3;
        //Scrolling up
        if (Input.mousePosition.y >= Screen.height * 0.999 && currentPos.y < mapSize - myCamera.orthographicSize - 1 + cameraAdjustment + (borderAdjustment - 2))
        {
            transform.position = new Vector3(currentPos.x, currentPos.y + scrollSpeed * Time.unscaledDeltaTime * cameraSize, currentPos.z);
        }
        //Scrolling down
        if (Input.mousePosition.y <= Screen.height * 0.001 && currentPos.y > myCamera.orthographicSize - cameraAdjustment - borderAdjustment)
        {
            transform.position = new Vector3(currentPos.x, currentPos.y - scrollSpeed * Time.unscaledDeltaTime * cameraSize, currentPos.z);
        }
        //Scrolling right
        if (Input.mousePosition.x >= Screen.width * 0.999 && currentPos.x < mapSize - myCamera.aspect * myCamera.orthographicSize - 1 + cameraAdjustment + (borderAdjustment - 2))
        {
            transform.position = new Vector3(currentPos.x + scrollSpeed * Time.unscaledDeltaTime * cameraSize, currentPos.y, currentPos.z);
        }
        //Scrolling left
        if (Input.mousePosition.x <= Screen.width * 0.001 && currentPos.x > myCamera.aspect * myCamera.orthographicSize - cameraAdjustment - (borderAdjustment + 2))
        {
            transform.position = new Vector3(currentPos.x - scrollSpeed * Time.unscaledDeltaTime * cameraSize, currentPos.y, currentPos.z);
        }

        //Zooming in and out with the mouse scrollwheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (!EventSystem.current.IsPointerOverGameObject() && scroll > 0f && myCamera.orthographicSize > 1)
        {
            myCamera.orthographicSize -= 1;
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && scroll < 0f && myCamera.orthographicSize < 8)
        {
            myCamera.orthographicSize += 1;
        }

        //keeps the camera from showing anything outside the map area when zooming out at the edges
        //Top
        if (currentPos.y > mapSize - myCamera.orthographicSize - 1 + cameraAdjustment + 1 + (borderAdjustment - 2))
        {
            transform.position = new Vector3(currentPos.x, mapSize - myCamera.orthographicSize - 1 + cameraAdjustment - 1.8f, currentPos.z);
        }
        //Bottom
        if (currentPos.y < myCamera.orthographicSize - cameraAdjustment - 1 - borderAdjustment)
        {
            transform.position = new Vector3(currentPos.x, myCamera.orthographicSize - cameraAdjustment, currentPos.z);
        }
        //Right
        if (currentPos.x > mapSize - myCamera.aspect * myCamera.orthographicSize - 1 + cameraAdjustment + 1 + (borderAdjustment - 2))
        {
            transform.position = new Vector3(mapSize - myCamera.aspect * myCamera.orthographicSize - 1 + cameraAdjustment, currentPos.y, currentPos.z);
        }
        //Left
        if (currentPos.x < myCamera.aspect * myCamera.orthographicSize - cameraAdjustment - 1 - (borderAdjustment + 2))
        {
            transform.position = new Vector3(myCamera.aspect * myCamera.orthographicSize - cameraAdjustment, currentPos.y, currentPos.z);
        }
    }
}
