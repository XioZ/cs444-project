using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

/**
 * Controls overall game flow
 *
 * TODO refactor & annotate
 */
public class GameManager : MonoBehaviour
{
    public TextMeshPro scoreText;

    private readonly Order[] _receivedOrders = new Order[2]; // always 2 orders
    private int _numOrdersCompleted;

    private Random _random;

    private readonly string[] _ingredients =
    {
        ITags.GrilledSteak, ITags.LettuceSlice, ITags.TomatoSlice, ITags.Cheese
    };

    private void Start()
    {
        _random = new Random();
        StartGame();
    }

    private void StartGame()
    {
        // GenerateOrders();
        GenerateTestOrders();
    }

    private void GenerateTestOrders()
    {
        _receivedOrders[0] = GenerateOrderEmpty();
        _receivedOrders[1] = GenerateOrderFull();
    }

    private void GenerateOrders()
    {
        for (int i = 0; i < _receivedOrders.Length; i++)
        {
            _receivedOrders[i] = GenerateRandomOrder();
        }
    }

    private Order GenerateOrderEmpty()
    {
        return new Order(new[]
            {
                ITags.BottomBun,
                ITags.TopBun
            },
            false, false);
    }

    private Order GenerateOrderFull()
    {
        return new Order(new[]
            {
                ITags.BottomBun,
                ITags.GrilledSteak,
                ITags.LettuceSlice,
                ITags.TomatoSlice,
                ITags.Cheese,
                ITags.TopBun
            },
            false, true);
    }

    private Order GenerateRandomOrder()
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
        var completedOrder = FindCompletedOrder(foodDetector);
        if (completedOrder != -1)
        {
            // Order completed
            _numOrdersCompleted++;
            // TODO: generate & replace old order with new order 
            // TODO: play success sound
            Debug.LogWarningFormat("order {0} completed", completedOrder);
        }
        else
        {
            // Wrong order
            // TODO: play error sound & haptic feedback (optional) 
            Debug.LogWarningFormat("wrong order");
        }

        // Update UI
        scoreText.text = _numOrdersCompleted + " orders completed";
        // Reset tray for new order preparation
        foodDetector.Clear();
    }

    private int FindCompletedOrder(FoodDetector foodDetector)
    {
        // Check a served order against all received orders
        // if no match is found, show negative feedback 
        for (var i = 0; i < _receivedOrders.Length; i++)
        {
            if (IsCorrectOrder(foodDetector.BurgerIngredients(),
                    foodDetector.NumOfFries(), foodDetector.NumOfDrinks(),
                    _receivedOrders[i]))
            {
                return i;
            }
        }

        return -1;
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