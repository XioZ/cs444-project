using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Follows tutorial at https://www.youtube.com/watch?v=HFNzVMi5MSQ
 *
 * TODO use event & listener to execute onPress() callback
 */
public class ButtonController : MonoBehaviour
{
    public float threshold = 0.1f;
    public float deadZone = 0.025f;

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
        var adjustedDegreePressed =
            degreePressed < deadZone ? 0f : degreePressed;
        var finalDegreePressed = Mathf.Clamp(adjustedDegreePressed, 0f, 1f);

        if (!_isPressing && finalDegreePressed > threshold)
        {
            _isPressing = true;
            Debug.LogWarningFormat("pressing");
        }
        else if (_isPressing && finalDegreePressed < threshold)
        {
            _isPressing = false;
            Debug.LogWarningFormat("pressed");
            onPress.Invoke();
        }
    }
}