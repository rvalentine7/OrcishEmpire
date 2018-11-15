using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Bridge is one segment of a larger bridge.  It keeps track of other bridge segments
 * for the purposes of destroying the overall bridge.
 */
public class Bridge : MonoBehaviour {
    public Sprite northConnection;
    public Sprite southConnection;
    public Sprite westConnection;
    public Sprite eastConnection;
    public Sprite verticalMiddle;
    public Sprite horizontalMiddle;

    public const string VERTICAL = "Vertical";
    public const string HORIZONTAL = "Horizontal";
    public const string NORTH_END = "North End";
    public const string SOUTH_END = "South End";
    public const string WEST_END = "West End";
    public const string EAST_END = "East End";

    private List<GameObject> connectedBridgeObjects;
    private GameObject world;
    private World myWorld;
    GameObject[,] structureArr;

    // Use this for initialization
    void Start () {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        structureArr = myWorld.constructNetwork.getConstructArr();

        updateNearbyRoads(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Sets references to the other segments of the bridge that this bridge is a segment of.
     * This is used for destroying all of the bridge segments when a bridge segment is destroyed.
     * @param connectedBridgeObjects a list of the other bridge segments
     */
    public void setConnectedBridgeObjects(List<GameObject> connectedBridgeObjects)
    {
        this.connectedBridgeObjects = connectedBridgeObjects;
    }

    /**
     * Sets the sprite used by the gameobject
     * @param appearance the string description of what sprite to set
     */
    public void setSpriteAppearance(string appearance)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (appearance.Equals(VERTICAL))
        {
            spriteRenderer.sprite = verticalMiddle;
        }
        else if (appearance.Equals(HORIZONTAL))
        {
            spriteRenderer.sprite = horizontalMiddle;
        }
        else if (appearance.Equals(NORTH_END))
        {
            spriteRenderer.sprite = northConnection;
        }
        else if (appearance.Equals(SOUTH_END))
        {
            spriteRenderer.sprite = southConnection;
        }
        else if (appearance.Equals(WEST_END))
        {
            spriteRenderer.sprite = westConnection;
        }
        else if (appearance.Equals(EAST_END))
        {
            spriteRenderer.sprite = eastConnection;
        }
    }

    /**
     * Updates the connecting roads
     * @param bridgeSegment the bridge segment we are updating the roads around
     */
    public void updateNearbyRoads(GameObject bridgeSegment)
    {
        if ((int)bridgeSegment.transform.position.y + 1 < myWorld.mapSize
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y + 1] != null
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y + 1].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y + 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)bridgeSegment.transform.position.y - 1 > 0
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y - 1] != null
            && structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y - 1].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x, (int)bridgeSegment.transform.position.y - 1].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)bridgeSegment.transform.position.x - 1 > 0
            && structureArr[(int)bridgeSegment.transform.position.x - 1, (int)bridgeSegment.transform.position.y] != null
            && structureArr[(int)bridgeSegment.transform.position.x - 1, (int)bridgeSegment.transform.position.y].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x - 1, (int)bridgeSegment.transform.position.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
        if ((int)bridgeSegment.transform.position.x + 1 < myWorld.mapSize
            && structureArr[(int)bridgeSegment.transform.position.x + 1, (int)bridgeSegment.transform.position.y] != null
            && structureArr[(int)bridgeSegment.transform.position.x + 1, (int)bridgeSegment.transform.position.y].tag == World.ROAD)
        {
            structureArr[(int)bridgeSegment.transform.position.x + 1, (int)bridgeSegment.transform.position.y].GetComponent<RoadInformation>().updateRoadConnection();
        }
    }

    /**
     * Destroys this bridge segment and all of the other bridge segments from the same bridge
     */
    public void destroyBridge()
    {
        structureArr = myWorld.constructNetwork.getConstructArr();

        foreach (GameObject connectedBridge in connectedBridgeObjects)
        {
            structureArr[(int)connectedBridge.transform.position.x, (int)connectedBridge.transform.position.y] = null;
            updateNearbyRoads(connectedBridge);
            Destroy(connectedBridge);
        }

        structureArr[(int)gameObject.transform.position.x, (int)gameObject.transform.position.y] = null;
        updateNearbyRoads(gameObject);

        Destroy(gameObject);
    }
}
