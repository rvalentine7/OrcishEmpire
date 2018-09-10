using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to supply water to fountains and mud baths
 */
public class Reservoir : MonoBehaviour {
    public float timeDelay;
    public int waterRadius;
    public Sprite filledSprite;
    public Sprite filled1N;
    public Sprite filled1S;
    public Sprite filled1W;
    public Sprite filled1E;
    public Sprite filled2NS;
    public Sprite filled2NW;
    public Sprite filled2NE;
    public Sprite filled2SW;
    public Sprite filled2SE;
    public Sprite filled2WE;
    public Sprite filled3NSE;
    public Sprite filled3NSW;
    public Sprite filled3NWE;
    public Sprite filled3SWE;
    public Sprite filled4;
    public Sprite emptySprite;
    public Sprite empty1N;
    public Sprite empty1S;
    public Sprite empty1W;
    public Sprite empty1E;
    public Sprite empty2NS;
    public Sprite empty2NW;
    public Sprite empty2NE;
    public Sprite empty2SW;
    public Sprite empty2SE;
    public Sprite empty2WE;
    public Sprite empty3NSE;
    public Sprite empty3NSW;
    public Sprite empty3NWE;
    public Sprite empty3SWE;
    public Sprite empty4;

    private GameObject world;
    private World myWorld;
    private SpriteRenderer spriteRenderer;
    //private GameObject[,] structureArr;
    private GameObject[,] terrainArr;
    private int width;
    private int height;
    private Vector2 reservoirPosition;
    private bool nextToWater;
    private List<GameObject> fillSources;//other reservoirs/aqueducts that help fill this one
    private GameObject northConnection;
    private GameObject southConnection;
    private GameObject westConnection;
    private GameObject eastConnection;

    private void Awake()
    {
        world = GameObject.Find("WorldInformation");
        myWorld = world.GetComponent<World>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //structureArr = myWorld.constructNetwork.getConstructArr();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        width = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        height = (int)gameObject.GetComponent<BoxCollider2D>().size.y;
        reservoirPosition = gameObject.transform.position;
        nextToWater = false;
        fillSources = new List<GameObject>();
        northConnection = null;
        southConnection = null;
        westConnection = null;
        eastConnection = null;
    }

