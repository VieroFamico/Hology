using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day_Manager : MonoBehaviour
{
    [Header("Reference")]
    public General_Game_Manager general_Game_Manager;
    public List<TextMeshProUGUI> currTimeDisplay;
    public List<TextMeshProUGUI> currDayDisplay;
    public AudioClip startingDayClip;
    public AudioClip endingDayClip;

    /*[Header("Start Day UI References")]
    public GameObject startDayPanel;
    public Button startDayButton;*/

    [Header("End Day UI References")]
    public GameObject endDayPanel;
    public Button endDayButton;

    [Header("Day Variables")]
    public int currDay;
    public float dayLength; // in seconds
    public float hourStart;
    public float minuteStart;

    private float currDayTimer = 0f;
    private int hours;
    private int minutes;



    private bool isStoppingDayTimer = false;
    void Start()
    {
        general_Game_Manager = General_Game_Manager.instance;

        //startDayButton.onClick.AddListener(StartDay);

        endDayButton.onClick.AddListener(EndDay);

        UpdateTimerDisplay(hourStart, minuteStart);

        isStoppingDayTimer = true;

    }

    private void Update()
    {
        if(isStoppingDayTimer)
        {
            return;
        }
        currDayTimer += Time.deltaTime;

        if(currDayTimer >= dayLength)
        {
            EndDay();
        }

        UpdateTimer();

    }

    public void Initialize()
    {
        UpdateDayDisplay();
        UpdateTimer();
        isStoppingDayTimer = true;

        StartDay();

        //ShowStartDayPanel();
    }

    /*public void ShowStartDayPanel()
    {
        startDayPanel.SetActive(true);
        endDayPanel.SetActive(false);
    }*/

    public void ShowEndDayPanel()
    {
        endDayPanel.SetActive(true);
        //startDayPanel.SetActive(false);
    }

    public void StartDay()
    {
        isStoppingDayTimer = false;
        currDayTimer = 0;
        Audio_Manager.instance.PlaySFX(startingDayClip);
        //startDayPanel.SetActive(false);
        general_Game_Manager.dayHasStarted = true;
        UpdateDayDisplay();
    }

    public void EndDay()
    {
        currDayTimer = dayLength;
        isStoppingDayTimer = true;
        Audio_Manager.instance.PlaySFX(endingDayClip);
        endDayPanel.SetActive(false);
        general_Game_Manager.dayHasStarted = false;

    }

    private void UpdateTimer()
    {
        // Calculate total time in minutes (from start to now)
        float totalTimeInMinutes = currDayTimer + (hourStart * 60f + minuteStart);

        

        // Extract hours and minutes from total minutes
        hours = Mathf.FloorToInt(totalTimeInMinutes / 60f) % 24; // Ensure hours are in 24-hour format
        minutes = Mathf.FloorToInt(totalTimeInMinutes % 60f);

        // Update the time display
        UpdateTimerDisplay(hours, minutes);
    }

    private void UpdateTimerDisplay(float hour, float minute)
    {
        // Format the time to always have two digits for hours and minutes

        foreach(TextMeshProUGUI timeDisplay in currTimeDisplay)
        {
            timeDisplay.text = $"{hour:00}:{minute:00}";
        }
    }

    private void UpdateDayDisplay()
    {
        foreach(TextMeshProUGUI dayDisplay in currDayDisplay)
        {
            dayDisplay.text = currDay.ToString();
        }
    }
}
