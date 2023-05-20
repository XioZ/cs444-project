using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    // step 1 -- show the recipe, blink, sound 
    // step 2 -- show the fridge, check if fridge is open 
    // step 3 -- check if a tomato is chopped 
    // step 4 -- check if a steak is cooked 
    // step 5 -- check if assembled 
    private int statusStep = 0 ; 
    private bool statusChanged = true; 
    private AudioSource audioSource; 

    public AudioClip correctSound;  
    public AudioClip step1Sound;  
    private GameObject[] door; 
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (statusChanged) { 
            statusChanged = false;
            switch (statusStep) {
                case 0: 
                    // show the recipe, blink, sound 
                    Step0();
                    break; 
                case 1: 
                    // show the fridge, check if fridge is open 
                    break; 
                default: 
                    break; 
            }
        }
    }

    public void Step0 () { 
        audioSource.PlayOneShot(correctSound);
        statusStep += 1; 
        statusChanged = true;
    }

    public void Step1 () { 
        door = GameObject.FindGameObjectsWithTag("FridgeDoor");
        if (door.Length > 0){
            audioSource.PlayOneShot(step1Sound);
        }
    }



}
