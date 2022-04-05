using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information on whether a resource is being traded and how much is being traded
/// </summary>
public class ResourceTrading
{
    private TradeManager.TradeStatus tradeStatus;
    private int tradeAmount;
    private int cost;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tradeStatus">How the resource is being traded (import, export, not traded)</param>
    /// <param name="tradeAmount">The max amount a trader can buy/sell</param>
    /// <param name="cost">The cost of the good</param>
    public ResourceTrading(TradeManager.TradeStatus tradeStatus, int tradeAmount, int cost)
    {
        this.tradeStatus = tradeStatus;
        this.tradeAmount = tradeAmount;
        this.cost = cost;
    }

    /// <summary>
    /// Gets the current trade status of the resource (import/export/not traded)
    /// </summary>
    /// <returns>The current trade status of the resource</returns>
    public TradeManager.TradeStatus getTradeStatus()
    {
        return tradeStatus;
    }

    /// <summary>
    /// Sets the trade status of the resource (import/export/not traded)
    /// </summary>
    /// <param name="tradeStatus">The trade status of the resource</param>
    public void setTradeStatus(TradeManager.TradeStatus tradeStatus)
    {
        this.tradeStatus = tradeStatus;
    }

    /// <summary>
    /// Gets the amount of resource any individual trader can trade
    /// </summary>
    /// <returns>The amount of resource any individual trader can trade</returns>
    public int getTradeAmount()
    {
        return tradeAmount;
    }

    /// <summary>
    /// Sets the amount of resource any individual trader can trade
    /// </summary>
    /// <param name="tradeAmount">The amount of resource any individual trader can trade</param>
    public void setTradeAmount(int tradeAmount)
    {
        this.tradeAmount = tradeAmount;
    }

    /// <summary>
    /// Gets the cost of the good
    /// </summary>
    /// <returns>The cost of the good</returns>
    public int getCostPerGood()
    {
        return cost;
    }

    /// <summary>
    /// Creates and returns a deep copy of this ResourceTrading object
    /// </summary>
    /// <returns>A deep copy of this ResourceTrading object</returns>
    public ResourceTrading deepCopy()
    {
        ResourceTrading resourceTrading = new ResourceTrading(TradeManager.TradeStatus.notTrading, 0, 0);
        resourceTrading.tradeStatus = this.tradeStatus;
        resourceTrading.tradeAmount = this.tradeAmount;
        resourceTrading.cost = this.cost;

        return resourceTrading;
    }
}
