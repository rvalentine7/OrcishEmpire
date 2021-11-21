using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Used to supply water to fountains and mud baths
 */
public class Reservoir : Building {
    public float timeDelay;
    public int waterRadius;
    public float waterDelay;
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
    
    private World myWorld;
    private SpriteRenderer spriteRenderer;
    private GameObject[,] terrainArr;
    private int width;
    private int height;
    private Vector2 reservoirPosition;
    private bool nextToWater;
    private List<GameObject> waterSources;
    private List<GameObject> northWaterSources;//other reservoirs/aqueducts that help fill this one
    private List<GameObject> southWaterSources;
    private List<GameObject> westWaterSources;
    private List<GameObject> eastWaterSources;
    private GameObject northConnection;
    private GameObject southConnection;
    private GameObject westConnection;
    private GameObject eastConnection;
    private bool preparingToUpdateWaterSources;
    private float timeToUpdateWaterSources;
    private bool filling;
    private bool checkingWaterSources;
    private bool nextToClearedWaterStructure;
    private bool supplyingPipes;
    private bool initialCreation;

    private void Awake()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        terrainArr = myWorld.terrainNetwork.getTerrainArr();
        width = (int)gameObject.GetComponent<BoxCollider2D>().size.x;
        height = (int)gameObject.GetComponent<BoxCollider2D>().size.y;
        reservoirPosition = gameObject.transform.position;
        nextToWater = false;
        waterSources = new List<GameObject>();
        northWaterSources = new List<GameObject>();
        southWaterSources = new List<GameObject>();
        westWaterSources = new List<GameObject>();
        eastWaterSources = new List<GameObject>();
        northConnection = null;
        southConnection = null;
        westConnection = null;
        eastConnection = null;
        preparingToUpdateWaterSources = true;
        timeToUpdateWaterSources = Time.time + waterDelay;
        filling = true;
        checkingWaterSources = false;
        nextToClearedWaterStructure = false;
        supplyingPipes = false;
        initialCreation = true;
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
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) - 1, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag.Equals(World.WATER)))
            {
                nextToWater = true;
            }
            //Right side
            else if ((int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width < myWorld.mapSize - 1
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + width, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + i].tag.Equals(World.WATER)))
            {
                nextToWater = true;
            }
            //Top side
            else if ((int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height < myWorld.mapSize - 1
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) + height].tag.Equals(World.WATER)))
            {
                nextToWater = true;
            }
            //Bottom side
            else if ((int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1 > 0
                && terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1] != null
                && (terrainArr[(int)reservoirPosition.x - Mathf.CeilToInt(width / 2.0f - 1) + i, (int)reservoirPosition.y - Mathf.CeilToInt(height / 2.0f - 1) - 1].tag.Equals(World.WATER)))
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
            waterSources.Add(gameObject);
            //Update all nearby tiles with water
            updatePipes(true);
        }
        updateConnections();//art and setting connections
        updateNeighbors();//updating neighbor connections
    }

    /**
     * Updates the appearance of the reservoir and determines if it is filled
     */
    void Update() {
        if (!checkingWaterSources && preparingToUpdateWaterSources && Time.time > timeToUpdateWaterSources)
        {
            //If not filling and getting in here, need to check what water sources this aqueduct still has access to
            if (!filling && !checkingWaterSources && nextToClearedWaterStructure)
            {
                nextToClearedWaterStructure = false;
                checkingWaterSources = true;
                //Get a list of connected aqueducts and reservoirs (probably need to use a coroutine)
                StartCoroutine(checkForWaterSources(waterStructures => {
                    if (waterStructures != null)
                    {
                        checkingWaterSources = false;
                        updateConnectedStructuresAfterClear(waterStructures);
                    }
                }));
            }
            preparingToUpdateWaterSources = false;
            fillNeighbors();
            filling = true;
            if (supplyingPipes && waterSources.Count == 0)
            {
                updatePipes(false);
                //supplyingPipes = false;
            }
        }
        if (!supplyingPipes && waterSources.Count > 0)
        {
            updatePipes(true);
            //supplyingPipes = true;
        }
    }

    /**
     * Adds a reservoir to the list of reservoirs helping fill this reservoir with water
     * @param the reservoir that is helping fill this reservoir
     */
    public void addFillSources(GameObject reservoir)
    {
        if (waterSources.Count == 0 && !nextToWater && !supplyingPipes)
        {
            //Update all nearby tiles with water
            updatePipes(true);
            //supplyingPipes = true;
        }
        waterSources.Add(reservoir);
    }

    /**
     * Removes a reservoir from the list of reservoirs helping fill this reservoir with water
     * @param the reservoir that is no longer helping to fill this reservoir
     */
    public void removeFillSources(GameObject reservoir)
    {
        if (waterSources.Count == 1 && !nextToWater && supplyingPipes)
        {
            //Update all nearby tiles to no longer have water from this reservoir
            updatePipes(false);
            //supplyingPipes = false;
        }
        waterSources.Remove(reservoir);
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
        return waterSources.Count > 0 || nextToWater;
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
                    if (supplying && supplyingPipes == false)
                    {
                        tile.addWaterPipes(gameObject);
                        //supplyingPipes = true;
                    }
                    else if (!supplying && supplyingPipes == true)
                    {
                        tile.removeWaterPipes(gameObject);
                        //supplyingPipes = false;
                    }
                }
            }
        }
        supplyingPipes = supplying;
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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
            if (waterSources.Count > 0)
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

    /**
     * Fills neighbors with water sources from this Reservoir.  If the neighbors don't have
     * the water sources, this method has the neighbors pass on the water sources to their neighbors
     */
    public void fillNeighbors()
    {
        //Get references to how the water sources are before they are updated to see if they need to update neighbors
        List<GameObject> prevNorthWaterSources = northWaterSources;
        List<GameObject> prevSouthWaterSources = southWaterSources;
        List<GameObject> prevWestWaterSources = westWaterSources;
        List<GameObject> prevEastWaterSources = eastWaterSources;
        //Updates the current water sources
        int prevWaterSourcesCount = waterSources.Count;
        if (filling)
        {
            setWaterSources();
        }
        if (waterSources.Count > 0 && !supplyingPipes)
        {
            updatePipes(true);
            //supplyingPipes = true;
        }
        else if (prevWaterSourcesCount > 0 && waterSources.Count == 0 && supplyingPipes)
        {
            updatePipes(false);
            //supplyingPipes = false;
        }
        //Water sources could be different now.  If they are, update neighbors
        bool northSourcesEqual = prevNorthWaterSources.All(northWaterSources.Contains) && prevNorthWaterSources.Count == northWaterSources.Count;
        bool southSourcesEqual = prevSouthWaterSources.All(southWaterSources.Contains) && prevSouthWaterSources.Count == southWaterSources.Count;
        bool westSourcesEqual = prevWestWaterSources.All(westWaterSources.Contains) && prevWestWaterSources.Count == westWaterSources.Count;
        bool eastSourcesEqual = prevEastWaterSources.All(eastWaterSources.Contains) && prevEastWaterSources.Count == eastWaterSources.Count;
        if (initialCreation || !filling || !northSourcesEqual || !southSourcesEqual || !westSourcesEqual || !eastSourcesEqual)
        {
            initialCreation = false;
            if (northConnection != null)
            {
                if (northConnection.tag == "Road")
                {
                    if (northConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        northConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                    }
                }
                else if (northConnection.GetComponent<Reservoir>() != null)
                {
                    northConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources(filling);
                }
                else if (northConnection.GetComponent<Aqueduct>() != null)
                {
                    northConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                }
            }
            if (southConnection != null)
            {
                if (southConnection.tag == "Road")
                {
                    if (southConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        southConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                    }
                }
                else if (southConnection.GetComponent<Reservoir>() != null)
                {
                    southConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources(filling);
                }
                else if (southConnection.GetComponent<Aqueduct>() != null)
                {
                    southConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                }
            }
            if (westConnection != null)
            {
                if (westConnection.tag == "Road")
                {
                    if (westConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        westConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                    }
                }
                else if (westConnection.GetComponent<Reservoir>() != null)
                {
                    westConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources(filling);
                }
                else if (westConnection.GetComponent<Aqueduct>() != null)
                {
                    westConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                }
            }
            if (eastConnection != null)
            {
                if (eastConnection.tag == "Road")
                {
                    if (eastConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                    {
                        eastConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                    }
                }
                else if (eastConnection.GetComponent<Reservoir>() != null)
                {
                    eastConnection.GetComponent<Reservoir>().setTimeToUpdateWaterSources(filling);
                }
                else if (eastConnection.GetComponent<Aqueduct>() != null)
                {
                    eastConnection.GetComponent<Aqueduct>().setTimeToUpdateWaterSources(filling);
                }
            }
        }
        updateConnections();
        filling = true;
    }

    /**
     * Sets the water sources this object gets from neighbors
     */
    public void setWaterSources()
    {
        northWaterSources = new List<GameObject>();
        southWaterSources = new List<GameObject>();
        westWaterSources = new List<GameObject>();
        eastWaterSources = new List<GameObject>();
        //Sets the water source lists (the 4 directions and the overall)
        if (northConnection != null)
        {
            if (northConnection.tag == "Road")
            {
                if (northConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    northWaterSources = northConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (northConnection.GetComponent<Reservoir>() != null)
            {
                northWaterSources = northConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (northConnection.GetComponent<Aqueduct>() != null)
            {
                northWaterSources = northConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        if (southConnection != null)
        {
            if (southConnection.tag == "Road")
            {
                if (southConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    southWaterSources = southConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (southConnection.GetComponent<Reservoir>() != null)
            {
                southWaterSources = southConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (southConnection.GetComponent<Aqueduct>() != null)
            {
                southWaterSources = southConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        if (westConnection != null)
        {
            if (westConnection.tag == "Road")
            {
                if (westConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    westWaterSources = westConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (westConnection.GetComponent<Reservoir>() != null)
            {
                westWaterSources = westConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (westConnection.GetComponent<Aqueduct>() != null)
            {
                westWaterSources = westConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        if (eastConnection != null)
        {
            if (eastConnection.tag == "Road")
            {
                if (eastConnection.GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    eastWaterSources = eastConnection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getWaterSources();
                }
            }
            else if (eastConnection.GetComponent<Reservoir>() != null)
            {
                eastWaterSources = eastConnection.GetComponent<Reservoir>().getWaterSources();
            }
            else if (eastConnection.GetComponent<Aqueduct>() != null)
            {
                eastWaterSources = eastConnection.GetComponent<Aqueduct>().getWaterSources();
            }
        }
        //If updating water sources of a reservoir next to water, make sure it retains itself
        if (waterSources.Contains(gameObject))
        {
            waterSources = new List<GameObject>();
            waterSources.Add(gameObject);
        }
        else
        {
            waterSources = new List<GameObject>();
        }
        foreach (GameObject waterSource in northWaterSources)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
        foreach (GameObject waterSource in southWaterSources)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
        foreach (GameObject waterSource in westWaterSources)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
        foreach (GameObject waterSource in eastWaterSources)
        {
            if (!waterSources.Contains(waterSource))
            {
                waterSources.Add(waterSource);
            }
        }
    }

    /**
     * Updates the structures with correct water amount and appearance following a clear
     * @param connectedWaterStructures the structures to update
     */
    public void updateConnectedStructuresAfterClear(List<GameObject> connectedWaterStructures)
    {
        //Update everything in the list to remove any water sources they no longer have connection to (can check water sources from this object and make a list of what needs to be removed)
        List<GameObject> waterSourcesToRemove = new List<GameObject>();
        foreach (GameObject waterSource in waterSources)
        {
            if (!connectedWaterStructures.Contains(waterSource))
            {
                waterSourcesToRemove.Add(waterSource);
            }
        }
        //TODO?: This might need a coroutine
        for (int i = 0; i < connectedWaterStructures.Count; i++)
        {
            if (connectedWaterStructures[i] != null)
            {
                if (connectedWaterStructures[i].tag == "Road" && connectedWaterStructures[i].GetComponent<RoadInformation>().getAqueduct() != null)
                {
                    foreach (GameObject waterSourceToRemove in waterSourcesToRemove)
                    {
                        connectedWaterStructures[i].GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().removeWaterSource(waterSourceToRemove);
                    }
                }
                else if (connectedWaterStructures[i].GetComponent<Aqueduct>() != null)
                {
                    foreach (GameObject waterSourceToRemove in waterSourcesToRemove)
                    {
                        connectedWaterStructures[i].GetComponent<Aqueduct>().removeWaterSource(waterSourceToRemove);
                    }
                }
                else if (connectedWaterStructures[i].GetComponent<Reservoir>() != null)
                {
                    foreach (GameObject waterSourceToRemove in waterSourcesToRemove)
                    {
                        connectedWaterStructures[i].GetComponent<Reservoir>().removeWaterSource(waterSourceToRemove);
                    }
                }
            }
        }
        preparingToUpdateWaterSources = false;
        fillNeighbors();
    }

    /**
     * Gets a list of every water structure from this reservoir in order to check if water sources are in the list
     * TODO: this method and the idential one in Aqueduct should be updated to be standalone so there is one method they both use instead of 2 identical ones
     * @param waterStructures is a callback of all the connected water structures to this reservoir
     */
    private IEnumerator checkForWaterSources(System.Action<List<GameObject>> waterStructures)
    {
        List<GameObject> connectedStructures = getConnections();
        List<GameObject> newConnections = connectedStructures;
        bool addingStructures = true;
        while (addingStructures)
        {
            //Debug.Log("updating");
            addingStructures = false;
            List<GameObject> tempConnections = new List<GameObject>();
            //Go through each new connection and get its connections
            foreach (GameObject connection in newConnections)
            {
                if (connection != null)
                {
                    List<GameObject> connectionsOfConnection = new List<GameObject>();
                    if (connection.tag == "Road")
                    {
                        if (connection.GetComponent<RoadInformation>().getAqueduct() != null)
                        {
                            connectionsOfConnection = connection.GetComponent<RoadInformation>().getAqueduct().GetComponent<Aqueduct>().getConnections();
                        }
                    }
                    else if (connection.GetComponent<Aqueduct>() != null)
                    {
                        connectionsOfConnection = connection.GetComponent<Aqueduct>().getConnections();
                    }
                    else if (connection.GetComponent<Reservoir>() != null)
                    {
                        connectionsOfConnection = connection.GetComponent<Reservoir>().getConnections();
                    }
                    //Checking if the connections of the connection have been added yet and adding them if not
                    for (int i = 0; i < connectionsOfConnection.Count; i++)
                    {
                        if (!connectedStructures.Contains(connectionsOfConnection[i]))
                        {
                            //There are new structures still being added
                            addingStructures = true;
                            tempConnections.Add(connectionsOfConnection[i]);
                        }
                    }
                    yield return new WaitForSeconds(0.001f);
                }
            }
            newConnections = tempConnections;
            //Adding the new structures that were discovered
            foreach (GameObject structure in tempConnections)
            {
                if (structure != null)
                {
                    connectedStructures.Add(structure);
                }
            }
            tempConnections = new List<GameObject>();
        }
        //Debug.Log("finishing");
        waterStructures(connectedStructures);
        yield break;
    }

    /**
     * Sets the time at which the reservoir should update its water sources
     * @param whether the reservoir is going to be filling up after the time delay
     */
    public void setTimeToUpdateWaterSources(bool filling)
    {
        this.filling = filling;
        timeToUpdateWaterSources = Time.time + waterDelay;
        preparingToUpdateWaterSources = true;
    }

    /**
     * Gets the sources filling this reservoir
     * @return the unique water sources this reservoir has
     */
    public List<GameObject> getWaterSources()
    {
        return waterSources;
    }

    /**
     * Removes a water source from all the lists holding references to it
     * @param waterSource the water source to be removed
     */
    public void removeWaterSource(GameObject waterSource)
    {
        northWaterSources.Remove(waterSource);
        southWaterSources.Remove(waterSource);
        westWaterSources.Remove(waterSource);
        eastWaterSources.Remove(waterSource);
        waterSources.Remove(waterSource);
    }

    /**
     * Sets whether the gameObject is next to a water structure that was just cleared
     * @param nextToClearedWaterStructure whether the gameObject is next to a water structure that was just cleared
     */
    public void setNextToClearedWaterStructure(bool nextToClearedWaterStructure)
    {
        this.nextToClearedWaterStructure = nextToClearedWaterStructure;
    }
}