    /**
     * Sets the initial sprite for the reservoir
     */
    void Start() {
        for (int i = 0; i < width; i++)
        {
            //Mathf.CeilToInt(width / 2.0f - 1) finds the middle square and then that is subtracted from x to get to the edge to start checking the structure array
            //Left side
            if ((int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1 > 0
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag == "Water"))
            {
                nextToWater = true;
            }
            //Right side
            else if ((int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width < myWorld.mapSize - 1
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag == "Water"))
            {
                nextToWater = true;
            }
            //Top side
            else if ((int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height < myWorld.mapSize - 1
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height].tag == "Water"))
            {
                nextToWater = true;
            }
            //Bottom side
            else if ((int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1 > 0
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1].tag == "Water"))
            {
                nextToWater = true;
            }
        }
        if (!nextToWater)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = emptySprite;
        }
        else
        {
            fillSources.Add(gameObject);
            //Update all nearby tiles with water
            updatePipes(true);
        }
        updateConnections();
        updateNeighbors();
    }

    /**
     * Updates the appearance of the reservoir and determines if it is filled
     */
    void Update() {
        //TODO: have public methods for dealing with aqueducts.  use enums for determining which sprite should be displayed?
        //make sure to be calling add and remove fill sources to update water supply
    }

    /**
     * Adds a reservoir to the list of reservoirs helping fill this reservoir with water
     * @param the reservoir that is helping fill this reservoir
     */
    public void addFillSources(GameObject reservoir)
    {
        if (fillSources.Count == 0 && !nextToWater)
        {
            //Update all nearby tiles with water
            updatePipes(true);
        }
        fillSources.Add(reservoir);
    }

    /**
     * Removes a reservoir from the list of reservoirs helping fill this reservoir with water
     * @param the reservoir that is no longer helping to fill this reservoir
     */
    public void removeFillSources(GameObject reservoir)
    {
        if (fillSources.Count == 1 && !nextToWater)
        {
            //Update all nearby tiles to no longer have water from this reservoir
            updatePipes(false);
        }
        fillSources.Remove(reservoir);
    }

    /**
     * Gets whether the reservoir is next to water and can fill other reservoirs/aqueducts
     * @return whether the reservoir is next to water
     */
    public bool getNextToWater()
    {
        return nextToWater;
    }

    /**
     * Gets whether the reservoir is filled with water for the purposes of filling fountains/reservoirs/aqueducts
     * @return whether the reservoir is filled
     */
    public bool getFilled()
    {
        return fillSources.Count > 0 || nextToWater;
    }

    /**
     * Update water in nearby tiles
     * @param supplying whether this reservoir should be supplying water to nearby tiles
     */
    public void updatePipes(bool supplying)
    {
        for (int i = 0; i < waterRadius * 2; i++)
        {
            for (int j = 0; j < waterRadius * 2; j++)
            {
                if (Mathf.RoundToInt(gameObject.transform.position.x) - waterRadius + i >= 0
                        && Mathf.RoundToInt(gameObject.transform.position.y) - waterRadius + j >= 0
                        && Mathf.RoundToInt(gameObject.transform.position.x) - waterRadius + i <= 40
                        && Mathf.RoundToInt(gameObject.transform.position.y) - waterRadius + j <= 40
                        && terrainArr[Mathf.RoundToInt(gameObject.transform.position.x) - waterRadius + i,
                        Mathf.RoundToInt(gameObject.transform.position.y) - waterRadius + j] != null)
                {
                    Tile tile = terrainArr[Mathf.RoundToInt(gameObject.transform.position.x) - waterRadius + i,
                        Mathf.RoundToInt(gameObject.transform.position.y) - waterRadius + j].GetComponent<Tile>();
                    if (supplying)
                    {
                        tile.addWaterPipes(gameObject);
                    }
                    else
                    {
                        tile.removeWaterPipes(gameObject);
                    }
                }
            }
        }
    }

    /**
     * Updates the connections this reservoir has to nearby aqueducts/reservoirs
     */
    public void updateConnections()
    {
        //Check for other reservoirs, aqueducts, and arches (roads with aqueduct objects) around the reservoir
        //Update art based on available connection sources and whether filled or empty
        //Update north/south/west/eastConnection with what is valid

        //anything that lines up with the center of the reservoir is a valid connection
        GameObject[,] structureArr = myWorld.constructNetwork.getConstructArr();
        Vector2 reservoirCenter = new Vector2(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y));
        northConnection = null;
        southConnection = null;
        westConnection = null;
        eastConnection = null;
        if ((int)reservoirCenter.y + 2 < myWorld.mapSize && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2] != null
            && ((structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].tag == "Road"
                && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2].transform.position.x) == Mathf.RoundToInt(transform.position.x))))
        {
            northConnection = structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y + 2];
        }
        if ((int)reservoirCenter.y - 2 > 0 && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2] != null
            && ((structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].tag == "Road"
                && structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2].transform.position.x) == Mathf.RoundToInt(transform.position.x))))
        {
            southConnection = structureArr[(int)reservoirCenter.x, (int)reservoirCenter.y - 2];
        }
        if ((int)reservoirCenter.x - 2 > 0 && structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y] != null
            && ((structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].tag == "Road"
                && structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y].transform.position.y) == Mathf.RoundToInt(transform.position.y))))
        {
            westConnection = structureArr[(int)reservoirCenter.x - 2, (int)reservoirCenter.y];
        }
        if ((int)reservoirCenter.x + 2 < myWorld.mapSize && structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y] != null
            && ((structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].tag == "Road"
                && structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].GetComponent<RoadInformation>().getAqueduct() != null)
            || structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].GetComponent<Aqueduct>() != null
            || (structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].GetComponent<Reservoir>() != null
                && Mathf.RoundToInt(structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y].transform.position.y) == Mathf.RoundToInt(transform.position.y))))
        {
            eastConnection = structureArr[(int)reservoirCenter.x + 2, (int)reservoirCenter.y];
        }
        //Update sprite based on connections
        if (northConnection != null && southConnection != null && westConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled4;
            }
            else
            {
                spriteRenderer.sprite = empty4;
            }
        }
        else if (northConnection != null && southConnection != null && westConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled3NSW;
            }
            else
            {
                spriteRenderer.sprite = empty3NSW;
            }
        }
        else if (northConnection != null && southConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled3NSE;
            }
            else
            {
                spriteRenderer.sprite = empty3NSE;
            }
        }
        else if (northConnection != null && westConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled3NWE;
            }
            else
            {
                spriteRenderer.sprite = empty3NWE;
            }
        }
        else if (southConnection != null && westConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled3SWE;
            }
            else
            {
                spriteRenderer.sprite = empty3SWE;
            }
        }
        else if (northConnection != null && southConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled2NS;
            }
            else
            {
                spriteRenderer.sprite = empty2NS;
            }
        }
        else if (northConnection != null && westConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled2NW;
            }
            else
            {
                spriteRenderer.sprite = empty2NW;
            }
        }
        else if (northConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled2NE;
            }
            else
            {
                spriteRenderer.sprite = empty2NE;
            }
        }
        else if (southConnection != null && westConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled2SW;
            }
            else
            {
                spriteRenderer.sprite = empty2SW;
            }
        }
        else if (southConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled2SE;
            }
            else
            {
                spriteRenderer.sprite = empty2SE;
            }
        }
        else if (westConnection != null && eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled2WE;
            }
            else
            {
                spriteRenderer.sprite = empty2WE;
            }
        }
        else if (northConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled1N;
            }
            else
            {
                spriteRenderer.sprite = empty1N;
            }
        }
        else if (southConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled1S;
            }
            else
            {
                spriteRenderer.sprite = empty1S;
            }
        }
        else if (westConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled1W;
            }
            else
            {
                spriteRenderer.sprite = empty1W;
            }
        }
        else if (eastConnection != null)
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filled1E;
            }
            else
            {
                spriteRenderer.sprite = empty1E;
            }
        }
        else
        {
            if (fillSources.Count > 0)
            {
                spriteRenderer.sprite = filledSprite;
            }
            else
            {
                spriteRenderer.sprite = emptySprite;
            }
        }
    }

    /**
     * Updates the connections of neighboring aqueducts/reservoirs to this reservoir
     */
    public void updateNeighbors()
    {
        //call this when created (in awake or start)
        //TODO: go into clear and update it to update connections of neighbors when clearing a reservoir
        if (northConnection != null)
        {
            if (northConnection.tag == "Road" && northConnection.GetComponent<RoadInformation>().getAqueduct() != null)
            {
                northConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else if (northConnection.GetComponent<Aqueduct>() != null)
            {
                northConnection.GetComponent<Aqueduct>().updateConnections();
            }
            else if (northConnection.GetComponent<Reservoir>() != null)
            {
                northConnection.GetComponent<Reservoir>().updateConnections();
            }
        }
        if (southConnection != null)
        {
            if (southConnection.tag == "Road" && southConnection.GetComponent<RoadInformation>().getAqueduct() != null)
            {
                southConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else if (southConnection.GetComponent<Aqueduct>() != null)
            {
                southConnection.GetComponent<Aqueduct>().updateConnections();
            }
            else if (southConnection.GetComponent<Reservoir>() != null)
            {
                southConnection.GetComponent<Reservoir>().updateConnections();
            }
        }
        if (westConnection != null)
        {
            if (westConnection.tag == "Road" && westConnection.GetComponent<RoadInformation>().getAqueduct() != null)
            {
                westConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else if (westConnection.GetComponent<Aqueduct>() != null)
            {
                westConnection.GetComponent<Aqueduct>().updateConnections();
            }
            else if (westConnection.GetComponent<Reservoir>() != null)
            {
                westConnection.GetComponent<Reservoir>().updateConnections();
            }
        }
        if (eastConnection != null)
        {
            if (eastConnection.tag == "Road" && eastConnection.GetComponent<RoadInformation>().getAqueduct() != null)
            {
                eastConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().updateConnections();
            }
            else if (eastConnection.GetComponent<Aqueduct>() != null)
            {
                eastConnection.GetComponent<Aqueduct>().updateConnections();
            }
            else if (eastConnection.GetComponent<Reservoir>() != null)
            {
                eastConnection.GetComponent<Reservoir>().updateConnections();
            }
        }
    }

    /**
     * Gets the aqueducts/reservoirs connected to this reservoir
     */
    public List<GameObject> getConnections()
    {
        List<GameObject> connections = new List<GameObject>();
        if (northConnection != null)
        {
            connections.Add(northConnection);
        }
        if (southConnection != null)
        {
            connections.Add(southConnection);
        }
        if (westConnection != null)
        {
            connections.Add(westConnection);
        }
        if (eastConnection != null)
        {
            connections.Add(eastConnection);
        }
        return connections;
    }
}
