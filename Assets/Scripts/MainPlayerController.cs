using System;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerController : MonoBehaviour {

	void OnTriggerEnter ( Collider other ) {

        Debug.LogFormat("OnTriggerEnter() {0} ", this.name );
		// Retreive the object to be collected if it exits
		InteractiveItem interactive_item = other.GetComponent<InteractiveItem>();
		if ( interactive_item == null ) return;

		// Forward the current player to the object to be collected
		interactive_item.interacted_with( this );

	}
}