using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the road sprite and aqueduct information
/// </summary>
public class RoadInformation : MonoBehaviour {
    public Sprite cornerRoad;
    public Sprite crossRoad;
    public Sprite tRoad;
    public Sprite road;

    private GameObject aqueduct;
    private GameObject world;
    private World myWorld;
    private SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        aqueduct = null;
        world = GameObject.Find(World.WORLD_INFORMATION);
        myWorld = world.GetComponent<World>();
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    /**
     * Updates the appearance of the roads as they are added so that they line up with one another
     */
    public void updateRoadConnection()
    {
        Vector2 roadPos = gameObject.transform.position;
        var roadRotation = transform.rotation.eulerAngles;
        roadRotation.z = 0;
        gameObject.transform.rotation = Quaternion.Euler(roadRotation);
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        GameObject[,] terrainArr = myWorld.terrainNetwork.getTerrainArr();

        //check here for what gameobject I want and add it to the buildArr
        int nearbyRoadCount = 0;
        //booleans allow me to where the other roads are
        bool top = false;
        if (structureArr[(int)roadPos.x, (int)roadPos.y + 1] != null
            && (structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag == World.ROAD
            || structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag == World.HIGH_BRIDGE
            || structureArr[(int)roadPos.x, (int)roadPos.y + 1].tag == World.LOW_BRIDGE))
        {
            top = true;
            nearbyRoadCount++;
        }
        bool bot = false;
        if (structureArr[(int)roadPos.x, (int)roadPos.y - 1] != null
            && (structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag == World.ROAD
            || structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag == World.HIGH_BRIDGE
            || structureArr[(int)roadPos.x, (int)roadPos.y - 1].tag == World.LOW_BRIDGE))
        {
            bot = true;
            nearbyRoadCount++;
        }
        bool left = false;
        if (structureArr[(int)roadPos.x - 1, (int)roadPos.y] != null
            && (structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag == World.ROAD
            || structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag == World.HIGH_BRIDGE
            || structureArr[(int)roadPos.x - 1, (int)roadPos.y].tag == World.LOW_BRIDGE))
        {
            left = true;
            nearbyRoadCount++;
        }
        bool right = false;
        if (structureArr[(int)roadPos.x + 1, (int)roadPos.y] != null
            && (structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag == World.ROAD
            || structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag == World.HIGH_BRIDGE
            || structureArr[(int)roadPos.x + 1, (int)roadPos.y].tag == World.LOW_BRIDGE))
        {
            right = true;
            nearbyRoadCount++;
        }
        //checking for stairs
        if (top == false && terrainArr[(int)roadPos.x, (int)roadPos.y + 1] != null
            && terrainArr[(int)roadPos.x, (int)roadPos.y + 1].tag == World.STAIRS)
        {
            top = true;
            nearbyRoadCount++;
        }
        if (bot == false && terrainArr[(int)roadPos.x, (int)roadPos.y - 1] != null
            && terrainArr[(int)roadPos.x, (int)roadPos.y - 1].tag == World.STAIRS)
        {
            bot = true;
            nearbyRoadCount++;
        }
        if (left == false && terrainArr[(int)roadPos.x - 1, (int)roadPos.y] != null
            && terrainArr[(int)roadPos.x - 1, (int)roadPos.y].tag == World.STAIRS)
        {
            left = true;
            nearbyRoadCount++;
        }
        if (right == false && terrainArr[(int)roadPos.x + 1, (int)roadPos.y] != null
            && terrainArr[(int)roadPos.x + 1, (int)roadPos.y].tag == World.STAIRS)
        {
            right = true;
            nearbyRoadCount++;
        }


        //no nearby roads
        if (nearbyRoadCount == 0)
        {
            mySpriteRenderer.sprite = road;
        }

        //Positions in the structureArr:
        //right: structureArr[(int)mousePos.y, (int)mousePos.x + 1]
        //left: structureArr[(int)mousePos.y, (int)mousePos.x - 1]
        //top: structureArr[(int)mousePos.y + 1, (int)mousePos.x]
        //bot: structureArr[(int)mousePos.y - 1, (int)mousePos.x]

        //straight road
        if (nearbyRoadCount == 1)
        {
            mySpriteRenderer.sprite = road;
            if (top || bot)
            {
                //change the z rotation
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.z = 90;
                gameObject.transform.rotation = Quaternion.Euler(rotationVector);
            }
        }

        //straight or corner; depends on if nearby roads are opposite or diagonal
        if (nearbyRoadCount == 2)
        {
            if (right && left)
            {
                mySpriteRenderer.sprite = road;
            }
            else if (top && bot)
            {
                mySpriteRenderer.sprite = road;
                var rotationVector = transform.rotation.eulerAngles;
                rotationVector.z = 90;
                gameObject.transform.rotation = Quaternion.Euler(rotationVector);
            }
            else if (top)
            {
                mySpriteRenderer.sprite = cornerRoad;
                //only needs to be rotated if the second connecting road is on the right
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 270;
                    gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                }
            }
            else if (bot)
            {
                mySpriteRenderer.sprite = cornerRoad;
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 180;
                    gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                }
                else
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 90;
                    gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                }
            }
        }

        //T road
        if (nearbyRoadCount == 3)
        {
            if (top && bot)
            {
                mySpriteRenderer.sprite = tRoad;
                if (right)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 270;
                    gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                }
                else
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 90;
                    gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                }
            }
            else if (right && left)
            {
                mySpriteRenderer.sprite = tRoad;
                //starting rotation is if the third connection is on top, therefore only need to rotation in this
                // case if last connection is bot
                if (bot)
                {
                    var rotationVector = transform.rotation.eulerAngles;
                    rotationVector.z = 180;
                    gameObject.transform.rotation = Quaternion.Euler(rotationVector);
                }
            }
        }

        //crossroad
        if (nearbyRoadCount == 4)
        {
            mySpriteRenderer.sprite = crossRoad;
        }
    }

    /**
     * Updates the connecting neighbors of this aqueduct to also connect to this aqueduct
     */
    public void updateNeighbors()
    {
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        if ((int)gameObject.transform.position.y + 1 < myWorld.mapSize
            && structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y + 1] != null
            && structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y + 1].tag == World.ROAD)
        {
            structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y + 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)gameObject.transform.position.y - 1 > 0
            && structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y - 1] != null
            && structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y - 1].tag == World.ROAD)
        {
            structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y - 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)gameObject.transform.position.x - 1 > 0
            && structureArr[(int)gameObject.transform.position.x - 1, (int)gameObject.transform.position.y] != null
            && structureArr[(int)gameObject.transform.position.x - 1, (int)gameObject.transform.position.y].tag == World.ROAD)
        {
            structureArr[(int)gameObject.transform.position.x - 1, (int)gameObject.transform.position.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)gameObject.transform.position.x + 1 < myWorld.mapSize
            && structureArr[(int)gameObject.transform.position.x + 1, (int)gameObject.transform.position.y] != null
            && structureArr[(int)gameObject.transform.position.x + 1, (int)gameObject.transform.position.y].tag == World.ROAD)
        {
            structureArr[(int)gameObject.transform.position.x + 1, (int)gameObject.transform.position.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
    }

    /// <summary>
    /// Sets the aqueduct that is over this road object
    /// </summary>
    /// <param name="aqueduct">the aqueduct to go over this road</param>
    public void setAqueduct(GameObject aqueduct)
    {
        this.aqueduct = aqueduct;
    }

    /// <summary>
    /// Gets the aqueduct over this road object
    /// </summary>
    /// <returns>the aqueduct over this road object if there is one, null otherwise</returns>
    public GameObject getAqueduct()
    {
        return this.aqueduct;
    }

    /// <summary>
    /// Destroys the aqueduct if there is one on top of the road, otherwise destroy the road
    /// </summary>
    public void destroyRoad()
    {
        if (this.aqueduct != null)
        {
            aqueduct.GetComponent<Aqueduct>().destroyAqueduct();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
