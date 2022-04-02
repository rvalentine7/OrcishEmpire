using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingPost : Building
{
    private List<Trade> traders;
    private List<Trade> waitingTraders;
    private bool collectingTradeGoods;
    private float testTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        traders = new List<Trade>();
        waitingTraders = new List<Trade>();
        collectingTradeGoods = false;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: when a trade collector picks up goods from a warehouse, subtract it from the city's gold. if the city runs out of gold, the trade collector can return to the trading post

        if (!collectingTradeGoods && waitingTraders.Count > 0)
        {
            collectingTradeGoods = true;

            testTime = Time.time + 10.0f;//TODO: to be removed once the trade collector is in (collectingTradeGoods will suffice)
        }
        if (collectingTradeGoods && Time.time > testTime)//TODO: remove the time and add the contents of this if statement to a method the trade collector calls when it returns from collecting
        {
            collectingTradeGoods = false;

            traders[0].setFinishedTrading(true);
            traders.Remove(traders[0]);
            waitingTraders.RemoveAt(0);

            foreach (Trade trade in waitingTraders)
            {
                trade.moveUpInWaitLine();
            }
            //waitingTraders = new List<Trade>();
        }
    }

    /// <summary>
    /// Gets the number of traders at or going to this trading post
    /// </summary>
    /// <returns>The number of traders at or going to this trading post</returns>
    public int getNumTraders()
    {
        return traders.Count;
    }

    public void receiveTrader(Trade trade)
    {
        if (!traders.Contains(trade))
        {
            traders.Add(trade);
        }
    }

    public void traderArrived(Trade trade)
    {
        if (!waitingTraders.Contains(trade))
        {
            waitingTraders.Add(trade);
        }
    }

    public int getTraderWaitPosition(Trade trade)
    {
        return waitingTraders.Contains(trade) ? waitingTraders.IndexOf(trade) : waitingTraders.Count;
    }

    private new void OnDestroy()
    {
        foreach (Trade trade in traders)
        {
            trade.receiveTradingPostDestruction();
        }

        base.OnDestroy();
    }
}
