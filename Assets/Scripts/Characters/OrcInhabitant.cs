using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class should keep track of recoveries
 * If an orc is considered recovered by this class, notify its home and its hospital (if it has one)
 * TODO: make sure orcs leaving houses/houses being destroyed results in sick values at health buildings and employments updates correctly
 * TODO: make sure the health system is updating values correctly
 */
public class OrcInhabitant
{
    private bool healthy;
    private int sickTime;
    private GameObject home;
    private GameObject hospital;
    private GameObject workLocation;
    private Barber barber;

    /**
     * Constructor
     * 
     * @param wait how long this inhabitant has been sick
     * @param atHospital whether the inhabitant is at a hospital
     */
    public OrcInhabitant(GameObject home)//just pass in the HouseInformation object?
    {
        this.healthy = true;
        this.sickTime = 0;
        this.hospital = null;
        this.home = home;
        this.workLocation = null;
        this.barber = null;
    }

    /**
     * Gets the home this orc belongs to
     * @return home the home this orc belongs to
     */
    public GameObject getHome()
    {
        return this.home;
    }

    /**
     * Increase how long this inhabitant has spent recovering
     */
    public void increaseSickTime()
    {
        //If the orc is at a hospital, it will recover faster
        sickTime += (getHospital() != null ? 2 : 1);
    }

    /**
     * Gets how long the inhabitant has spent recovering
     * @return wait how long the inhabitant has spent recovering
     */
    public int getSickTime()
    {
        return sickTime;
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
        HouseInformation houseInformation = home.GetComponent<HouseInformation>();
        houseInformation.updateNumInhabitantsAtHospital(1);
    }

    /**
     * Removes the hospital this orc is at
     */
    public void removeHospital()
    {
        this.hospital = null;
        HouseInformation houseInformation = home.GetComponent<HouseInformation>();
        houseInformation.updateNumInhabitantsAtHospital(-1);
    }

    /**
     * Sets the barber this orc visits
     * 
     * @param barber the barer this orc visits
     */
    public void setBarber(Barber barber)
    {
        //set in here and set at the barber
        if (barber.getNumAvailableCustomerSpots() > 0)
        {
            this.barber = barber;
            barber.addCustomer(this);
        }
    }

    /**
     * Removes the barber this orc visits
     */
    public void removeBarber()
    {
        //remove in here and in HouseInformation
        if (barber != null)
        {
            this.home.GetComponent<HouseInformation>().removeOrcCoveredByBarber(this);
            barber = null;
        }
    }

    /**
     * Updates information on an orc when it is removed from a house
     */
    public void evictFromHouse()
    {
        //remove barber
        if (barber != null)
        {
            barber.removeCustomer(this);
        }
        if (hospital != null)
        {
            Hospital hospitalClass = hospital.GetComponent<Hospital>();
            hospitalClass.removeSickOrc(this);
        }
        if (workLocation != null && !healthy)
        {
            Employment employment = workLocation.GetComponent<Employment>();
            employment.updateSickWorkers(-1);
        }
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
     * Sets the health status of the orc
     * @param healthy whether the orc is healthy or sick
     */
    public void setHealthy(bool healthy)
    {
        this.healthy = healthy;
        if (this.workLocation != null)
        {
            Employment workLocationEmployment = workLocation.GetComponent<Employment>();
            if (healthy)
            {
                workLocationEmployment.updateSickWorkers(-1);
            }
            else
            {
                workLocationEmployment.updateSickWorkers(1);
            }
        }
    }

    /**
     * Gets whether the orc is healthy or sick
     */
    public bool getHealthy()
    {
        return this.healthy;
    }

    /**
     * Informs the work location this orc works at that it is now healthy
     */
    public void informWorkLocationOfHealth(bool healthy)
    {
        //It is possible this orc is unemployed and will not have a work location
        if (workLocation != null)
        {
            Employment workLocationEmployment = workLocation.GetComponent<Employment>();
            if (healthy)
            {
                workLocationEmployment.updateSickWorkers(1);
            }
            else
            {
                workLocationEmployment.updateSickWorkers(-1);
            }
        }
    }
}
