using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
TODO: 
1. fix arrow direction
2. detect steak 
3. 
**/
public class TutorialScript : MonoBehaviour
{
    /*** tutorial sequence: 
    step 0: default, upon entering 
        play welcome audio + recipe audio
         
        play open the fridge audio 
        increment 
    step 1: after opening the fridge 
        check if fridge is open
        play grab a tomato with all your fingers audio
        place the tomato on the cutting board 
        increment 
    step 2: after tomato on the board 
        detect if tomato slice present in the scene
        play the grill stake audio 
        increment 
    step 3: grill steak 
        detect if patty is present 
        play the assemble audio 
        increment 
    step 4: magnetic grab 
    step 5: assemble burger 
        detect if burger is assembled 
        play the serve audio 
        increment
    
    step 6: show locomotion
   
    step 7: go to the real game


**/

    private int statusStep = 0 ; 
    private bool statusChanged = true; 
    private AudioSource audioSource; 
    public float arrowDistance = 0.5f; 

    public GameObject recipe;
    public GameObject assemblyTray;

    private GameObject _pointToObject; 
    public GameObject centerEyeAnchor;
    public GameObject player; 
    public AudioClip correctSound;  
    public AudioClip welcomeSound;  
    public AudioClip recipeSound;  
    public AudioClip openFridgeSound;
    public AudioClip takeTomatoSound;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _pointToObject = recipe;
        // DoorArray = GameObject.FindGameObjectsWithTag("FridgeDoor");
        // if (DoorArray.Length > 0){
        //     door = DoorArray[0];
        // }
    }

    // Update is called once per frame
    public float rotateAngle = 90.0f;
    void Update()
    {
        // the position of the arrow bar 
        transform.position = centerEyeAnchor.transform.position + centerEyeAnchor.transform.forward * arrowDistance;
        // the internal rotation of the arrow bar 
        
        transform.LookAt(_pointToObject.transform.position);
        transform.Rotate(0.0f, rotateAngle, 0.0f, Space.Self);
        
        Debug.Log("\n\n\nbar position is {0} {1}" + transform.position + transform.rotation);
        Debug.Log("current object to look at is {0}" + _pointToObject.name);

        switch (statusStep) {
            case 0: 
                // show the recipe, blink, sound 
                Step0();
                break; 
            case 1: 
                // show the fridge, check if fridge is open 
                Step1();
                break; 
            case 2: 
                // show the tomato, check if tomato is on the board 
                Step2();
                break;
            case 3: 
                // show the grill, check if patty is on the grill 
                Step3();
                break;
            case 4:
                // show the magnetic grab, check if patty is on the grill 
                Step4();
                break;
            default: 
                break; 
        }
        
    }

    // NOTE: the previous step is responsible for playing the instruction of the next step!
    public void Step0 () { 
        audioSource.PlayOneShot(welcomeSound);
        statusStep += 1; // supposed to play open fridge sound 
    }
    
    private GameObject door; 


    public void Step1 () { 
        door = GameObject.Find("FreeRefrigerator_DoorBig");
        Debug.Log("door is " + door);
        if (door != null && door.transform.localRotation.y != 0.0f) {
        _pointToObject = door;
            audioSource.PlayOneShot(openFridgeSound); // supposed to be tomato
            statusStep += 1; 
        }
    }
    private GameObject[] tomatoObjects; 

    public void Step2 (){
        tomatoObjects = GameObject.FindGameObjectsWithTag("tomatoSlice");
        if (tomatoObjects.Length > 0){
        _pointToObject = assemblyTray;
            audioSource.PlayOneShot(takeTomatoSound); // supposed to be tomato
            statusStep += 1; 
        }
    }

    private GameObject[] steakObjects;
    public void Step3 () {
        steakObjects = GameObject.FindGameObjectsWithTag("GrilledSteak");
        if (steakObjects.Length > 0){
         _pointToObject = steakObject[0];
            audioSource.PlayOneShot(correctSound); // supposed to be tomato
            statusStep += 1; 
        }
    } 

    public void Step4 () {

    }

    public void Step5 () {

    }

    IEnumerator Wait(int seconds)
    {
        Debug.Log("Inside wait function " + seconds); 
        yield return new WaitForSeconds(seconds);
    }


}
