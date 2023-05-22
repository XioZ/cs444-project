using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Cut With ")] public GameObject cutWith; // object that can cut this object
    [Header("Cut Into")] public GameObject cutInto; // Prefab Only,slices, dices etc. 
    [Header("Trash Prefab")] public GameObject trashPrefab; // Prefab Only,slices, dices etc.
    private int cutCount = 0;
    public AudioClip cutAudioClip; 
    public AudioClip cutTooLightAudioClip;
    public AudioClip cutTooHardAudioClip;
    public AudioClip destroyAudioCilp;
    public int cutLimit = 4;  // cutting how many times will destroy the object 
    private AudioSource audioSource; // the object that can play the sound
    private GameObject hapticModule; 
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null){
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        hapticModule = GameObject.Find("HapticModule");
        if (hapticModule == null){
            Debug.Log("haptic module not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int destroyDelayTime = 3; 

    void OnCollisionEnter(Collision collision ){
        if (collision.gameObject.tag == ITags.Knife){
            Debug.Log("collision enter {0} {1}" + gameObject.name + gameObject.tag);
            if (collision.impulse.magnitude > 0.2 && collision.impulse.y < 0) {
                hapticModule.GetComponent<HapticFeedback>().RightShortVibration();
                hapticModule.GetComponent<HapticFeedback>().LeftShortVibration();
                cutCount += 1; 
                audioSource.PlayOneShot(cutAudioClip);
                Vector3 point3 = new Vector3(0, 2 * cutCount, 0);
                Instantiate(cutInto, transform.position, transform.rotation);
                audioSource.PlayOneShot(cutAudioClip);
                // cutting many times, destroy after 4 cuts
                if (cutCount == cutLimit){
                    audioSource.PlayOneShot(destroyAudioCilp);
                    // instantiate a trash object 
                    Instantiate(trashPrefab, transform.position, transform.rotation);
                    Wait(destroyDelayTime); 
                    // destroy the object
                    gameObject.SetActive(false); 
                }
            } else if (collision.impulse.magnitude > 5 && collision.impulse.y < 0){
                Debug.Log("______________FORCE TOO HARD ");
                audioSource.PlayOneShot(cutTooHardAudioClip);
                hapticModule.GetComponent<HapticFeedback>().RightShortVibration();
                hapticModule.GetComponent<HapticFeedback>().LeftShortVibration();
                Instantiate(trashPrefab, transform.position, transform.rotation);
                gameObject.SetActive(false); 
            } else {
                audioSource.PlayOneShot(cutTooLightAudioClip);
            }
            

            Debug.Log(" ==== Cutting Speed {0} {1}"  + collision.gameObject.tag + collision.relativeVelocity);
            Debug.Log(" #### Impulse {0} {1}"  + collision.impulse);

            
            
        }
    }

    IEnumerator Wait(int seconds)
{
    Debug.Log("Inside wait function " + seconds); 
    yield return new WaitForSeconds(seconds);
}


}
