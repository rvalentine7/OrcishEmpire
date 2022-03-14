using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles user inputs for their city's trade resources
/// </summary>
public class ResourceTradeInputs : MonoBehaviour
{
    public string resourceName;
    public InputField maxPerTraderInput;
    public Button tradeStatusButton;
    public Sprite notTradingButton;
    public Sprite importingButton;
    public Sprite exportingButton;
    private TradeManager myTradeManager;
    private TradeManager.TradeStatus tradeStatus;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Start()
    {
        myTradeManager = GameObject.Find(World.WORLD_INFORMATION).GetComponent<World>().getTradeManager();
        tradeStatus = TradeManager.TradeStatus.notTrading;
    }

    /// <summary>
    /// Handles the user clicking the trade status button
    /// </summary>
    public void tradeStatusClick()
    {
        if (tradeStatusButton.image.sprite == notTradingButton)
        {
            tradeStatusButton.image.sprite = importingButton;
            tradeStatus = TradeManager.TradeStatus.importing;
        }
        else if (tradeStatusButton.image.sprite == importingButton)
        {
            tradeStatusButton.image.sprite = exportingButton;
            tradeStatus = TradeManager.TradeStatus.exporting;
        }
        else
        {
            tradeStatusButton.image.sprite = notTradingButton;
            tradeStatus = TradeManager.TradeStatus.notTrading;
        }
        myTradeManager.setTradeStatus(resourceName, tradeStatus);
    }

    /// <summary>
    /// Handles the user inputing values for max resource count per trader
    /// </summary>
    public void inputFieldEndEdit()
    {
        int maxPerTraderNum = int.Parse(maxPerTraderInput.text);
        if (maxPerTraderNum < 0)
        {
            maxPerTraderNum = 0;
            maxPerTraderInput.text = 0 + "";
        }
        myTradeManager.setMaxPerTrader(resourceName, maxPerTraderNum);
    }

    /// <summary>
    /// Gets the current trade status from the UI
    /// </summary>
    /// <returns>The current trade status from the Uis</returns>
    public TradeManager.TradeStatus getTradeStatus()
    {
        return tradeStatus;
    }

    /// <summary>
    /// Gets the max resource count per trader from the UI
    /// </summary>
    /// <returns>The max resource count per trader from the UI</returns>
    public int getMaxPerTrader()
    {
        return int.Parse(maxPerTraderInput.text);
    }
}
