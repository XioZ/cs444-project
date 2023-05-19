using System.Collections.Generic;
using UnityEngine;

/**
 * Snaps ingredients in order into a designated zone in burger box
 */
// TODO refactor & annotate with comments 
// TODO: create prefabs with 1) colliders 2) suitable rotation & scale 3) drag into ingredientPrefabs lists in Editor 
// TODO: play error sound when released object is not an ingredient 
// TODO: use tags than names & extract const strings 
public class BurgerAssembly : MonoBehaviour
{
    private readonly List<GameObject> _progress = new();

    // next ingredient that entered snap zone
    private GameObject _nextIngredient;
    private bool _isStackable;
    private Grabbable _grabbable;

    private AudioSource _audioSource;

    // "BottomBun", "GrilledSteak", "TomatoSlice", "LettuceSlice", "Cheese", "TopBun" prefabs with BoxColliders for stacking ingredients
    public List<GameObject> ingredientPrefabs;

    // Base position for stacking ingredients
    public GameObject burgerBase;
    public AudioClip errorSound;

    private void Start()
    {
        _progress.Add(burgerBase);
        _audioSource.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player can stack any item that is an ingredient
        // and is expected to place the right ones in order
        // to make a burger and fulfill the order
        if (!ingredientPrefabs.Exists(prefab
                => other.gameObject.name + "Prefab" == prefab.name)) return;

        _nextIngredient = other.gameObject;
        _grabbable = _nextIngredient.GetComponent<Grabbable>();
        _isStackable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (ReferenceEquals(other.gameObject, _nextIngredient))
        {
            _isStackable = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Stack();
    }

    private void Stack()
    {
        if (!_grabbable.is_available()) return;
        if (!_isStackable)
        {
            _audioSource.PlayOneShot(errorSound);
            return;
        }

        // Instantiate the ingredient prefab &
        // Position relative to the last stacked ingredient 
        GameObject prefab = ingredientPrefabs.Find(prefab
            => prefab.name == _nextIngredient.name + "Prefab");
        GameObject lastIngredient = _progress[^1];
        BoxCollider boxCollider = lastIngredient.GetComponent<BoxCollider>();
        Vector3 position = boxCollider.bounds.center + new Vector3(0, boxCollider.bounds.size.y / 2, 0);
        Debug.LogWarningFormat("{0} last", lastIngredient.name);
        Debug.LogWarningFormat("{0} center", boxCollider.bounds.center);
        Debug.LogWarningFormat("{0} height/2", boxCollider.bounds.size.y / 2);
        Destroy(_nextIngredient);
        GameObject stackedIngredient = Instantiate(prefab, position, prefab.transform.rotation);
        // Make ingredients move together as a burger
        stackedIngredient.transform.SetParent(burgerBase.transform);

        _progress.Add(stackedIngredient);

        // Reset for next ingredient 
        _isStackable = false;
    }
}