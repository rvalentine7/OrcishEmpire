using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A popup for a WorldObject
/// TODO: have a base class for buildings? If every building uses an activate/deactivate method that looks the same... make the BuildingPopup class
/// </summary>
public abstract class Popup : MonoBehaviour
{
    protected GameObject objectOfPopup;
    protected bool initialClick;
    private AudioSource clickAudio;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        GameObject panel = GameObject.FindWithTag(World.BUILD_PANEL);
        if (panel != null)
        {
            panel.SetActive(false);
        }
        initialClick = true;

        clickAudio = GameObject.Find(World.UI_CANVAS).GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates popup
    /// </summary>
    protected void Update()
    {
        if (initialClick && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            initialClick = false;
        }
        if (Input.GetKey(KeyCode.Escape) || (!initialClick
            && !EventSystem.current.IsPointerOverGameObject()
            && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the WorldObject this popup is for
    /// </summary>
    /// <param name="objectOfPopup">The WorldObject for this popup</param>
    public virtual void setGameObject(GameObject objectOfPopup)
    {
        this.objectOfPopup = objectOfPopup;
    }

    /// <summary>
    /// Destroys the popup
    /// </summary>
    public void destroyObject()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Plays the standard click sound
    /// </summary>
    public void playClickSound()
    {
        clickAudio.Play();
    }
}
