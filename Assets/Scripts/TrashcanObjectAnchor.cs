using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashObjectAnchor : MonoBehaviour
{
    public AudioClip clip; // the sound to play when hit
    private AudioSource audioSource; // the object that can play the sound

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected Grabbable colliding_object;

    void OnTriggerEnter ( Collider other ) {

        Debug.LogWarningFormat("OnTriggerEnter() {0} ", this.name );
        // Retreive the object to be collected if it exits
        // InteractiveItem interative_item = other.GetComponent<InteractiveItem>();
        
        if (other.gameObject.tag == "trashball") {
            Debug.LogWarningFormat("Trash can inside if correct ", other.name );
            audioSource.PlayOneShot(clip);
            colliding_object = other.gameObject.GetComponent<Grabbable>();
            Debug.LogWarningFormat("found object ", other.gameObject);
            colliding_object.stop_moving(this.gameObject); 
        }
    }
}
