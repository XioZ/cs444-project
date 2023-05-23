using UnityEngine;
using System.Collections;


public class MagneticGrab : MonoBehaviour
{
    public HandController handController; // You need a reference to the hand controller
    public Material highlightedMaterial;
    public float highlightDistance = 1.5f;
    public float magneticForce = 10f;

    private Material _defaultMaterial;
    private MeshRenderer _renderer;
    private Rigidbody _rigidbody;
    

    void Start()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        IsHighlighted = false;
        _defaultMaterial = _renderer.material;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HighlightIfPointedAtAndClose();
    }
    public bool IsHighlighted { get; private set; }

    void HighlightIfPointedAtAndClose()
    {
        Ray ray = new Ray(handController.transform.position, handController.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform && hit.distance <= highlightDistance)
        {
            _renderer.material = highlightedMaterial;
            IsHighlighted = true;
            handController.SetHighlight(GetComponent<Grabbable>(), hit.point);
        }
        else if (IsHighlighted)
        {
            _renderer.material = _defaultMaterial;
            IsHighlighted = false;
            handController.SetHighlight(null);
        }
    }

    public void ResetHighlight()
    {
        _renderer.material = _defaultMaterial;
        IsHighlighted = false;
        if (HandController.CurrentHighlighted == GetComponent<Grabbable>())
        {
           handController.SetHighlight(null);
        }
    }
}
