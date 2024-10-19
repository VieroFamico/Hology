using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood_Spawner : MonoBehaviour
{
    public GameObject floodPrefab;

    public List<Transform> spawnPosition;

    [Header("Spawn Parameter")]
    [SerializeField] private int amountPerSpawnPos;
    [SerializeField] private float floodSpeed;
    [SerializeField] private float floodSpawnDelay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
