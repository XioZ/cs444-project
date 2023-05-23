using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Stacks ingredients player places in the burger box into a burger
 *
 * Any object that is an ingredient can be stacked and
 * player is expected to place the right ingredients in the right order
 * to fulfill the received order
 */
// TODO refactor & annotate with comments 
public class BurgerAssembly : MonoBehaviour
{
    // Prefabs need 1) same tag as _itemInZone 2) BoxCollider 3) rotated & scaled 4) dragged into Inspector
    // "BottomBun", "GrilledSteak", "Cheese", "TomatoSlice", "LettuceSlice", "TopBun"
    public List<GameObject> ingredientPrefabs;

    // Base position for stacking ingredients
    public BoxCollider baseCollider;
    public AudioClip errorSound;

    private AudioSource _audioSource;

    private bool _hasCollider;
    private Collider _collider;
    private Grabbable _colliderGrabbable;
    private bool _hasGrabbable;
    private GameObject _ingredientPrefab;
    private bool _isIngredient;

    private List<BoxCollider> _stackedColliders = new();

    public string[] BurgerIngredients()
    {
        return _stackedColliders.Select(i => i.tag).ToArray();
    }

    public void ClearIngredients()
    {
        _stackedColliders.ForEach(_ => Destroy(_.gameObject));
        _stackedColliders = new List<BoxCollider>();
    }

    private void Start()
    {
        _audioSource = this.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ITags.Tray)) return;

        _hasCollider = true;
        _ingredientPrefab = ingredientPrefabs.Find(prefab
            => prefab.CompareTag(other.tag));
        _isIngredient = _ingredientPrefab != null;
        _collider = other;
        _colliderGrabbable = _collider.GetComponent<Grabbable>();
        _hasGrabbable = _colliderGrabbable != null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (ReferenceEquals(other, _collider))
        {
            _hasCollider = false;
        }
    }

    private void Update()
    {
        Stack();
    }

    private void Stack()
    {
        if (!_hasCollider || !_hasGrabbable ||
            !_colliderGrabbable.is_available()) return;

        if (!_isIngredient)
        {
            _audioSource.PlayOneShot(errorSound);
        }
        else
        {
            // Position this ingredient relative to the last one 
            var prevCollider = _stackedColliders.Any()
                ? _stackedColliders[^1]
                : baseCollider;
            var prevColliderBounds = prevCollider.bounds;
            var position = prevColliderBounds.center +
                           new Vector3(0f, prevColliderBounds.size.y / 2f, 0f);

            Destroy(_collider.gameObject);
            var stackedIngredient = Instantiate(_ingredientPrefab,
                position, _ingredientPrefab.transform.rotation);
            // Make ingredients move together as a burger
            stackedIngredient.transform.SetParent(baseCollider.transform);

            var stackedCollider = stackedIngredient.GetComponent<BoxCollider>();
            _stackedColliders.Add(stackedCollider);
        }

        // Reset for next object
        _hasCollider = false;
    }
}