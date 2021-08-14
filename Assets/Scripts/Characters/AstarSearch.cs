using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Searches for a route for an agent to take to traverse the game world.
 */
public class AstarSearch {
    
    public AstarSearch()
    {

    }

    /**
     * Determines the path the agent must follow to get from its current location to
     *  where it wants to go.
     * @param start is where the agent is currently at
     * @param goal is where the agent is trying to reach
     * @param network is a multidimensional array of objects in the game world
     * @return the path found by A* to go from the start to the goal
     */
     //change the method type to IEnumerator, have a variable at the start that is System.Action<List<Vector2>> path
     //path will be returned through callback
    public IEnumerator aStar(System.Action<List<Vector2>> finalPath, Vector2 start, GameObject goalObject, GameObject[,] network)
    {
        Vector2 goal = goalObject.transform.position;
        List<Vector2> closedSet = new List<Vector2>();
        List<Vector2> openSet = new List<Vector2>();
        openSet.Add(start);
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();

        Dictionary<Vector2, int> gScore = new Dictionary<Vector2, int>();
        Dictionary<Vector2, int> fScore = new Dictionary<Vector2, int>();
        for (int i = 0; i < network.GetLength(0); i++)
        {
            for (int j = 0; j < network.GetLength(1); j++)
            {
                //can either make this what characters cannot travel over (buildings)
                // or make it what they can (roads)
                if (network[i, j] != null && (!network[i, j].tag.Equals(World.BUILDING) || new Vector2(i, j) == goal
                    || network[i, j] == goalObject))
                {
                    gScore.Add(new Vector2(i, j), int.MaxValue);
                    fScore.Add(new Vector2(i, j), int.MaxValue);
                    cameFrom.Add(new Vector2(i, j), new Vector2(i, j));
                }
                //if (j % 50 == 0)
                //{
                //    yield return new WaitForSeconds(1f);
                //}
            }
            yield return new WaitForSeconds(0.05f);
        }
        //yield return null;
        gScore[start] = 0;
        fScore[start] = distance(start, goal);
        while (openSet.Count != 0)
        {
            Vector2 current = new Vector2();
            int lowestFVal = int.MaxValue;
            foreach (Vector2 point in openSet)
            {
                if (fScore[point] < lowestFVal)
                {
                    lowestFVal = fScore[point];
                    current = point;
                }
                //yield return null;
            }
            //yield return null;
            if (current.Equals(goal) || network[(int)current.x, (int)current.y] == goalObject)
            {
                List<Vector2> path = aStarPath(cameFrom, current, start, goal);
                //return path;
                finalPath(path);
                yield break;
            }
            openSet.Remove(current);
            closedSet.Add(current);

            List<Vector2> neighbors = new List<Vector2>();
            //Check neighboring locations to see if they are viable locations
            // to move to.
            GameObject world = GameObject.Find(World.WORLD_INFORMATION);
            World myWorld = world.GetComponent<World>();
            if (current.x + 1 < myWorld.mapSize && current.y > 0
                && network[(int)current.x + 1, (int)current.y] != null
                && (!network[(int)current.x + 1, (int)current.y].tag.Equals(World.BUILDING) || new Vector2(current.x + 1, current.y).Equals(goal)
                || network[(int)current.x + 1, (int)current.y] == goalObject))
            {
                neighbors.Add(new Vector2(current.x + 1, current.y));
            }
            if (current.x - 1 > 0 && current.y > 0
                && network[(int)current.x - 1, (int)current.y] != null
                && (!network[(int)current.x - 1, (int)current.y].tag.Equals(World.BUILDING) || new Vector2(current.x - 1, current.y).Equals(goal)
                || network[(int)current.x - 1, (int)current.y] == goalObject))
            {
                neighbors.Add(new Vector2(current.x - 1, current.y));
            }
            if (current.y + 1 < myWorld.mapSize && current.x > 0
                && network[(int)current.x, (int)current.y + 1] != null
                && (!network[(int)current.x, (int)current.y + 1].tag.Equals(World.BUILDING) || new Vector2(current.x, current.y + 1).Equals(goal)
                || network[(int)current.x, (int)current.y + 1] == goalObject))
            {
                neighbors.Add(new Vector2(current.x, current.y + 1));
            }
            if (current.y - 1 > 0 && current.x > 0
                && network[(int)current.x, (int)current.y - 1] != null
                && (!network[(int)current.x, (int)current.y - 1].tag.Equals(World.BUILDING) || new Vector2(current.x, current.y - 1).Equals(goal)
                || network[(int)current.x, (int)current.y - 1] == goalObject))
            {
                neighbors.Add(new Vector2(current.x, current.y - 1));
            }
            
            foreach (Vector2 neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }
                //If the neighbor location is a road, it is easier for the character
                // to travel on.
                int tentativeGScore = 0;
                if (network[(int)neighbor.x, (int)neighbor.y].tag.Equals(World.ROAD)
                    || network[(int)neighbor.x, (int)neighbor.y].tag.Equals(World.STAIRS)
                    || network[(int)neighbor.x, (int)neighbor.y].tag.Equals(World.HIGH_BRIDGE)
                    || network[(int)neighbor.x, (int)neighbor.y].tag.Equals(World.LOW_BRIDGE))
                {
                    tentativeGScore = gScore[current] + 1;
                }
                else if (new Vector2(neighbor.x, neighbor.y) == goal
                    || network[(int)neighbor.x, (int)neighbor.y] == goalObject)
                {
                    tentativeGScore = gScore[current];
                }
                else
                {
                    tentativeGScore = gScore[current] + 20;
                }

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + distance(neighbor, goal);
            }
        }
        //return new List<Vector2>();
        yield return new WaitForSeconds(0.02f);
    }

    /**
     * Reconstructs the path created by A*.
     * @param cameFrom is a Dictionary containing the route from the character's position
     *   to the goal in reverse order
     * @param current is the goal A* arrived at
     * @return path is the List containing the path vectors in order
     */
    List<Vector2> aStarPath (Dictionary<Vector2, Vector2> cameFrom, Vector2 current, Vector2 start, Vector2 goal)
    {
        List<Vector2> path = new List<Vector2>();
        //the 0.4f added to the y value is to bump the characters up enough to appear as though they are
        // travelling in the middle of the block rather than the bottom
        path.Add(new Vector2(current.x, current.y + 0.4f));
        while(current != start)
        {
            current = cameFrom[current];
            path.Add(new Vector2(current.x, current.y + 0.4f));
        }
        if (path.Contains(start))
        {
            path.Remove(start);
        }
        path.Reverse();
        return path;
    }

    /**
     * Checks the distance between two points.
     * @param point1 is the first point
     * @param point2 is the second point
     * @return the distance between the two points rounded to an int
     */
    int distance(Vector2 point1, Vector2 point2)
    {
        return Mathf.RoundToInt(Mathf.Sqrt((point2.x - point1.x)
            * (point2.x - point1.x) + (point2.y - point1.y)
            * (point2.y - point1.y)));
    }
}