using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flood : MonoBehaviour
{
    [Header("Dependencies")]
    public Flood_Spawner spawner;
    public Transform targetTransform;
    private Rigidbody rb;

    [Header("Variables")]
    [SerializeField] private float floodSpeed;
    [SerializeField] private Vector3 targetDir;


    private bool isMoving = true;
    public void Initialize(Transform target, float speed)
    {
        targetDir = (target.position - transform.position).normalized;
        targetDir.z = 0f;
        floodSpeed = speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            MoveToTargetDirection();
        }
    }

    private void MoveToTargetDirection()
    {
        rb.velocity = floodSpeed * Time.deltaTime * targetDir;
    }

    
}
