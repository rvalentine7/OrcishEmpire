using UnityEngine;
using System.Collections;

/**
 * Lets the player place buildings in the world.
 */
public class BuildingPlacement : MonoBehaviour {
    public int width;
    public int height;
    public Sprite possibleSprite;
    public Sprite impossibleSprite;
    public GameObject building;
    //private int buildingPos;
    private bool validPlacement;

    /**
     * Initializes the BuildingPlacement class
     */
    void Start () {
        //validPlacement = true;
	}
	
	/**
     * Allows the player to place a building in a viable location.
     */
	void Update () {
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Escape))
        {
            //exits out of construction mode if the right mouse button or escape is clicked
            Destroy(gameObject);
        }

        GameObject myCamera = GameObject.Find("Main Camera");
        Controls myControls = myCamera.GetComponent<Controls>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.RoundToInt(mousePos.x);
        mousePos.y = Mathf.RoundToInt(mousePos.y);
        mousePos.z = 0;

        validPlacement = true;
        //Buildings cannot be placed in negative number locations
        if (mousePos.x < 0)
        {
            mousePos.x = 0;
            validPlacement = false;
        }
        if (mousePos.y < 0)
        {
            mousePos.y = 0;
            validPlacement = false;
        }

        GameObject[,] structureArr = myControls.constructNetwork.getConstructArr();

        //39s in the follow line need to be replaced with mapSize when a level manager is added
        if (validPlacement && (mousePos.x <= 0 && mousePos.x >= 39 && mousePos.y <= 0 && mousePos.y >= 39))
        {
            validPlacement = false;
        }
        //can't place a building on other constructs
        int r = 0;
        while (validPlacement && r < height)
        {
            int c = 0;
            while (validPlacement && c < width)
            {
                if (structureArr[(int)mousePos.y + r, (int)mousePos.x + c] != null
                    && (structureArr[(int)mousePos.y + r, (int)mousePos.x + c].tag == "Road"
                    || structureArr[(int)mousePos.y + r, (int)mousePos.x + c].tag == "Building"))
                {
                    validPlacement = false;
                }
                c++;
            }
            r++;
        }

        //make sure sprite is correct based on if the location is possible or not to build on
        if (validPlacement)
        {
            GetComponent<SpriteRenderer>().sprite = possibleSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = impossibleSprite;
        }

        //If the building is in a valid location and the left mouse is clicked, place it in the world
        if (Input.GetMouseButton(0) && validPlacement)
        {
            if (mousePos.x > 0 && mousePos.x < 39 && mousePos.y > 0 && mousePos.y < 39)
            {//swap 39s with mapSize
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
            }
            
            //create the new building in the game world
            GameObject buildingObj = Instantiate(building, mousePos, Quaternion.identity) as GameObject;
            for (r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    myControls.constructNetwork.setConstructArr((int)mousePos.y + r, (int)mousePos.x + c, buildingObj);
                }
            }
        }

        if (mousePos.x > 0 && mousePos.x < 39 && mousePos.y > 0 && mousePos.y < 39)
        {//swap 39s with mapSize
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }
    }
}
