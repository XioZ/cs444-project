using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Follow tutorial at https://www.youtube.com/watch?v=HFNzVMi5MSQ
 * 
 */
public class ButtonController : MonoBehaviour
{
    public float threshold = 0.5f;

    private bool _isPressing;
    private Vector3 _startPosition;
    private ConfigurableJoint _joint;

    public UnityEvent onPress;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        var degreePressed =
            Vector3.Distance(_startPosition, transform.localPosition) /
            _joint.linearLimit.limit;
        var finalDegreePressed = Mathf.Clamp(degreePressed, 0f, 1f);

        if (!_isPressing && finalDegreePressed > threshold)
        {
            _isPressing = true;
            Debug.LogWarningFormat("{0} pressing", name);
        }
        else if (_isPressing && finalDegreePressed < threshold)
        {
            _isPressing = false;
            Debug.LogWarningFormat("{0} pressed", name);
            onPress.Invoke();
        }
    }
}