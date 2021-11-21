using UnityEngine.UI;

/// <summary>
/// Popup for a tax collector orc
/// </summary>
public class TaxCollectorOrcPopup : Popup
{
    public Text status;
    private CollectTaxes collectTaxes;

    /// <summary>
    /// Updates status information about the tax collector orc
    /// </summary>
    new void Update()
    {
        base.Update();

        if (collectTaxes == null)
        {
            collectTaxes = objectOfPopup.GetComponent<CollectTaxes>();
        }
        else
        {
            if (collectTaxes.getHeadingHome())
            {
                status.text = "Returning to the tax collector building.";
            }
        }
    }
}
