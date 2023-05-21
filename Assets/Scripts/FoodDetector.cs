using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *
 * TODO add tag for Drink
 */
public class FoodDetector : MonoBehaviour
{
    public BurgerAssembly burgerAssembly;

    private List<GameObject> _fries = new();
    private List<GameObject> _drinks = new();

    public void Clear()
    {
        _fries.ForEach(Destroy);
        _fries = new List<GameObject>();

        _drinks.ForEach(Destroy);
        _drinks = new List<GameObject>();

        burgerAssembly.ClearIngredients();
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
        return burgerAssembly.BurgerIngredients();
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
    }
}