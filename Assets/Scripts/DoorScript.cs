using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Set these in the Unity Inspector
    public float openAngle = 80f; // The amount to rotate when open

    private Vector3 closedRotation; // The original rotation of the door
    private Vector3 openRotation; // The target rotation when open
    public Renderer doorRenderer;
    public Material highlightMaterial;
    private Material originalMaterial;
    public AudioClip doorOpenClip;
    private bool doorOpen = false;
    private bool is_hand_closed_previous_frame = false;
    private AudioSource audioSource;
    private GameObject hapticModule;

    void Start()
    {
        // Store the original and target rotations
        audioSource = GetComponent<AudioSource>();
        closedRotation = transform.eulerAngles;
        openRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + openAngle, transform.eulerAngles.z);
        originalMaterial = doorRenderer.material;
        UnityEngine.Debug.Log("Inside start door ");
        hapticModule = GameObject.Find("HapticModule");
        if (hapticModule == null)
        {
            Debug.Log("hapticModule is null");
        }
        
    }

    public void OpenDoor(float pullDistance)
    {
        
        Debug.Log("Inside OpenDoor() ");
        // _rController.GetComponent<HapticFeedback>().TriggerHapticFeedback();
        // _lController.GetComponent<HapticFeedback>().TriggerHapticFeedback();
        
        // it's possible to know which hand is opening the door, but the tag needs to be more precise than "playerHand"
        hapticModule.GetComponent<HapticFeedback>().RightShortVibration();
        hapticModule.GetComponent<HapticFeedback>().LeftShortVibration();
        // Convert pull distance to a percentage of the max pull distance
        float pullPercentage = Mathf.Clamp(pullDistance / 1.5f, 0f, 1.5f); // Assuming a max pull distance of 1 meter

        // Set the door's rotation based on the pull percentage
        transform.eulerAngles = Vector3.Lerp(closedRotation, openRotation, pullPercentage);
        audioSource.PlayOneShot(doorOpenClip);
    }

    public void CloseDoor()
    {
        // Rotate back to the closed rotation
        hapticModule.GetComponent<HapticFeedback>().RightShortVibration();
        hapticModule.GetComponent<HapticFeedback>().LeftShortVibration();
        
        transform.eulerAngles = closedRotation;
        audioSource.PlayOneShot(doorOpenClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Inside Door OnTriggerEnter ");
        if (other.gameObject.tag == "PlayerHand")
        {
            doorRenderer.material = highlightMaterial;
        }
    }

    // Change the door's material back when the player's hand exits the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerHand")
        
        {
            doorRenderer.material = originalMaterial;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerHand")
        {
            // Access the VR controller script. Replace VRControllerScript with the actual name of your VR controller script.
            HandController controllerScript = other.GetComponent<HandController>();
            bool hand_closed = controllerScript.is_hand_closed();
            if (hand_closed == is_hand_closed_previous_frame) return;
            is_hand_closed_previous_frame = hand_closed;
            // Check if all buttons are pressed
            if (doorOpen == false && hand_closed)
            {
                // Open the door
                OpenDoor(3f);
                doorOpen = true;// replace 1f with the desired pull distance
            }
            else if (doorOpen == true && hand_closed)
            {
                // Close the door
                CloseDoor();
                doorOpen = false;
            }
        }
    }

}
