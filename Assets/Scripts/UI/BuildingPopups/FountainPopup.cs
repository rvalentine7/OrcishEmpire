using UnityEngine.UI;

/// <summary>
/// A popup for a fountain
/// </summary>
public class FountainPopup : Popup {
    public Text status;
	
	/// <summary>
    /// Displays information on whether the fountain is supplying water to nearby locations
    /// </summary>
	new void Update ()
    {
        base.Update();

        Fountain fountainScript = objectOfPopup.GetComponent<Fountain>();
        if (fountainScript.getFilled())
        {
            status.text = "Supplying water to nearby locations";
        }
        else
        {
            status.text = "This fountain needs access to a filled reservoir to supply water to nearby locations";
        }
    }
}
