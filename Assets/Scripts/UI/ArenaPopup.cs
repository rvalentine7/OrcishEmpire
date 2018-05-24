using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaPopup : MonoBehaviour {
    public GameObject arena;
    public Text status;
    public Text employeeNum;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
        Employment employment = arena.GetComponent<Employment>();
        employeeNum.text = "" + employment.getNumWorkers() + "/" + employment.getWorkerCap();
        if (employment.getNumWorkers() == 0)
        {
            status.text = "There are no fights going on at the arena right now.";
        }
        else if (employment.getWorkerCap() > employment.getNumWorkers())
        {
            status.text = "With few employees, fights will rarely occur.";
        }
        else
        {
            status.text = "This arena has fights occuring on a regular basis.";
        }
    }

    /**
     * Removes the game object from the game.
     */
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /**
     * Sets the farm object this popup is displaying information on.
     */
    public void setArena(GameObject arena)
    {
        this.arena = arena;
    }
}
