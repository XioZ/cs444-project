using UnityEngine;

public class SteakSizzle : MonoBehaviour
{
    public GameObject sizzlingSound; // Assign the SizzlingSound object in the Inspector
    public AudioClip tickingSound; // Assign the TickingSound object in the Inspector
    public GameObject cookedSteakPrefab; // Prefab for the cooked steak
    public AudioClip doneSound; // Sound to play when steak is done
    public GameObject progressBarPrefab; // Prefab for the progress bar

    private AudioSource sizzlingAudioSource;
    private AudioSource tickingAudioSource;
    private AudioSource doneAudioSource;

    private float cookTime = 6f; // Time to cook steak in seconds
    private float cookProgress = 0f; // How much the steak has been cooked
    private bool isCooking = false;

    private GameObject progressBar;

    void Start()
    {
        sizzlingAudioSource = sizzlingSound.GetComponent<AudioSource>();
        tickingAudioSource = gameObject.AddComponent<AudioSource>(); // Add new audio source for ticking sound
        tickingAudioSource.clip = tickingSound; // Set ticking sound as audio clip of tickingAudioSource
        tickingAudioSource.loop = true; // Make ticking sound loop
        tickingAudioSource.playOnAwake = false;

        doneAudioSource = GetComponent<AudioSource>();

        progressBar = Instantiate(progressBarPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);
        progressBar.transform.rotation = Quaternion.Euler(0,0,0);
        progressBar.SetActive(false); // Hide the progress bar initially
    }

    void Update()
    {
        if (isCooking)
        {
            cookProgress += Time.deltaTime;
            progressBar.transform.localScale = new Vector3(0.05f + cookProgress*0.1f / cookTime, 0.05f + cookProgress * 0.1f / cookTime, 1f);
            progressBar.transform.position = transform.position + Vector3.up * 0.2f;
            progressBar.transform.rotation = Quaternion.Euler(0, 0, 0);


            if (cookProgress >= cookTime)
            {
                CookSteak();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(ITags.Pan))
        {
            sizzlingAudioSource.Play();
            tickingAudioSource.Play();
            isCooking = true;
            progressBar.SetActive(true); // Show the progress bar
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(ITags.Pan))
        {
            sizzlingAudioSource.Stop();
            tickingAudioSource.Stop();
            isCooking = false;
            progressBar.SetActive(false); // Hide the progress bar
        }
    }

    void CookSteak()
    {
        isCooking = false;
        sizzlingAudioSource.Stop();
        tickingAudioSource.Stop();

        // Replace the raw steak with the cooked steak
        GameObject cookedSteak = Instantiate(cookedSteakPrefab, transform.position, transform.rotation);
        cookedSteak.tag = "GrilledSteak";
        AudioSource cookedSteakAudioSource = cookedSteak.AddComponent<AudioSource>();
        cookedSteakAudioSource.PlayOneShot(doneSound);
      
        // Hide the progress bar
        progressBar.SetActive(false);
        gameObject.SetActive(false); // Hide the raw steak
    }
}
