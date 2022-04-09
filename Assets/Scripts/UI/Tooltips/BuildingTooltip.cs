using UnityEngine;
using UnityEngine.UI;

public class BuildingTooltip : MonoBehaviour
{
    public BuildMode buildGameObject;
    public Text buildCost;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        if (buildGameObject != null)
        {
            buildCost.text = buildGameObject.getBuildingCost() + "";
        }
        else
        {
            buildCost.text = -1 + "";
        }
    }
}
