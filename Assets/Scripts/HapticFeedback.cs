using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedback : MonoBehaviour
{
    private OVRInput.Controller _Lcontroller = OVRInput.Controller.LTouch;
    private OVRInput.Controller _Rcontroller = OVRInput.Controller.RTouch;
    public float DurationShort = 0.2f;
    public float IntensityShort = 0.8f;
    public float DurationLong = 0.5f;
    public float IntensityLong = 0.5f;


    // Start is called before the first frame update
    public void RightShortVibration()
    {
        Debug.Log("!!!!!!!! TriggerHapticFeedback()" ); 
        OVRInput.SetControllerVibration(IntensityShort, IntensityShort, _Rcontroller);
        Invoke("StopHapticFeedback", DurationShort);
    }
    public void LeftShortVibration()
    {
        Debug.Log("!!!!!!!! TriggerHapticFeedback()" ); 
        OVRInput.SetControllerVibration(IntensityShort, IntensityShort, _Lcontroller);
        Invoke("StopHapticFeedback", DurationShort);
    }

    public void RightLongVibration()
    {
        OVRInput.SetControllerVibration(IntensityLong, IntensityLong, _Rcontroller);
        Invoke("StopHapticFeedback", DurationLong);
    }

    public void LeftLongVibration()
    {
        OVRInput.SetControllerVibration(IntensityLong, IntensityLong, _Lcontroller);
        Invoke("StopHapticFeedback", DurationLong);
    }
    
    private void StopHapticFeedback(){
        OVRInput.SetControllerVibration(0, 0, _Rcontroller);
        OVRInput.SetControllerVibration(0, 0, _Lcontroller);
    }

    void Start()
    {
        if (_Lcontroller == null || _Rcontroller == null)
        {
            Debug.LogError("ERROR: No controller found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
