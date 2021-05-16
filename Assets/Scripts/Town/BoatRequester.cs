using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles implementation required by all buildings that require boats in order to function.
/// </summary>
public interface BoatRequester
{
    Vector2 receiveBoat(GameObject receivingFrom);
}
