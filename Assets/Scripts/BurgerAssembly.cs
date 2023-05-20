using System.Collections.Generic;
using System.Linq;
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

    private bool _isInSnapZone;
    private GameObject _itemInZone;
    private Grabbable _itemGrabbable;
    private bool _isIngredient;

    private AudioSource _audioSource;

    // "BottomBun", "GrilledSteak", "TomatoSlice", "LettuceSlice", "Cheese", "TopBun" prefabs with BoxColliders for stacking ingredients
    public List<GameObject> ingredientPrefabs;

    // Base position for stacking ingredients
    public GameObject burgerBase;

    public string[] StackedIngredients()
    {
        return _progress.Select(i => i.tag).ToArray();
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player can stack any item that is an ingredient
        // and is expected to place the right ones in order
        // to make a burger and fulfill the order

        _isInSnapZone = true;
        _isIngredient = ingredientPrefabs.Exists(prefab
            => other.gameObject.name == prefab.name);
        _itemInZone = other.gameObject;
        _itemGrabbable = _itemInZone.GetComponent<Grabbable>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (ReferenceEquals(other.gameObject, _itemInZone))
        {
            _isInSnapZone = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Stack();
    }

    private void Stack()
    {
        if (!_isInSnapZone || !_itemGrabbable.is_available()) return;

        if (!_isIngredient)
        {
            _audioSource.Play();
        }
        else
        {
            // Instantiate the ingredient prefab &
            // Position relative to the last stacked ingredient 
            GameObject prefab = ingredientPrefabs.Find(prefab
                => prefab.name == _itemInZone.name);
            GameObject lastIngredient = _progress.Any() ? _progress[^1] : burgerBase;
            BoxCollider boxCollider = lastIngredient.GetComponent<BoxCollider>();
            Vector3 position = boxCollider.bounds.center + new Vector3(0, boxCollider.bounds.size.y / 2, 0);
            Destroy(_itemInZone);
            GameObject stackedIngredient = Instantiate(prefab, position, prefab.transform.rotation);
            // Make ingredients move together as a burger
            stackedIngredient.transform.SetParent(burgerBase.transform);

            _progress.Add(stackedIngredient);
        }

        // Reset for next object
        _isInSnapZone = false;
    }
}