using UnityEngine;
using UnityEngine.Serialization;

// TODO refactor & rename
/**
 * Snaps an object into place when released inside the snap zone
 * Pre-requisites:
 * 1) trigger collider 
 */
public class Snappable : MonoBehaviour
{
    public GameObject snapObjectPrefab; // same tag as snap object 

    private GameObject _snapObject; // grabbable
    private Grabbable _grabbable; // detects release
    private bool _isSnappable;
    private bool _isSnapped; // snaps only once

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(snapObjectPrefab.tag))
        {
            _snapObject = other.gameObject;
            _grabbable = _snapObject.GetComponent<Grabbable>();
            _isSnappable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(snapObjectPrefab.tag))
        {
            _isSnappable = false;
        }
    }

    private void Update()
    {
        SnapOnce();
    }

    private void SnapOnce()
    {
        // todo play error sound if not tag doesn't match when released ?
        if (!_isSnappable || _isSnapped || !_grabbable.is_available()) return;

        Destroy(_snapObject);
        Transform cachedTransform = transform;
        Instantiate(snapObjectPrefab, cachedTransform.position, cachedTransform.rotation, cachedTransform);

        _isSnapped = true;
    }
}