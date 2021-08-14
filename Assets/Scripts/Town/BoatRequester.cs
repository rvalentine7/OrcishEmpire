using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles implementation required by all buildings that require boats in order to function.
/// </summary>
public abstract class BoatRequester : Building
{
    /// <summary>
    /// Gets each unique water section around the boat requester
    /// </summary>
    /// <returns></returns>
    public abstract List<int> getNearbyWaterSections();
    /// <summary>
    /// Whether the boat requester can receive a new boat
    /// </summary>
    /// <returns></returns>
    public abstract bool canReceiveBoat();
    /// <summary>
    /// Gives the boat requester a standard boat
    /// </summary>
    /// <param name="boat">The boat to give this class</param>
    public abstract void receiveBoat(StandardBoat boat);
    /// <summary>
    /// Gets a vector a boat can arrive at given a specific water section or Vector2(-1, -1)
    /// if there is no valid receiving location
    /// </summary>
    /// <param name="waterSection">The water section the boat is in</param>
    /// <returns>The location the boat can be received at</returns>
    public abstract Vector2 receivingLocation(int waterSection);
    /// <summary>
    /// Informs the building the standard boat has arrived
    /// </summary>
    public abstract void standardBoatArrived();
    /// <summary>
    /// Informs the building it will no longer be getting a standard boat
    /// </summary>
    public abstract void cancelStandardBoat();
}
