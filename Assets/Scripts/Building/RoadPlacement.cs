using UnityEngine;
using System.Collections;

/**
 * Lets the player place roads in the world.
 * This class also gives visual feedback as to whether or not
 * a road can be placed in a given location.  It also updates
 * roads to connect visually when placed next to each other.
 */
public class RoadPlacement : MonoBehaviour {
    private bool validPlacement;
    public Sprite possibleSprite;
    public Sprite possibleXRoadSprite;
    public Sprite possibleTRoadSprite;
    public Sprite possibleCornerRoadSprite;
    public Sprite impossibleSprite;
    public GameObject straightRoad;
    public GameObject cornerRoad;
    public GameObject tRoad;
    public GameObject crossRoad;
    

	/**
     * Initializes the RoadPlacement class.
     */
	void Start () {
        validPlacement = true;
    }
	
	/**
     * Moves the visual road icon around to tell whether a location is viable to build on.
     * The player can build a road game object at the mouse location if it is in a viable build location.
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
        //Check if the structure is currently in a valid location
        //need to make sure there isn't a road/building already in the place where the road is currently located
        //check roadArr for the x/y coordinates of the mousePos to see if there is anything there...
        GameObject[,] structureArr = myControls.constructNetwork.getConstructArr();

        bool valid = true;
        //39 in the follow line needs to be replaced with mapSize when more levels are added
        if ((mousePos.x <= 0 && mousePos.x >= 39 && mousePos.y <= 0 && mousePos.y >= 39))
        {
            valid = false;
        }
        //can't place a road on other objects
        if (valid && structureArr[(int)mousePos.y, (int)mousePos.x] != null
            && (structureArr[(int) mousePos.y, (int) mousePos.x].tag == "Road"
            || structureArr[(int)mousePos.y, (int)mousePos.x].tag == "Building"))
        {
            valid = false;
        }
        //raycast to avoid building underneath the UI
        //RaycastHit2D hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics2D.Raycast(Input.mousePosition, -Vector2.up))
        //{
        //    Debug.Log("hit");
        //}

        if (valid && (structureArr[(int) mousePos.y, (int) mousePos.x] == null))
        {
            validPlacement = true;
            //Border is green when it is possible to place a sprite in its current location
            updateRoadPreview(gameObject, structureArr);
        } else
        {
            validPlacement = false;
            //Border turns red when it is impossible to place a sprite in its current location
            GetComponent<SpriteRenderer>().sprite = impossibleSprite;
        }

        //If the road is in a valid location and the left mouse is clicked, place it in the world
        if (Input.GetMouseButton(0) && validPlacement)
        {
            //when mouse down, can place road in any viable location the mouse moves to
            if (mousePos.x > 0 && mousePos.x < 39 && mousePos.y > 0 && mousePos.y < 39)
            {//swap 39s with mapSize
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
            }

            //update the new road
            updateRoadConnection(gameObject, structureArr);
            structureArr = myControls.constructNetwork.getConstructArr();
            //go through roads attached to the new road to update them visibly
            //do not update the roads outside building limits
            if ((int)mousePos.y + 1 < 39 && structureArr[(int)mousePos.y + 1, (int)mousePos.x] != null
               && structureArr[(int)mousePos.y + 1, (int)mousePos.x].tag == "Road")
            {
                //update road above the one you are trying to build
                updateRoadConnection(structureArr[(int)mousePos.y + 1, (int)mousePos.x], structureArr);
            }
            if ((int)mousePos.y - 1 > 0 && structureArr[(int)mousePos.y - 1, (int)mousePos.x] != null
                && structureArr[(int)mousePos.y - 1, (int)mousePos.x].tag == "Road")
            {
                //update road below the one you are trying to build
                updateRoadConnection(structureArr[(int)mousePos.y - 1, (int)mousePos.x], structureArr);
            }
            if ((int)mousePos.x - 1 > 0 && structureArr[(int)mousePos.y, (int)mousePos.x - 1] != null
                && structureArr[(int)mousePos.y, (int)mousePos.x - 1].tag == "Road")
            {
                //update road to the left of the one you are trying to build
                updateRoadConnection(structureArr[(int)mousePos.y, (int)mousePos.x - 1], structureArr);
            }
            if ((int)mousePos.x + 1 < 39 && structureArr[(int)mousePos.y, (int)mousePos.x + 1] != null
                && structureArr[(int)mousePos.y, (int)mousePos.x + 1].tag == "Road")
            {
                //update road to the right of the one you are trying to build
                updateRoadConnection(structureArr[(int)mousePos.y, (int)mousePos.x + 1], structureArr);
            }

            if (mousePos.x > 0 && mousePos.x < 39 && mousePos.y > 0 && mousePos.y < 39)
            {//swap 39s with mapSize
                transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
            }
        }
        if (mousePos.x > 0 && mousePos.x < 39 && mousePos.y > 0 && mousePos.y < 39)
        {//swap 39s with mapSize
            transform.position = Vector2.Lerp(transform.position, mousePos, 1f);
        }
    }
    
    /**
     * Updates the appearance of the roads as they are added so that they line up with one another
     * @param road is the road being updated
     * @param structureArr is a multidimensional array containing all of the roads and buildings
     */
    void updateRoadConnection(GameObject road, GameObject[,] structureArr)
    {
        GameObject myCamera = GameObject.Find("Main Camera");
        Controls myControls = myCamera.GetComponent<Controls>();
        Vector2 roadPos = road.transform.position;

        //check here for what gameobject I want and add it to the buildArr
        int nearbyRoadCount = 0;
        //booleans allow me to where the other roads are
        bool top = false;
        if (structureArr[(int)roadPos.y + 1, (int)roadPos.x] != null
            && structureArr[(int)roadPos.y + 1, (int)roadPos.x].tag == "Road")
        {
            top = true;
            nearbyRoadCount++;
        }
        bool bot = false;
        if (structureArr[(int)roadPos.y - 1, (int)roadPos.x] != null
            && structureArr[(int)roadPos.y - 1, (int)roadPos.x].tag == "Road")
        {
            bot = true;
            nearbyRoadCount++;
        }
        bool left = false;
        if (structureArr[(int)roadPos.y, (int)roadPos.x - 1] != null
            && structureArr[(int)roadPos.y, (int)roadPos.x - 1].tag == "Road")
        {
            left = true;
            nearbyRoadCount++;
        }
        bool right = false;
        if (structureArr[(int)roadPos.y, (int)roadPos.x + 1] != null
            && structureArr[(int)roadPos.y, (int)roadPos.x + 1].tag == "Road")
        {
            right = true;
            nearbyRoadCount++;
        }


        //no nearby roads
        GameObject roadObj;
        if (nearbyRoadCount == 0)
        {
            roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
            myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
        }

        //Positions in the structureArr:
        //right: structureArr[(int)mousePos.y, (int)mousePos.x + 1]
        //left: structureArr[(int)mousePos.y, (int)mousePos.x - 1]
        //top: structureArr[(int)mousePos.y + 1, (int)mousePos.x]
        //bot: structureArr[(int)mousePos.y - 1, (int)mousePos.x]

        //straight road
        if (nearbyRoadCount == 1)
        {
            roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
            if (top || bot)
            {
                //change the z rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.z = 90;
                roadObj.transform.rotation = Quaternion.Euler(rotationVector);
            }
            myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
        }

        //straight or corner; depends on if nearby roads are opposite or diagonal
        if (nearbyRoadCount == 2)
        {
            if (right && left)
            {
                roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
                myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
            }
            else if (top && bot)
            {
                roadObj = Instantiate(straightRoad, roadPos, Quaternion.identity) as GameObject;
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.z = 90;
                roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
            }
            else if (top)
            {
                roadObj = Instantiate(cornerRoad, roadPos, Quaternion.identity) as GameObject;
                //only needs to be rotated if the second connecting road is on the right
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 270;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
            }
            else if (bot)
            {
                roadObj = Instantiate(cornerRoad, roadPos, Quaternion.identity) as GameObject;
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 180;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                else
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 90;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
            }
        }

        //T road
        if (nearbyRoadCount == 3)
        {
            if (top && bot)
            {
                roadObj = Instantiate(tRoad, roadPos, Quaternion.identity) as GameObject;
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 270;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                else
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 90;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
            }
            else if (right && left)
            {
                roadObj = Instantiate(tRoad, roadPos, Quaternion.identity) as GameObject;
                //starting rotation is if the third connection is on top, therefore only need to rotation in this
                // case if last connection is bot
                if (bot)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 180;
                    roadObj.transform.rotation = Quaternion.Euler(rotationVector);
                }
                myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
            }
        }

