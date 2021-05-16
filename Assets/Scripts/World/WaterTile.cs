using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A water tile is like a normal Tile except for the fact that it holds a number that
/// identifies it as belonging to a certain section of water.  Sections of water are
/// sections that a single boat can travel anywhere in.  For example, if there is a
/// giant lake in the map and a low bridge goes right down the middle, there are 2
/// sections of water because a boat cannot travel under a low bridge.  These sections
/// would be section 0 and section 1.
/// </summary>
public class WaterTile : Tile
{
    public int waterSection;

    /// <summary>
    /// Initializes the tile and adds it to the correct water section in world
    /// </summary>
    new void Start()
    {
        base.Start();
        myWorld.addToWaterSection(this);
    }

    /// <summary>
    /// Sets the water section number
    /// </summary>
    /// <param name="num">The new number for the water section</param>
    public void setWaterSectionNum(int num)
    {
        if (waterSection != -1)
        {
            myWorld.removeFromWaterSection(this);
        }
        this.waterSection = num;
        if (num != -1)
        {
            myWorld.addToWaterSection(this);
        }
    }

    /// <summary>
    /// Gets the water section number
    /// </summary>
    /// <returns>The water section number</returns>
    public int getWaterSectionNum()
    {
        return this.waterSection;
    }
}
