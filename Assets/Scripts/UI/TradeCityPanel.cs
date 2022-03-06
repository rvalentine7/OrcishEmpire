using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Allows the user to see trade information for a city and open trade with the city
/// </summary>
public class TradeCityPanel : WorldPopupPanel
{
    public GameObject openTradeButton;
    public Text openTradeButtonText;
    public Text importOne;
    public Text importTwo;
    public Text importThree;
    public Text importOneCost;
    public Text importTwoCost;
    public Text importThreeCost;
    public Image importOneIcon;
    public Image importTwoIcon;
    public Image importThreeIcon;
    public Text exportOne;
    public Text exportTwo;
    public Text exportThree;
    public Text exportOneCost;
    public Text exportTwoCost;
    public Text exportThreeCost;
    public Image exportOneIcon;
    public Image exportTwoIcon;
    public Image exportThreeIcon;
    public Sprite beerIcon;
    public Sprite eggIcon;
    public Sprite fishIcon;
    public Sprite furnitureIcon;
    public Sprite hopsIcon;
    public Sprite ironIcon;
    public Sprite lumberIcon;
    public Sprite meatIcon;
    public Sprite ochreIcon;
    public Sprite paintIcon;
    public Sprite weaponIcon;
    public Sprite wheatIcon;
    public GameObject importGoldIconOne;
    public GameObject importGoldIconTwo;
    public GameObject importGoldIconThree;
    public GameObject exportGoldIconOne;
    public GameObject exportGoldIconTwo;
    public GameObject exportGoldIconThree;

    private World myWorld;
    private TradeManager tradeManager;
    private TradeCityObject currentTradeCityObject;

    /// <summary>
    /// Initialization
    /// </summary>
    void Awake()
    {
        myWorld = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>();
        tradeManager = myWorld.getTradeManager();
        currentTradeCityObject = null;
    }

