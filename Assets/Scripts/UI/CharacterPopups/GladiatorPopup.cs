using UnityEngine.UI;

/// <summary>
/// Popup for a gladiator
/// </summary>
public class GladiatorPopup : Popup
{
    public Text status;

    private Gladiator gladiator;

    /// <summary>
    /// Updates status based on where the gladiator is heading
    /// </summary>
    new void Update()
    {
        base.Update();

        if (gladiator == null)
        {
            gladiator = objectOfPopup.GetComponent<Gladiator>();
        }
        else
        {
            if (gladiator.getHeadingHome())
            {
                status.text = "Returning to gladiator pit.";
            }
        }
    }
}
