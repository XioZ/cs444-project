using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/**
TODO: 
1. adjust the force of cuttables and sounds 
2. add hatpic feedback to cuttable 
3. add haptic feedback to 
3. test haptic feedback

**/
public class TutorialScript : MonoBehaviour
{
    /*** tutorial sequence: 
    step 0: default, upon entering 
        welcome to the game, the red arrow is showing you the direction 
        you can grab by pressing all four fingers 
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
    private AudioSource audioSource; 
    public float arrowDistance = 0.5f; 
    private GameObject _pointToObject;
    public GameObject order;
    public GameObject centerEyeAnchor;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _pointToObject = order;
    }

    // Update is called once per frame
    public float rotateAngle = 90.0f;
    void Update()
    {
        // the position of the arrow bar 
        transform.position = centerEyeAnchor.transform.position + centerEyeAnchor.transform.forward * arrowDistance;        
        transform.LookAt(_pointToObject.transform.position);
        transform.Rotate(0.0f, rotateAngle, 0.0f, Space.Self); // internal rotation adjustement
        
        // Debug.Log("\n\n\nbar position is {0} {1}" + transform.position + transform.rotation);
        // Debug.Log("current object to look at is {0}" + _pointToObject.name);

        switch (statusStep) {
            case 0: 
                // show the recipe, blink, sound 
                Step0Welcome();
                break; 
            case 1: 
                // show the fridge, check if fridge is open 
                Step1Fridge();
                break; 
            case 2: 
                // show the tomato, check if tomato is on the board 
                Step2Cut();
                break;
            case 3: 
                // show the grill, check if patty is on the grill 
                Step3Steak();
                break;
            case 4:
                // show the magnetic grab, check if patty is on the grill 
                // TEMP this will send user to main scene
                Step4Magnetic();
                break;
            case 5: 
                Step5Assembly();
                break; 

            default: 
                break;
        }
        
    }

    public AudioClip Step0Sound; 
    private GameObject door; 
    public GameObject fridge; 
    private bool haveChangedArrow = false; 
    public void Step0Welcome () { 
        door = GameObject.Find("FreeRefrigerator_DoorBig");
        audioSource.PlayOneShot(Step0Sound);
        statusStep += 1; // supposed to play open fridge sound 
        if (!haveChangedArrow){
            haveChangedArrow = true;
            Invoke("helperChangeArrow", 15.0f);
        }
    }
    void helperChangeArrow() {
        _pointToObject = fridge;
    }

    public AudioClip Step1Sound; // cut tomato instruction 
    public GameObject CuttingBoard;
    public void Step1Fridge () { 
        if (door != null && door.transform.localRotation.y != 0.0f) {
            _pointToObject = CuttingBoard;
            audioSource.Stop();
            audioSource.PlayOneShot(Step1Sound); // supposed to be tomato
            statusStep += 1; 
        }
    }

    private GameObject[] tomatoObjects;
    private GameObject[] lettuceSlices;
    public AudioClip Step2Sound; // grill steak instruction
    public GameObject RawSteak; 
 
    public void Step2Cut (){
        tomatoObjects = GameObject.FindGameObjectsWithTag("TomatoSlice");
        lettuceSlices = GameObject.FindGameObjectsWithTag("LettuceSlice");
        if (tomatoObjects.Length > 0 && lettuceSlices.Length > 0){
            _pointToObject = RawSteak;
            audioSource.Stop();
            audioSource.PlayOneShot(Step2Sound); // supposed to be tomato
            statusStep += 1; 
        }
    }

    public AudioClip Step3Sound; // grab bun audio 
    private GameObject[] steakObjects;
    public GameObject BottomBun;
    private bool helperFlag = false; 
    public void Step3Steak () {
        steakObjects = GameObject.FindGameObjectsWithTag("GrilledSteak");
        Debug.Log("Steak length is " + steakObjects.Length); 
        if (steakObjects.Length != 0){ // jumping through
            _pointToObject = BottomBun;
            audioSource.Stop();
            audioSource.PlayOneShot(Step3Sound); // supposed to be tomato
            statusStep += 1; 
        }
        
    }
    
    public AudioClip Step4Sound;  // assemble burger audio 
    private GameObject[] BottomBuns; 
    public GameObject BurgerBox;
    public void Step4Magnetic () {
        BottomBuns = GameObject.FindGameObjectsWithTag("BottomBun");
        if (BottomBuns.Length > 2 ){
            _pointToObject = BurgerBox;
            audioSource.Stop();
            audioSource.PlayOneShot(Step4Sound); 
            statusStep += 1;
        }
    }
    private GameObject[] BurgerBoxes; 
    public GameObject CollectionArea; 
    public AudioClip Step5Sound;  // assemble burger audio
    private string[] ingredients1; 
    private string[] ingredients2;
    public void Step5Assembly() {
        BurgerBoxes = GameObject.FindGameObjectsWithTag("BurgerBox");
        ingredients1 = BurgerBoxes[0].GetComponent<BurgerAssembly>().BurgerIngredients();
        ingredients2 = BurgerBoxes[1].GetComponent<BurgerAssembly>().BurgerIngredients();
        if (ingredients1.Length > 0 || ingredients2.Length >0){
            _pointToObject = CollectionArea; 
            audioSource.Stop();
            audioSource.PlayOneShot(Step5Sound); 
            statusStep += 1;
            Invoke("GoToMainGame", 48.0f); 
        }

    }

    void GoToMainGame(){
        Debug.Log("Go to main game function");
        SceneManager.LoadScene("MainScene");
    }

    IEnumerator Wait(int seconds)
    {
        Debug.Log("Inside wait function " + seconds); 
        yield return new WaitForSeconds(seconds);
    }


}
