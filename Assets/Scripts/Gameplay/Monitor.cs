using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour
{
    public bool ZoomedInOnThisMonitor = false;  // Tracks if the player is zoomed in on this monitor

    public Outline outline;
    // Reference to the Mini_Games script (optional, if needed)
    public Mini_Games miniGames;

    private void Awake()
    {
        // Optionally, get the Mini_Games script if it's attached to the same GameObject
        miniGames = GetComponent<Mini_Games>();
    }
    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    // This function can be called when the player zooms in on this monitor
    public void SetZoomStatus(bool zoomedIn)
    {
        ZoomedInOnThisMonitor = zoomedIn;
        if (miniGames != null)
        {
            miniGames.OnZoomStatusChanged(ZoomedInOnThisMonitor);
        }
    }

}
