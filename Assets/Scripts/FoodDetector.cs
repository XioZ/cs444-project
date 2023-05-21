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
    private int _numFries;
    private int _numDrinks;


    public int NumOfFries()
    {
        return _numFries;
    }

    public int NumOfDrinks()
    {
        return _numDrinks;
    }

    public string[] BurgerIngredients()
    {
        return burgerAssembly.StackedIngredients();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ITags.Fries))
        {
            _numFries++;
        }

        if (other.CompareTag(ITags.Drink))
        {
            _numDrinks++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ITags.Fries))
        {
            _numFries--;
        }

        if (other.CompareTag(ITags.Drink))
        {
            _numDrinks--;
        }
    }
}