using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Follow tutorial at https://www.youtube.com/watch?v=HFNzVMi5MSQ
 *
 * Fire an event when button is pressed
 */
public class ButtonController : MonoBehaviour
{
    public float threshold = 0.1f;

    private bool _isPressing;
    private Vector3 _startPosition;
    private ConfigurableJoint _joint;
    private Rigidbody _rigidbody;

    public UnityEvent onPress;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
        _rigidbody = GetComponent<Rigidbody>();
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
        }
        else if (_isPressing && finalDegreePressed < threshold)
        {
            _isPressing = false;
            onPress.Invoke();
        }
    }
}