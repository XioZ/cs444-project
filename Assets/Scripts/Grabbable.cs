using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour
{
    [Header("Grasping Properties")] public float graspingRadius = 0.1f;
    [Header("Throwing Properties")] public float throwingForce = 20f;

    // Store the hand controller this object will be attached to
    private HandController _handController;
    private bool _isAvailable = true;

    //regernation logic
    public bool regenerates = false; // Set this to true for objects that should regenerate
    public float regenerationTime = 5f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private GameObject originalParent;
    private bool isRegenerating = false;

    // Store initial transform parent
    private Transform _initialTransformParent;
    private Rigidbody _rigidbody; // optional
    private bool _hasRigidBody;

    //For doorscript
    private DoorScript doorScript;
    // Store the initial hand position when grabbing
    private Vector3 initialHandPosition;
    // Whether the door is currently being grabbed
    private bool isGrabbing;

    void Start()
    {
        _initialTransformParent = transform.parent;
        _rigidbody = GetComponent<Rigidbody>();
        _hasRigidBody = _rigidbody != null;
        
        doorScript = GetComponent<DoorScript>();
        if (doorScript != null) { doorScript.OpenDoor(1); }

        
        //originalParent = transform.parent.gameObject;
    }

    // todo refactor & clean up: move/initialize expensive calls in Start(); comment/remove logs after done testing 
    public void stop_moving (GameObject trashcan) {
        Vector3 zeroforce = new Vector3 (0,0,0);
        this.GetComponent<Rigidbody>().AddForce(zeroforce); 
        this.GetComponent<Rigidbody>().isKinematic = true;
        Debug.Log("Inside stop moving "); 
        transform.localScale = new Vector3(0.2F, 0.2F, 0.2F);
        transform.SetParent( trashcan.transform );
        Debug.Log("successfully set parent "); 
        transform.localPosition = new Vector3(0, 0.3F, 0);
        Debug.Log("stop moving parent is " + transform.parent.ToString()); 
        Debug.Log("current position is " + transform.position.ToString() );
        Debug.Log("current local position is " + transform.localPosition.ToString() );
    }

    public void throw_to ( Vector3 linearVelocity) {
        // Make sure that the object is not attached to a hand controller
        if ( _handController != null ) return;
        Debug.Log("inside throw_to () " + linearVelocity.ToString() ); 
        // Move the object to the given position
        if (this.GetComponent<Rigidbody>() != null) {
            this.GetComponent<Rigidbody>().isKinematic = false; 
            this.GetComponent<Rigidbody>().AddForce(linearVelocity, ForceMode.Impulse); 

            Debug.Log("thrown already" );
        }
    }
    public bool HasBeenGrabbed { get; private set; }


    public void attach_to(HandController handController)
    {
        HasBeenGrabbed = true;
        if (regenerates && !isRegenerating)
        {
            StartCoroutine(Regenerate());
        }
        // Store the hand controller in memory
        _handController = handController;
        _isAvailable = false;

        // Set Rigidbody to kinematic so object moves with the parent (transform-based motion)
        // NOTE: seemingly has to be done before SetParent() to work
        if (_hasRigidBody)
        {
            _rigidbody.isKinematic = true;
        }
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        // Set the object to be placed in the hand controller referential
        transform.SetParent(handController.transform);
       
    }

    public void detach_from(HandController handController)
    {
        HasBeenGrabbed = false;
        if (regenerates)
        {
            isRegenerating = false;
        }
        // Make sure that the right hand controller ask for the release
        if (_handController != handController) return;

        // Detach the hand controller
        _handController = null;
        _isAvailable = true;

        // Set the object to be placed in the original transform parent
        transform.SetParent(_initialTransformParent);
        // No longer a kinematic Rigidbody after 1st grab
        // i.e. grabbable objects start to have collision effect & physics-based motion
        // after being moved away from initial position in scene
        if (_hasRigidBody)
        {
            _rigidbody.isKinematic = false;
        }

    }
    IEnumerator Regenerate()
    {
        isRegenerating = true;

        // Wait for the specified time
        yield return new WaitForSeconds(regenerationTime);

        // Create a new instance of the object at the original position and rotation
        GameObject newItem = Instantiate(gameObject, originalPosition, originalRotation);//originalParent.transform);


    

        // Get the Grabbable component of the new item
        //Grabbable newGrabbable = newItem.GetComponent<Grabbable>();

        //if (newGrabbable != null)
        //{
            // Enable regeneration on the new item
            //newGrabbable.regenerates = true;
            //newGrabbable.isRegenerating = false;
        //}

        // Disable regeneration on the current item
        regenerates = false;

        isRegenerating = false;
    }



    public bool is_available()
    {
        return _isAvailable;
    }

    public float get_grasping_radius()
    {
        return graspingRadius;
    }

}