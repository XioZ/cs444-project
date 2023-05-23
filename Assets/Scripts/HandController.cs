using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class HandController : MonoBehaviour {

	// Store the hand type to know which button should be pressed
	public enum HandType : int { LeftHand, RightHand };
	[Header( "Hand Properties" )]
	public HandType handType;


	// Store the player controller to forward it to the object
	[Header( "Player Controller" )]
	public MainPlayerController playerController;


	// Store all gameobjects containing an Anchor
	// N.B. This list is static as it is the same list for all hands controller
	// thus there is no need to duplicate it for each instance
	static protected MagneticGrab[] magnetic_anchors_in_the_scene;
    static protected Grabbable[] anchors_in_the_scene;
    public LineRenderer lineRenderer;

    void Start () {
		// Prevent multiple fetch
		if ( anchors_in_the_scene == null ) anchors_in_the_scene = GameObject.FindObjectsOfType<Grabbable>();
        if (magnetic_anchors_in_the_scene == null) magnetic_anchors_in_the_scene = GameObject.FindObjectsOfType<MagneticGrab>();
        lineRenderer = GetComponent<LineRenderer>();
    }


	// This method checks that the hand is closed depending on the hand side
	public bool is_hand_closed () {
		// Case of a left hand
		if ( handType == HandType.LeftHand ) return
			OVRInput.Get( OVRInput.Button.Three )                           // Check that the A button is pressed
			&& OVRInput.Get( OVRInput.Button.Four )                         // Check that the B button is pressed
			&& OVRInput.Get( OVRInput.Axis1D.PrimaryHandTrigger ) > 0.5     // Check that the middle finger is pressing
			&& OVRInput.Get( OVRInput.Axis1D.PrimaryIndexTrigger ) > 0.5;   // Check that the index finger is pressing


		// Case of a right hand
		else return
			OVRInput.Get( OVRInput.Button.One )                             // Check that the A button is pressed
			&& OVRInput.Get( OVRInput.Button.Two )                          // Check that the B button is pressed
			&& OVRInput.Get( OVRInput.Axis1D.SecondaryHandTrigger ) > 0.5   // Check that the middle finger is pressing
			&& OVRInput.Get( OVRInput.Axis1D.SecondaryIndexTrigger ) > 0.5; // Check that the index finger is pressing
	}


	protected Vector3 get_velocity(){ 
		Vector3 throw_velocity = Vector3.zero; 
		if (handType == HandType.LeftHand)
			throw_velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
		else if (handType == HandType.RightHand){
			throw_velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
		}
		return throw_velocity;
	}

	protected Vector3 get_angular_velocity(){
		Vector3 angular_velocity = Vector3.zero;
		if (handType == HandType.LeftHand)
			angular_velocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
		else if (handType == HandType.RightHand){
			angular_velocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
		}
		return angular_velocity;
	}

	// Automatically called at each frame
	void Update () { 
		handle_controller_behavior();
    }

	// Store the previous state of triggers to detect edges
	protected bool is_hand_closed_previous_frame = false;

	// Store the object atached to this hand
	// N.B. This can be extended by using a list to attach several objects at the same time
	protected Grabbable object_grasped = null;



    public static Grabbable CurrentHighlighted { get; private set; }

    public void SetHighlight(Grabbable highlightedObject, Vector3 hitPoint = default)
    {
        if (highlightedObject != null)
        {
            //CurrentHighlighted = highlightedObject;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hitPoint);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }


    /// <summary>
    /// This method handles the linking of object anchors to this hand controller
    /// </summary>
    protected void handle_controller_behavior () {
        anchors_in_the_scene = GameObject.FindObjectsOfType<Grabbable>();
		magnetic_anchors_in_the_scene = GameObject.FindObjectsOfType<MagneticGrab>();
        // Check if there is a change in the grasping state (i.e. an edge) otherwise do nothing
        bool hand_closed = is_hand_closed();
		if ( hand_closed == is_hand_closed_previous_frame ) return;
		is_hand_closed_previous_frame = hand_closed;

		//==============================================//
		// Define the behavior when the hand get closed //
		//==============================================//
		if ( hand_closed ) {
			// Log hand action detection
			Debug.LogFormat( "{0} closed", this.transform.parent.name );

			// Determine which object available is the closest from the left hand
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;

            for (int i = 0; i < magnetic_anchors_in_the_scene.Length; i++)
            {
                if (magnetic_anchors_in_the_scene[i].IsHighlighted)
                {
                    magnetic_anchors_in_the_scene[i].ResetHighlight();
                    //magnetic_anchors_in_the_scene[i].AttachToHand();
                    object_grasped = magnetic_anchors_in_the_scene[i].GetComponent<Grabbable>();
                    if (magnetic_anchors_in_the_scene[i].CompareTag("MainButton"))
					{
                        SceneManager.LoadScene("MainScene");
						break;
                    } else if (magnetic_anchors_in_the_scene[i].CompareTag("TutorialButton"))
					{
                        SceneManager.LoadScene("TutorialScene");
						break;
                    }
                    object_grasped.attach_to(this);
				
                    break;

                }
            }

            // Iterate over objects to determine if we can interact with it
            for ( int i = 0; i < anchors_in_the_scene.Length; i++ ) {
				// Skip object not available
				if ( !anchors_in_the_scene[i].is_available() ) continue;

				// Compute the distance to the object
				oject_distance = Vector3.Distance( this.transform.position, anchors_in_the_scene[i].transform.position );

				// Keep in memory the closest object
				// N.B. We can extend this selection using priorities
				if ( oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius() ) {
					best_object_id = i;
					best_object_distance = oject_distance;
				}
            }
			
                // If the best object is in range grab it
            if ( best_object_id != -1 ) {

				// Store in memory the object grasped
				object_grasped = anchors_in_the_scene[best_object_id];
				// Log the grasp
				Debug.LogFormat( "{0} grasped {1}", this.transform.parent.name, object_grasped.name );

                object_grasped.attach_to(this);

            }
		//==============================================//
		// Define the behavior when the hand get opened //
		//==============================================//
		} else if ( object_grasped != null ) {
			// Log the release
			Debug.LogFormat("{0} released {1}", this.transform.parent.name, object_grasped.name);
            Vector3 linearVelocity = get_velocity();
			Vector3 angular_velocity = get_angular_velocity();

			object_grasped.detach_from(this, linearVelocity, angular_velocity);
			Debug.Log(" hand releasing velocity " + linearVelocity );
			// Vector3 acceleration = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch);
			// Debug.Log(" hand releasing acceleration {0}" + acceleration);
			// object_grasped.throw_to(linearVelocity);

            // Magnetic Stuff 
			MagneticGrab magneticGrab = object_grasped.GetComponent<MagneticGrab>();
			if (magneticGrab != null ) { 
				magneticGrab.ResetHighlight();
				lineRenderer.enabled = false;
				magneticGrab.enabled = false;
                
            } 
			object_grasped = null;
		}
	}
}