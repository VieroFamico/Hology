using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tools_Manager : MonoBehaviour
{
    [Header("Camera")]
    public Camera gameCamera;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin vCamShake;

    [Header("Drill Tools")]
    public Button drillButton;
    public Transform drillRadiusIndicator;
    public ParticleSystem drillParticleSystem;
    public Animator drillAnimator;
    public AudioClip drillAudioClip;
    public AudioSource drillAudioSource;
    public float drillRadius;
    public float drillDuration = 5f; // Time it takes for the drill to complete in seconds
    public float targetZOffsetPosAfterDrill = 0f;

    [Header("Acoustic Detector")]
    public Button acousticDetectorButton;
    public SpriteRenderer acousticDetectorIndicator;
    public float acousticDetector_MaxRadius;
    public float acousticDetector_MidRadius;
    public float acousticDetector_MinRadius;
    public AudioSource acousticAudioSource;
    public AudioClip farAudioClip;
    public AudioClip nearAudioClip;
    public LayerMask victimLayerMask;

    public LayerMask terrainLayerMask;

    private bool isUsingDrill = false;
    private bool isUsingAcoustic = false;

    private bool isDrilling = false;

    private float drillTimer = 0f;
    private Victim currentVictim;
    private Tanah_Longsor_Manager tanahLongsorManager;

    void Start()
    {
        vCamShake = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        acousticDetectorIndicator.gameObject.SetActive(false);
        drillRadiusIndicator.gameObject.SetActive(false);

        // Add listeners to the buttons to switch tools
        drillButton.onClick.AddListener(() => SelectTool("Drill"));
        acousticDetectorButton.onClick.AddListener(() => SelectTool("Acoustic"));

        // Initialize the drill as off
        isUsingDrill = false;
        isUsingAcoustic = false;

        tanahLongsorManager = FindObjectOfType<Tanah_Longsor_Manager>();
    }

    void Update()
    {
        if (isUsingAcoustic)
        {
            HandleAcousticDetector();
        }

        if (isUsingDrill)
        {
            HandleDrilling();
        }
    }

    // Function to switch between tools
    void SelectTool(string toolName)
    {
        if (isDrilling)
        {
            return;
        }

        switch (toolName)
        {
            case "Drill":
                isUsingDrill = true;
                isUsingAcoustic = false;
                acousticAudioSource.Stop(); // Stop acoustic detector sound
                acousticDetectorIndicator.gameObject.SetActive(false);
                drillRadiusIndicator.gameObject.SetActive(true);
                break;
            case "Acoustic":
                isUsingDrill = false;
                isUsingAcoustic = true;
                acousticAudioSource.Play();
                acousticDetectorIndicator.gameObject.SetActive(true);
                drillRadiusIndicator.gameObject.SetActive(false);
                break;
        }
    }

    // Handle the Acoustic Detector functionality
    void HandleAcousticDetector()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = gameCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            // Check if we are over terrain
            if (hit.collider.CompareTag("Terrain"))
            {
                // Display an area around the mouse pointer like a radar
                Vector3 detectPosition = hit.point;

                acousticDetectorIndicator.transform.position = detectPosition;

                // Detect victims within range using a circular radius (like a radar sweep)
                Collider[] possibleVictims = Physics.OverlapSphere(detectPosition, acousticDetector_MaxRadius, victimLayerMask);

                // Play sound and give a visual cue for detection
                foreach (Collider victim in possibleVictims)
                {
                    Victim nearbyVictim = victim.GetComponent<Victim>();
                    if (nearbyVictim != null)
                    {
                        float distanceToVictim = Vector3.Distance(detectPosition, currentVictim.transform.position);

                        if(distanceToVictim > acousticDetector_MidRadius)
                        {

                            if(acousticAudioSource.clip != farAudioClip)
                            {
                                acousticAudioSource.clip = farAudioClip;
                                acousticAudioSource.Play();
                            }
                            else
                            {
                                acousticAudioSource.clip = farAudioClip;
                            }
                            
                        }
                        else if(distanceToVictim <= acousticDetector_MidRadius)
                        {
                            if (acousticAudioSource.clip != nearAudioClip)
                            {
                                acousticAudioSource.clip = nearAudioClip;
                                acousticAudioSource.Play();
                            }
                            else
                            {
                                acousticAudioSource.clip = nearAudioClip;
                            }
                        }
                        // Adjust audio volume based on proximity to the victim
                        float volume = Mathf.Lerp(1f, 0f, distanceToVictim / acousticDetector_MaxRadius);
                        acousticAudioSource.volume = volume;

                        // Optionally, you can visualize proximity with color change (e.g., Red to Green)
                        Color proximityColor = Color.Lerp(Color.red, Color.green, 1 - (distanceToVictim / acousticDetector_MaxRadius));
                        // Update UI or radar visualization using this color
                        acousticDetectorIndicator.color = proximityColor;
                    }
                }
            }
        }
    }

    // Handle the drilling functionality
    void HandleDrilling()
    {
        if (currentVictim == null || isDrilling)
        {
            return; // No victim detected yet
        }

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = gameCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            // Display an area around the mouse pointer like a radar
            Vector3 drillPosition = hit.point;
            drillRadiusIndicator.transform.position = drillPosition;
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(drillPosition);
                StartCoroutine(StartDrilling(drillPosition));
            }
        }

    }

    private IEnumerator StartDrilling(Vector3 drillPosition)
    {
        drillTimer = 0;
        isDrilling = true;
        drillButton.enabled = false;
        acousticDetectorButton.enabled = false;

        drillParticleSystem.Play();
        drillAnimator.SetBool("Drill", true);
        drillAudioSource.clip = drillAudioClip;
        drillAudioSource.Play();

        vCamShake.m_FrequencyGain = 1;

        Victim victim = null;
        float victimCurrZPos = 0;
        


        float distanceToVictim = Vector3.Distance(drillPosition, currentVictim.transform.position);

        if (distanceToVictim < drillRadius)
        {
            victim = currentVictim;
            victimCurrZPos = victim.transform.position.z;
        }

        float targetZPos = victim.transform.position.z + targetZOffsetPosAfterDrill;

        while (drillTimer < drillDuration)
        {
            if (victim)
            {
                victim.gameObject.transform.position = new Vector3(victim.transform.position.x,
                victim.transform.position.y,
                Mathf.Lerp(victimCurrZPos, targetZPos, drillTimer / drillDuration));
            }
            

            drillTimer += Time.deltaTime;
            yield return null;
        }

        if (victim)
        {
            victim.gameObject.transform.position = new Vector3(victim.transform.position.x,
                victim.transform.position.y, targetZPos);
            RescueVictim(currentVictim);
        }
        else
        {

        }

        isDrilling = false;
        drillButton.enabled = true;
        acousticDetectorButton.enabled = true;

        drillParticleSystem.Stop();
        drillAnimator.SetBool("Drill", false);
        drillAudioSource.Stop();

        vCamShake.m_FrequencyGain = 0;
    }

    // Rescue the victim
    void RescueVictim(Victim victim)
    {
        // Logic for rescuing a victim, like despawning the victim and updating score
        Destroy(victim.gameObject);
        currentVictim = null;
        tanahLongsorManager.CurrentVictimRescued(); // Spawn the next victim
    }

    public void SetVictim(Victim victim)
    {
        currentVictim = victim;
    }
}
