using UnityEngine;

public class BuildingTooltip : MonoBehaviour
{
    public BuildMode buildGameObject;
    private int buildCost;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        if (buildGameObject != null)
        {
            buildCost = buildGameObject.getBuildingCost();
        }
        else
        {
            buildCost = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
