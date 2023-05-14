using System;
using UnityEngine;
using UnityEngine.Serialization;

// follows tutorial https://www.youtube.com/watch?v=7bevpWbHKe4 
public class SwingingArms : MonoBehaviour
{
    public GameObject centerEyeAnchor;
    public GameObject forwardDirection;
    public GameObject leftHand;
    public GameObject rightHand;
    public float speedup = 70f;

    private float _handSpeed;

    private Vector3 _previousPlayerPosition;
    private Vector3 _previousLeftHandPosition;
    private Vector3 _previousRightHandPosition;

    private Vector3 _currentPlayerPosition;
    private Vector3 _currentLeftHandPosition;
    private Vector3 _currentRightHandPosition;

    private void Start()
    {
        if (centerEyeAnchor == null || forwardDirection == null ||
            leftHand == null || rightHand == null)
        {
            Debug.LogErrorFormat("Required fields cannot be null (check Unity Editor)");
            return;
        }

        // initialize previous positions
        _previousPlayerPosition = transform.position;
        _previousLeftHandPosition = leftHand.transform.position;
        _previousRightHandPosition = rightHand.transform.position;
    }

    private void Update()
    {
        // set forward direction around y-axis based on where camera/player is looking
        float yRotation = centerEyeAnchor.transform.eulerAngles.y;
        forwardDirection.transform.rotation = Quaternion.AngleAxis(yRotation, Vector3.up);

        // update current positions
        _currentPlayerPosition = transform.position;
        _currentLeftHandPosition = leftHand.transform.position;
        _currentRightHandPosition = rightHand.transform.position;

        // compute hand travelled distances neglecting body movement between frames 
        var playerDistance =
            Vector3.Distance(_currentPlayerPosition, _previousPlayerPosition);
        var leftHandDistance =
            Vector3.Distance(_currentLeftHandPosition, _previousLeftHandPosition)
            - playerDistance;
        var rightHandDistance =
            Vector3.Distance(_currentRightHandPosition, _previousRightHandPosition)
            - playerDistance;
        _handSpeed = leftHandDistance + rightHandDistance;

        // update player/camera position
        if (Time.timeSinceLevelLoad > 1f)
        {
            var direction = forwardDirection.transform.forward;
            var distance = _handSpeed * speedup * Time.deltaTime;
            transform.position += direction * distance;

            Debug.LogWarningFormat("direction: {0}\n" +
                                   "hand speed: {1}\n" +
                                   "distance: {2}\n" +
                                   "camera: {3}\n",
                direction, _handSpeed, distance, transform.position);
        }

        // update previous positions 
        _previousPlayerPosition = _currentPlayerPosition;
        _previousLeftHandPosition = _currentLeftHandPosition;
        _previousRightHandPosition = _currentRightHandPosition;
    }
}