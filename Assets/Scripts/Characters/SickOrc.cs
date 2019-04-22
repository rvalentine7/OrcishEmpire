﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class should keep track of recoveries
 * If an orc is considered recovered by this class, notify its home and its hospital (if it has one)
 */
public class SickOrc
{
    private int wait;
    private GameObject home;
    private GameObject hospital;
    private GameObject workLocation;

    /**
     * Constructor
     * 
     * @param wait how long this inhabitant has been sick
     * @param atHospital whether the inhabitant is at a hospital
     */
    public SickOrc()
    {
        this.wait = 0;
        this.hospital = null;
        this.home = null;
        this.workLocation = null;
    }

    /**
     * Increase how long this inhabitant has spent recovering
     */
    public void increaseWaitTime()
    {
        //If the orc is at a hospital, it will recover faster
        wait += (getHospital() != null ? 2 : 1);
    }

    /**
     * Gets how long the inhabitant has spent recovering
     * @return wait how long the inhabitant has spent recovering
     */
    public int getWaitTime()
    {
        return wait;
    }

    /**
     * Gets whether the inhabitant is at a hospital
     */
    public GameObject getHospital()
    {
        return hospital;
    }

    /**
     * Sets the hospital this orc is currently recovering at
     * @param hospital the hospital the orc is at
     */
    public void setHospital(GameObject hospital)
    {
        this.hospital = hospital;
    }

    /**
     * Removes the hospital this orc is at
     */
    public void removeHospital()
    {
        this.hospital = null;
    }

    /**
     * Keeps track of which work location this orc works at
     * @param workLocation the location this orc works at
     */
    public void setWorkLocation(GameObject workLocation)
    {
        this.workLocation = workLocation;
    }

    /**
     * Gets the work location this orc works at
     */
    public GameObject getWorkLocation()
    {
        return this.workLocation;
    }

    /**
     * Informs the work location this orc works at that it is now healthy
     */
    public void informWorkLocationHealthy()
    {
        //It is possible this orc is unemployed and will not have a work location
        if (workLocation != null)
        {
            Employment workLocationEmployment = workLocation.GetComponent<Employment>();
            workLocationEmployment.updateSickWorkers(-1);
        }
    }
}