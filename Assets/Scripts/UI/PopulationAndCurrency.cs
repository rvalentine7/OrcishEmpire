using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationAndCurrency : MonoBehaviour {
    public Text populationCount;
    public Text currencyCount;
    
    // Use this for initialization
    void Start () {
        GameObject world = GameObject.Find("WorldInformation");
        World myWorld = world.GetComponent<World>();
        populationCount.text = "" + myWorld.getPopulation();
        currencyCount.text = "" + myWorld.getCurrency();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * Updates the population text to the new population count
     * @param population the new population count
     */
    public void updatePopulationCount(int population)
    {
        populationCount.text = "" + population;
    }

    /**
     * Updates the currency to the new currency count
     * @param currency the new currency count
     */
    public void updateCurrencyCount(int currency)
    {
        currencyCount.text = "" + currency;
    }
}
