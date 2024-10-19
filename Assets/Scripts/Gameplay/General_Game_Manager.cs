using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class General_Game_Manager : MonoBehaviour
{
    public static General_Game_Manager instance;

    [Header("Reference")]
    public Day_Manager day_manager;

    [Header("UI References")]
    public List<GameObject> tutorialPanels;

    [Header("Environment Variables")]
    public Light regularLight;
    public Light emergencyLight;

    [Header("Mini-Games to Solve")]
    public List<GameObject> Mini_Games;

    [Header("Day Has Started")]
    public bool dayHasStarted;

    [Header("Difficulty Variables")]
    public float miniGames_TimeLimitDecrease_PerDay = 3f; // in seconds
    public float flood_WaveSpeedIncrease_PerDay = 1f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Initialize();

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {
        dayHasStarted = false;
        day_manager.currDay = 1;
        day_manager.Initialize();
    }
}
