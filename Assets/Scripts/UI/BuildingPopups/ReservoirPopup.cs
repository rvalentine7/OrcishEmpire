using UnityEngine.UI;

/// <summary>
/// Popup for a reservoir
/// </summary>
public class ReservoirPopup : Popup {
    public Text status;
	
	/// <summary>
    /// Informs the player about whether the reservoir is supplying water to nearby fountains
    /// </summary>
	new void Update () {
        base.Update();
        
        Reservoir reservoirScript = objectOfPopup.GetComponent<Reservoir>();
        if (reservoirScript.getFilled())
        {
            status.text = "Supplying water access to nearby fountains.";
        }
        else
        {
            status.text = "This reservoir needs a connection to a filled reservoir in order to supply water access to nearby fountains";
        }
    }
}