        //crossroad
        if (nearbyRoadCount == 4)
        {
            roadObj = Instantiate(crossRoad, roadPos, Quaternion.identity) as GameObject;
            myControls.constructNetwork.setConstructArr((int)roadPos.y, (int)roadPos.x, roadObj);
        }

        //if the road is one of those surrounding the newly-placed road, delete it so
        // that there are not two road objects at one location
        if (road != gameObject)
        {
            Destroy(road);
        }
    }

    /**
     * Changes the preview appearance of the road attached to the player's mouse.
     * @param road is the road object being previewed
     * @param structureArr is a multidimensional array containing roads and buildings
     */
    void updateRoadPreview(GameObject road, GameObject[,] structureArr)
    {
        GameObject myCamera = GameObject.Find("Main Camera");
        Controls myControls = myCamera.GetComponent<Controls>();
        Vector2 roadPos = road.transform.position;
        //pretty much the same as the part up above
        //also add structureArr as something that needs to be passed in?

        //checking for what gameobject appearance I want and add it to the buildArr

        //one way to remove repetition would be to have a method for the following which returns
        // a data structure with a length of 5 or a data structure that resizes itself. the first
        // location in the structure would be how many nearby roads followed by all of the sides
        // my current road object is surrounded on
        int nearbyRoadCount = 0;
        bool top = false;
        if ((int)roadPos.y + 1 < 39 && structureArr[(int)roadPos.y + 1, (int)roadPos.x] != null
            && structureArr[(int)roadPos.y + 1, (int)roadPos.x].tag == "Road")
        {
            top = true;
            nearbyRoadCount++;
        }
        bool bot = false;
        if ((int)roadPos.y - 1 > 0 && structureArr[(int)roadPos.y - 1, (int)roadPos.x] != null
            && structureArr[(int)roadPos.y - 1, (int)roadPos.x].tag == "Road")
        {
            bot = true;
            nearbyRoadCount++;
        }
        bool left = false;
        if ((int)roadPos.x - 1 > 0 && structureArr[(int)roadPos.y, (int)roadPos.x - 1] != null
            && structureArr[(int)roadPos.y, (int)roadPos.x - 1].tag == "Road")
        {
            left = true;
            nearbyRoadCount++;
        }
        bool right = false;
        if ((int)roadPos.x + 1 < 39 && structureArr[(int)roadPos.y, (int)roadPos.x + 1] != null
            && structureArr[(int)roadPos.y, (int)roadPos.x + 1].tag == "Road")
        {
            right = true;
            nearbyRoadCount++;
        }


        //no nearby roads
        if (nearbyRoadCount == 0)
        {
            GetComponent<SpriteRenderer>().sprite = possibleSprite;
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        
        //straight road
        if (nearbyRoadCount == 1)
        {
            GetComponent<SpriteRenderer>().sprite = possibleSprite;
            if (top || bot)
            {
                //change the z rotation
                transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        //straight or corner; depends on if nearby roads are opposite or diagonal
        if (nearbyRoadCount == 2)
        {
            if (right && left)
            {
                GetComponent<SpriteRenderer>().sprite = possibleSprite;
            }
            else if (top && bot)
            {
                GetComponent<SpriteRenderer>().sprite = possibleSprite;
                transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else if (top)
            {
                GetComponent<SpriteRenderer>().sprite = possibleCornerRoadSprite;
                //only needs to be rotated if the second connecting road is on the right
                if (right)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
            else if (bot)
            {
                GetComponent<SpriteRenderer>().sprite = possibleCornerRoadSprite;
                if (right)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }
        }

        //T road
        if (nearbyRoadCount == 3)
        {
            if (top && bot)
            {
                GetComponent<SpriteRenderer>().sprite = possibleTRoadSprite;
                if (right)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                }
            }
            else if (right && left)
            {
                GetComponent<SpriteRenderer>().sprite = possibleTRoadSprite;
                //starting rotation is if the third connection is on top, therefore only need to rotation in this
                // case if last connection is bot
                if (bot)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
        }

        //crossroad
        if (nearbyRoadCount == 4)
        {
            GetComponent<SpriteRenderer>().sprite = possibleXRoadSprite;
        }
    }
}
