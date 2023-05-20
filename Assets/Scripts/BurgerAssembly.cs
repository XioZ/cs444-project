using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Stacks ingredients player places in the burger box into a burger
 *
 * Any object that is an ingredient can be stacked and
 * player is expected to place the right ingredients in the right order
 * to fulfill the received order
 */
// TODO refactor & annotate with comments 
// TODO: create prefabs w tags [LettuceSlice] with 1) box colliders 2) suitable rotation & scale 3) drag into ingredientPrefabs lists in Editor 
public class BurgerAssembly : MonoBehaviour
{
    // Prefabs need 1) same tag as _itemInZone 2) BoxCollider 3) rotated & scaled 4) dragged into ingredientPrefabs list
    // "BottomBun", "GrilledSteak", "TomatoSlice", "LettuceSlice", "Cheese", "TopBun"
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
            // Position this ingredient relative to the last one 
            var prevIngredient =
                _progress.Any() ? _progress[^1] : burgerBase;
            var dimension =
                prevIngredient.GetComponent<BoxCollider>().bounds;
            var position = dimension.center +
                           new Vector3(0, dimension.size.y / 2, 0);
            Destroy(_itemInZone);
            var stackedIngredient = Instantiate(_ingredientPrefab,
                position, _ingredientPrefab.transform.rotation);
            // Make ingredients move together as a burger
            stackedIngredient.transform.SetParent(burgerBase.transform);

            _progress.Add(stackedIngredient);
        }

        // Reset for next object
        _isInSnapZone = false;
    }
}