    /// <summary>
    /// Updates the trade city panel with information relating to a city name
    /// </summary>
    /// <param name="worldItemName">The name of the city this panel should represent</param>
    /// <param name="worldItemNameTemplate">The sprite containing the name and title of the city</param>
    /// <param name="worldItemIndicator">Used to indicate which item on the world map is displaying information</param>
    public override void receiveWorldItemInfo(string worldItemName, Sprite worldItemNameTemplate, GameObject worldItemIndicator)
    {
        toggleWorldItemGUI(worldItemIndicator);

        this.worldItemName = worldItemName;
        this.worldItemNameTemplate.sprite = worldItemNameTemplate;

        currentTradeCityObject = tradeManager.getTradeCityObject(worldItemName);
        //Deactivate the trade button if trade is already open at this location
        if (currentTradeCityObject.getTradeRouteOpened() && openTradeButton.activeSelf)
        {
            openTradeButton.SetActive(false);
        }
        //Activate the trade button if trade isn't open and the button is inactive
        if (!currentTradeCityObject.getTradeRouteOpened())
        {
            openTradeButton.SetActive(true);
            openTradeButtonText.text = "" + currentTradeCityObject.tradeRouteCost;
        }

        List<string> imports = currentTradeCityObject.imports;
        if (imports.Count > 0)
        {
            importOne.text = imports[0];
            importOneCost.text = myWorld.getGoodsCost(imports[0]) + "";
            setIcon(importOneIcon, imports[0]);
            importGoldIconOne.SetActive(true);

            if (imports.Count > 1)
            {
                importTwo.text = imports[1];
                importTwoCost.text = myWorld.getGoodsCost(imports[1]) + "";
                setIcon(importTwoIcon, imports[1]);
                importGoldIconTwo.SetActive(true);

                if (imports.Count > 2)
                {
                    importThree.text = imports[2];
                    importThreeCost.text = myWorld.getGoodsCost(imports[2]) + "";
                    setIcon(importThreeIcon, imports[2]);
                    importGoldIconThree.SetActive(true);
                }
            }
        }
        //Hiding text and images if I'm not using all imports
        if (imports.Count < 3)
        {
            importThree.text = "";
            importThreeCost.text = "";
            importThreeIcon.enabled = false;
            importGoldIconThree.SetActive(false);

            if (imports.Count < 2)
            {
                importTwo.text = "";
                importTwoCost.text = "";
                importTwoIcon.enabled = false;
                importGoldIconTwo.SetActive(false);

                if (imports.Count < 1)
                {
                    importOne.text = "";
                    importOneCost.text = "";
                    importOneIcon.enabled = false;
                    importGoldIconOne.SetActive(false);
                }
            }
        }
        
        List<string> exports = currentTradeCityObject.exports;
        if (exports.Count > 0)
        {
            exportOne.text = exports[0];
            exportOneCost.text = myWorld.getGoodsCost(exports[0]) + "";
            setIcon(exportOneIcon, exports[0]);
            exportGoldIconOne.SetActive(true);

            if (exports.Count > 1)
            {
                exportTwo.text = exports[1];
                exportTwoCost.text = myWorld.getGoodsCost(exports[1]) + "";
                setIcon(exportTwoIcon, exports[1]);
                exportGoldIconTwo.SetActive(true);

                if (exports.Count > 2)
                {
                    exportThree.text = exports[2];
                    exportThreeCost.text = myWorld.getGoodsCost(exports[2]) + "";
                    setIcon(exportThreeIcon, exports[2]);
                    exportGoldIconThree.SetActive(true);
                }
            }
        }
        //Hiding text and images if I'm not using all exports
        if (exports.Count < 3)
        {
            exportThree.text = "";
            exportThreeCost.text = "";
            exportThreeIcon.enabled = false;
            exportGoldIconThree.SetActive(false);

            if (exports.Count < 2)
            {
                exportTwo.text = "";
                exportTwoCost.text = "";
                exportTwoIcon.enabled = false;
                exportGoldIconTwo.SetActive(false);

                if (exports.Count < 1)
                {
                    exportOne.text = "";
                    exportOneCost.text = "";
                    exportOneIcon.enabled = false;
                    exportGoldIconOne.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Sets the icon for goods in the trade city panel
    /// </summary>
    /// <param name="imageToUpdate">The image to update</param>
    /// <param name="goodsName">The name the image should represent (meat should look like meat)</param>
    private void setIcon(Image imageToUpdate, string goodsName)
    {
        goodsName = goodsName.Replace(" ", "");

        if (goodsName.Equals(World.MEAT))
        {
            imageToUpdate.sprite = meatIcon;
        }
        else if (goodsName.Equals(World.WHEAT))
        {
            imageToUpdate.sprite = wheatIcon;
        }
        else if (goodsName.Equals(World.EGGS))
        {
            imageToUpdate.sprite = eggIcon;
        }
        else if (goodsName.Equals(World.HOPS))
        {
            imageToUpdate.sprite = hopsIcon;
        }
        else if (goodsName.Equals(World.FISH))
        {
            imageToUpdate.sprite = fishIcon;
        }
        else if (goodsName.Equals(World.LUMBER))
        {
            imageToUpdate.sprite = lumberIcon;
        }
        else if (goodsName.Equals(World.FURNITURE))
        {
            imageToUpdate.sprite = furnitureIcon;
        }
        else if (goodsName.Equals(World.IRON))
        {
            imageToUpdate.sprite = ironIcon;
        }
        else if (goodsName.Equals(World.WEAPON))
        {
            imageToUpdate.sprite = weaponIcon;
        }
        else if (goodsName.Equals(World.BEER))
        {
            imageToUpdate.sprite = beerIcon;
        }
        else if (goodsName.Equals(World.OCHRE))
        {
            imageToUpdate.sprite = ochreIcon;
        }
        else if (goodsName.Equals(World.WAR_PAINT))
        {
            imageToUpdate.sprite = paintIcon;
        }
        imageToUpdate.enabled = true;
    }

    /// <summary>
    /// Deactivates the open trade button when the panel is disabled and calls base's OnDisable
    /// </summary>
    protected override void OnDisable()
    {
        openTradeButton.SetActive(false);

        base.OnDisable();
    }

    /// <summary>
    /// Attempts to open a trade route for the player
    /// </summary>
    public void openTradeRoute()
    {
        if (currentTradeCityObject.tradeRouteCost <= myWorld.getCurrency())
        {
            myWorld.updateCurrency(-currentTradeCityObject.tradeRouteCost);
            currentTradeCityObject.setTradeRouteOpened(true);
            tradeManager.addActiveTradeRoute(currentTradeCityObject);
            openTradeButton.SetActive(false);
            foreach (GameObject tradeRouteMarker in currentTradeCityObject.tradeRouteMarkers)
            {
                if (!tradeRouteMarker.activeSelf)
                {
                    tradeRouteMarker.SetActive(true);
                }
            }
        }
    }
}
