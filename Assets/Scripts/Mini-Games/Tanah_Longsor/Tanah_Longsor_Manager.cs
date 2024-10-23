using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Tanah_Longsor_Manager : MonoBehaviour
{
    [Header("Game Parameter")]
    public Tools_Manager tools;
    public Slider timeSlider;
    public TextMeshProUGUI victimToRescueText;
    public float gameLength; //in seconds
    private float currTime;

    [Header("Spawn Parameter")]
    public BoxCollider spawnCollider;
    public List<GameObject> victimPrefabs;
    public int amountOfVictimsToSpawn; // will spawn 1 after the previus victim is rescued
    private Victim victim;
    private int amountOfVictim = 0;
    private int victimRescued = 0;

    /*[Header("Earth Stability Meter")]
    public float maxStability;
    public float currStability;*/
    

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (currTime >= gameLength)
        {
            EndMinigame();
        }
        else
        {
            UpdateTimer();
        }
    }

    public void Initialize()
    {
        //currStability = maxStability;

        timeSlider.maxValue = gameLength;
        victimToRescueText.text = "0 / " + amountOfVictimsToSpawn;

        currTime = 0f;


        tools = GetComponent<Tools_Manager>();
        SpawnVictim();
    }

    private void SpawnVictim()
    {
        int rand = UnityEngine.Random.Range(0, victimPrefabs.Count);

        GameObject newVictim = Instantiate(victimPrefabs[rand], GetRandomPosition(), Quaternion.identity);

        victim = newVictim.GetComponent<Victim>();

        tools.SetVictim(victim);
    }

    public void CurrentVictimRescued()
    {
        victimRescued += 1;

        victimToRescueText.text = victimRescued + " / " + amountOfVictimsToSpawn;

        if (amountOfVictim < amountOfVictimsToSpawn)
        {
            SpawnVictim();
        }
        else
        {
            EndMinigame();
        }
    }

    private Vector3 GetRandomPosition()
    {
        Bounds bounds = spawnCollider.bounds;
        float offsetX = UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x);
        float offsetY = UnityEngine.Random.Range(-bounds.extents.y, bounds.extents.y);

        return new Vector3(bounds.center.x + offsetX, bounds.center.y + offsetY , 3);
    }

    private void UpdateTimer()
    {
        currTime += Time.deltaTime;

        timeSlider.value = currTime;
    }

    public void EndMinigame()
    {
        Cursor.visible = true;
    }
}
