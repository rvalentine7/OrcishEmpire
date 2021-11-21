using UnityEngine.UI;

/// <summary>
/// Popup for a delivery orc
/// </summary>
public class DeliveryOrcPopup : Popup
{
    public Text status;
    private Deliver deliver;

    /// <summary>
    /// Updates status information about the delivery orc
    /// </summary>
    new void Update()
    {
        base.Update();

        if (deliver == null)
        {
            deliver = objectOfPopup.GetComponent<Deliver>();
        }
        else
        {
            if (deliver.getHeadingHome())
            {
                status.text = "Returning to place of employment.";
            }
        }
    }
}
