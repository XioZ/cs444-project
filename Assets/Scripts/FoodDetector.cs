using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *
 * TODO test food detector works to verify prepared order (one tray at a time)
 */
public class FoodDetector : MonoBehaviour
{
    private List<GameObject> _fries = new();
    private List<GameObject> _drinks = new();
    private BurgerAssembly _burgerAssembly;

    public void Clear()
    {
        _fries.ForEach(Destroy);
        _fries = new List<GameObject>();

        _drinks.ForEach(Destroy);
        _drinks = new List<GameObject>();

        _burgerAssembly.ClearIngredients();
    }

    public int NumOfFries()
    {
        return _fries.Count;
    }

    public int NumOfDrinks()
    {
        return _drinks.Count;
    }

    public string[] BurgerIngredients()
    {
        return _burgerAssembly == null
            ? new string[] { }
            : _burgerAssembly.BurgerIngredients();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ITags.Fries))
        {
            _fries.Add(other.gameObject);
        }
        else if (other.CompareTag(ITags.Drink))
        {
            _drinks.Add(other.gameObject);
        }
        else if (other.CompareTag(ITags.BurgerBox))
        {
            _burgerAssembly = other.GetComponent<BurgerAssembly>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ITags.Fries))
        {
            _fries.Remove(other.gameObject);
        }
        else if (other.CompareTag(ITags.Drink))
        {
            _drinks.Remove(other.gameObject);
        }
        else if (other.CompareTag(ITags.BurgerBox))
        {
            _burgerAssembly = null;
        }
    }
}