using UnityEngine.UI;

/// <summary>
/// Popup for a collector orc
/// </summary>
public class CollectorPopup : Popup
{
    public Text status;
    private Collect collect;

    /// <summary>
    /// Updates status information about the collector
    /// </summary>
    new void Update()
    {
        base.Update();

        if (collect == null)
        {
            collect = objectOfPopup.GetComponent<Collect>();
        }
        else
        {
            if (collect.getHeadingHome())
            {
                status.text = "Returning to place of employment.";
            }
        }
    }
}
