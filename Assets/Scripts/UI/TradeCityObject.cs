using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeCityObject : WorldPopupButton
{
    public int tradeRouteCost;
    public List<string> imports;
    public List<string> exports;
    public List<GameObject> tradeRouteMarkers;
    private bool tradeRouteOpened;

    private void Start()
    {
        TradeManager myTradeManager = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>().getTradeManager();
        myTradeManager.addTradeCity(this);
        tradeRouteOpened = false;
    }

    public bool getTradeRouteOpened()
    {
        return tradeRouteOpened;
    }

    public void setTradeRouteOpened(bool opened)
    {
        tradeRouteOpened = opened;
    }

    public string getCityName()
    {
        return this.worldItemName;
    }
}
