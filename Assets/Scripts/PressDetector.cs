using UnityEngine;

/**
 * Press the button on collision
 */
public class PressDetector : MonoBehaviour
{
    public Rigidbody push;
    private const string Controller = "ControllerAnchor";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.EndsWith(Controller)) return;

        push.useGravity = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.name.EndsWith(Controller)) return;

        push.useGravity = false;
        push.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
    }
}