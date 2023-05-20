using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Random = System.Random;

/**
 * Controls overall game flow
 *
 * TODO add bounding box on window sill in scene & collision trigger script to track trays/food containers
 * TODO add bell in scene on window sill
 * TODO add script to bell to verify orders when pressed
 * TODO 
 */
public class GameManager : MonoBehaviour
{
    private readonly Order[] _receivedOrders = new Order[2]; // always 2 orders
    private readonly List<GameObject> _servedOrders = new();

    private Random _random;

    private readonly string[] _ingredients =
    {
        Ingredients.Lettuce, Ingredients.Tomato, Ingredients.Patty, Ingredients
            .Cheese
    };

    private void Start()
    {
        _random = new Random();
    }

    public void StartGame()
    {
        // generate 2 random orders with sides
        _receivedOrders[0] = GenerateOrder();
        _receivedOrders[1] = GenerateOrder();
    }

    private Order GenerateOrder()
    {
        var hasDrink = _random.Next(2) == 0;
        var hasFries = _random.Next(2) == 0;
        var numIngredients = _random.Next(1, 5);
        var burgerIngredients = new string[numIngredients + 2];
        burgerIngredients[0] = Ingredients.BottomBun;
        burgerIngredients[numIngredients + 1] = Ingredients.TopBun;
        for (var i = 1; i < numIngredients + 1; i++)
        {
            burgerIngredients[i] = _ingredients[_random.Next(_ingredients.Length)];
        }

        return new Order(burgerIngredients, hasDrink, hasFries);
    }

    public void Serve(GameObject container)
    {
        _servedOrders.Add(container);
    }

    public void Return(GameObject container)
    {
        _servedOrders.Remove(container);
    }

    public void VerifyOrders()
    {
        // check each served order against each received order
        // if a match is found, complete the received order & show positive feedback 
        // if no match is found, show negative feedback 
        for (int servedIndex = 0; servedIndex < _servedOrders.Count; servedIndex++)
        {
            bool isServedOrderCorrect = false;
            for (int receivedIndex = 0; receivedIndex < _receivedOrders.Length; receivedIndex++)
            {
                if (IsCorrectOrder(_servedOrders[servedIndex], _receivedOrders[receivedIndex]))
                {
                    // TODO: calculate & increment score by used time 
                    // TODO: generate & replace old order with new order 
                    // TODO: show confetti & score increment
                    isServedOrderCorrect = true;
                    break;
                }
            }

            if (!isServedOrderCorrect)
            {
                // TODO: decrement score - fixed ?
                // TODO: show explosion, audio/haptic feedback & score decrement 
            }
        }
    }

    private bool IsCorrectOrder(GameObject container, Order order)
    {
        if (order.HasDrink && !HasChildWithTag(container.transform, "Drink"))
            return false;
        if (order.HasFries && !HasChildWithTag(container.transform, "Fries"))
            return false;
        // get BurgerAssembly._progress
        Transform burgerBase = FindChildWithTag(container.transform, "BurgerBase")
                               ?? throw new NoNullAllowedException("BurgerBase not found");
        BurgerAssembly burgerAssembly = burgerBase.gameObject.GetComponent<BurgerAssembly>() ??
                                        throw new NoNullAllowedException("BurgerAssembly not found");
        string[] stackedIngredients = burgerAssembly.StackedIngredients();
        if (stackedIngredients.Length != order.BurgerIngredients.Length)
            return false;
        // check each item's tag against order ingredient name one by one
        // till list end (same length)
        for (int i = 0; i < stackedIngredients.Length; i++)
        {
            if (stackedIngredients[i] != order.BurgerIngredients[i])
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasChildWithTag(Transform parent, string expectedTag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(expectedTag))
            {
                return true;
            }
        }

        return false;
    }

    private static Transform FindChildWithTag(Transform parent, string expectedTag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(expectedTag))
            {
                return child;
            }
        }

        return null;
    }
}