using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Random = System.Random;

/**
 * Controls overall game flow
 *
 * TODO refactor & annotate
 * TODO add bounding box on window sill in scene & collision trigger script to track trays/food containers
 * TODO add bell in scene on window sill
 * TODO add script to bell to verify orders when pressed
 */
public class GameManager : MonoBehaviour
{
    private readonly Order[] _receivedOrders = new Order[2]; // always 2 orders

    private Random _random;

    private readonly string[] _ingredients =
    {
        ITags.Lettuce, ITags.Tomato, ITags.Patty,
        ITags
            .Cheese
    };

    private void Start()
    {
        _random = new Random();
        StartGame();
    }

    private void StartGame()
    {
        for (int i = 0; i < _receivedOrders.Length; i++)
        {
            // _receivedOrders[i] = GenerateOrder();
            _receivedOrders[i] = GenerateTestOrder();
        }
    }

    private Order GenerateTestOrder()
    {
        return new Order(new[]
            {
                ITags.BottomBun,
                ITags.TopBun
            },
            true, true);
    }

    private Order GenerateOrder()
    {
        var hasDrink = _random.Next(2) == 0;
        var hasFries = _random.Next(2) == 0;
        var numIngredients = _random.Next(1, 5);
        var burgerIngredients = new string[numIngredients + 2];
        burgerIngredients[0] = ITags.BottomBun;
        burgerIngredients[numIngredients + 1] = ITags.TopBun;
        for (var i = 1; i < numIngredients + 1; i++)
        {
            burgerIngredients[i] =
                _ingredients[_random.Next(_ingredients.Length)];
        }

        return new Order(burgerIngredients, hasDrink, hasFries);
    }

    public void VerifyOrder(FoodDetector foodDetector)
    {
        // Check a served order against all received orders
        // if no match is found, show negative feedback 
        for (var i = 0; i < _receivedOrders.Length; i++)
        {
            if (!IsCorrectOrder(foodDetector.BurgerIngredients(),
                    foodDetector.NumOfFries(), foodDetector.NumOfDrinks(),
                    _receivedOrders[i])) continue;
            // Order completed
            // TODO: calculate & increment score by used time 
            // TODO: generate & replace old order with new order 
            // TODO: show confetti, success sound & score increment
            Debug.LogWarningFormat("order {0} completed", i);
            return;
        }

        // Wrong order
        // TODO: decrement score by fixed amount
        // TODO: show explosion, error sound, score decrement & haptic feedback (optional) 
        Debug.LogWarningFormat("wrong order");
    }

    private static bool IsCorrectOrder(
        IReadOnlyCollection<string> burgerIngredients,
        int numFries, int numDrinks, Order order)
    {
        // Check sides are added according to order
        if (order.HasFries ? numFries != 1 : numFries != 0)
            return false;
        if (order.HasDrink ? numDrinks != 1 : numFries != 0)
            return false;
        // Check burger ingredients are assembled according to order
        if (burgerIngredients.Count != order.BurgerIngredients.Length)
            return false;
        return !burgerIngredients.Where((ingredient, i) =>
            ingredient != order.BurgerIngredients[i]).Any();
    }
}