using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Snaps ingredients in order into a designated zone in burger box
 */
// TODO refactor & annotate with comments 
// TODO: create prefabs w tags [LettuceSlice] with 1) box colliders 2) suitable rotation & scale 3) drag into ingredientPrefabs lists in Editor 
public class BurgerAssembly : MonoBehaviour
{
    // "BottomBun", "GrilledSteak", "TomatoSlice", "LettuceSlice", "Cheese", "TopBun"
    // prefabs with BoxColliders for stacking ingredients
    public List<GameObject> ingredientPrefabs;

    // Base position for stacking ingredients
    public GameObject burgerBase;
    public AudioClip errorSound;

    private AudioSource _audioSource;

    private bool _isInSnapZone;
    private GameObject _itemInZone;
    private Grabbable _itemGrabbable;
    private bool _hasGrabbable;
    private GameObject _ingredientPrefab;
    private bool _isIngredient;

    private readonly List<GameObject> _progress = new();

    public string[] StackedIngredients()
    {
        return _progress.Select(i => i.tag).ToArray();
    }

    private void Start()
    {
        _audioSource = this.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player can stack any item that is an ingredient
        // and is expected to place the right ones in order
        // to make a burger and fulfill the order

        _isInSnapZone = true;
        _ingredientPrefab = ingredientPrefabs.Find(prefab
            => prefab.CompareTag(other.tag));
        _isIngredient = _ingredientPrefab != null;
        _itemInZone = other.gameObject;
        _itemGrabbable = _itemInZone.GetComponent<Grabbable>();
        _hasGrabbable = _itemGrabbable != null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (ReferenceEquals(other.gameObject, _itemInZone))
        {
            _isInSnapZone = false;
        }
    }

    private void Update()
    {
        Stack();
    }

    private void Stack()
    {
        if (!_isInSnapZone || !_hasGrabbable ||
            !_itemGrabbable.is_available()) return;

        if (!_isIngredient)
        {
            _audioSource.PlayOneShot(errorSound);
        }
        else
        {
            // Instantiate the ingredient prefab &
            // Position relative to the last stacked ingredient 
            GameObject prevIngredient =
                _progress.Any() ? _progress[^1] : burgerBase;
            BoxCollider boxCollider =
                prevIngredient.GetComponent<BoxCollider>();
            Vector3 position = boxCollider.bounds.center +
                               new Vector3(0, boxCollider.bounds.size.y / 2, 0);
            Destroy(_itemInZone);
            GameObject stackedIngredient = Instantiate(_ingredientPrefab,
                position,
                _ingredientPrefab.transform.rotation);
            // Make ingredients move together as a burger
            stackedIngredient.transform.SetParent(burgerBase.transform);

            _progress.Add(stackedIngredient);
        }

        // Reset for next object
        _isInSnapZone = false;
    }
}