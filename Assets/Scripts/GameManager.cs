using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

/**
 * Controls overall game flow
 */
public class GameManager : MonoBehaviour
{
    public TextMeshPro countdownText;
    public TextMeshPro revenueText;
    public AudioClip correctOrderSound;
    public AudioClip wrongOrderSound;
    public AudioClip timeUpSound;
    public int duration = 180;
    public GameObject hapticModule;

    private readonly Order[] _receivedOrders = new Order[2]; // always 2 orders
    private bool _hasStarted;
    private bool _hasEnded;
    private float _timeLeft;
    private int _revenue;

    private AudioSource _audioSource;
    private HapticFeedback _hapticFeedback;
    private Random _random;

    private readonly string[] _ingredients =
    {
        ITags.LettuceSlice, ITags.TomatoSlice, ITags.Cheese, ITags.GrilledSteak
    };

    public void VerifyOrder(FoodDetector foodDetector)
    {
        if (!_hasStarted || _hasEnded)
        {
            _audioSource.PlayOneShot(timeUpSound);
            return;
        }

        var completedOrder = FindCompletedOrder(foodDetector);
        if (completedOrder != -1)
        {
            // Order completed
            _revenue += 20;
            // TODO: generate & replace old order with new order 
            _audioSource.PlayOneShot(correctOrderSound);
            Debug.LogWarningFormat("order {0} completed", completedOrder);
        }
        else
        {
            // Wrong order
            // TODO: play error sound & haptic feedback (optional) 
            _audioSource.PlayOneShot(wrongOrderSound);
            _hapticFeedback.LeftLongVibration();
            _hapticFeedback.RightLongVibration();
            Debug.LogWarningFormat("wrong order");
        }

        // Update UI
        revenueText.text = "$" + _revenue;
        // Reset tray for new order preparation
        foodDetector.Clear();
    }

    private void Start()
    {
        _random = new Random();
        _audioSource = this.AddComponent<AudioSource>();
        _hapticFeedback = hapticModule.GetComponent<HapticFeedback>();
    }

    private void Update()
    {
        if (!_hasStarted && !_hasEnded)
        {
            StartGame();
        }
        else if (_hasStarted && !_hasEnded && _timeLeft <= 0f)
        {
            EndGame();
        }
        else if (_hasStarted && !_hasEnded && _timeLeft > 0f)
        {
            UpdateCountdown();
        }
    }

    private void UpdateCountdown()
    {
        if (Time.timeSinceLevelLoad < 2f || _timeLeft <= 0) return;

        _timeLeft -= Time.deltaTime;
        ShowTimeLeft();
    }

    private void ShowTimeLeft()
    {
        var minutes = Mathf.Max(Mathf.FloorToInt(_timeLeft / 60f), 0);
        var seconds = Mathf.Max(Mathf.FloorToInt(_timeLeft % 60f), 0);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }

    private void StartGame()
    {
        // GenerateOrders();
        GenerateTestOrders();

        _timeLeft = duration;
        _hasStarted = true;
        ShowTimeLeft();
    }

    private void EndGame()
    {
        _audioSource.PlayOneShot(timeUpSound);
        _timeLeft = 0f;
        _hasStarted = false;
        _hasEnded = true;
        ShowTimeLeft();
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
                ITags.Cheese,
                ITags.TomatoSlice,
                ITags.LettuceSlice,
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
        Debug.LogWarningFormat(
            "served {0} burger {1} fries {2} drinks vs order {3}",
            burgerIngredients, numFries, numDrinks, order);
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