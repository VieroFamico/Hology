using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tanah_Longsor_Manager : MonoBehaviour
{
    [Header("Game Parameter")]
    public float gameLength; //in seconds
    public Tools_Manager tools;

    [Header("Spawn Parameter")]
    public BoxCollider spawnCollider;
    public GameObject victimPrefabs;
    public int amountOfVictimsToSpawn; // will spawn 1 after the previus victim is rescued
    private Victim victim;
    private int amountOfVictim = 0;
    private int victimRescued = 0;

    [Header("Earth Stability Meter")]
    public float maxStability;
    public float currStability;
    

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (currStability <= 0)
        {
            EarthStabilityMeterBelowZero();
        }
    }

    public void Initialize()
    {
        currStability = maxStability;
        tools = GetComponent<Tools_Manager>();
        SpawnVictim();
    }

    private void SpawnVictim()
    {
        GameObject newVictim = Instantiate(victimPrefabs, GetRandomPosition(), Quaternion.identity);

        victim = newVictim.GetComponent<Victim>();

        tools.SetVictim(victim);
    }

    public void CurrentVictimRescued()
    {
        victimRescued += 1;

        if (amountOfVictim < amountOfVictimsToSpawn)
        {
            SpawnVictim();
        }
        else
        {

        }
    }

    private Vector3 GetRandomPosition()
    {
        Bounds bounds = spawnCollider.bounds;
        float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
        float offsetY = 0;
        float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);

        return bounds.center + new Vector3(offsetX, offsetY, offsetZ);
    }

    public void EndMinigame()
    {
        Cursor.visible = true;
    }

    public void EarthStabilityMeterBelowZero()
    {

    }
}
