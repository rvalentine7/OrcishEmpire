using UnityEngine.UI;

/// <summary>
/// Logic for the trade goods collector popup
/// </summary>
public class TradeGoodsCollectorPopup : Popup
{
    public Text status;
    private CollectTradeGoods collectTradeGoods;

    /// <summary>
    /// Updates status information about the tax collector orc
    /// </summary>
    new void Update()
    {
        base.Update();

        if (collectTradeGoods == null)
        {
            collectTradeGoods = objectOfPopup.GetComponent<CollectTradeGoods>();
        }
        else
        {
            if (collectTradeGoods.getHeadingHome())
            {
                status.text = "Returning to the trading post.";
            }
        }
    }
}
