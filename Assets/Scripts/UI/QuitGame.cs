using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the player to quit the game
/// </summary>
public class QuitGame : MonoBehaviour
{
    /// <summary>
    /// Quits the game
    /// </summary>
    public void quitGame()
    {
        Application.Quit();
    }
}
