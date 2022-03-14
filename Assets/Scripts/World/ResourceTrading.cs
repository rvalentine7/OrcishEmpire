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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tradeStatus">How the resource is being traded (import, export, not traded)</param>
    /// <param name="tradeAmount">The max amount a trader can buy/sell</param>
    public ResourceTrading(TradeManager.TradeStatus tradeStatus, int tradeAmount)
    {
        this.tradeStatus = tradeStatus;
        this.tradeAmount = tradeAmount;
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
}
