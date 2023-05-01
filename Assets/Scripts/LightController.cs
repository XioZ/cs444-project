using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : Controller
{
    [Header("Contolled Items")]
    public LightComponent lightcomp;
    public LightSwitch lightSwitch;
    
    // Start is called before the first frame update
    // this is used to add the callback function to the lightswitch
    void Start()
    {
        lightSwitch.on_toggled( // passing the callback function 
            ( switch_state ) => { 
            if ( switch_state ) 
                lightcomp.TurnOn(); 
            else 
                lightcomp.TurnOff();             
            } );
    }
  
}
