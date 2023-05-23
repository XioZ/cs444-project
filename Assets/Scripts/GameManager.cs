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
    public List<GameObject> displayedOrders;
    public List<Material> orderMaterials;
    public TextMeshPro countdownText;
    public TextMeshPro revenueText;
    public AudioClip correctOrderSound;
    public AudioClip wrongOrderSound;
    public AudioClip timeUpSound;
    public int duration = 180;
    public GameObject hapticModule;

    private Order[] _possibleOrders;
    private readonly Order[] _activeOrders = new Order[2]; // always 2 orders
    private bool _hasStarted;
    private bool _hasEnded;
    private float _timeLeft;
    private int _revenue;

    private AudioSource _audioSource;
    private HapticFeedback _hapticFeedback;
    private Random _random;

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
            _audioSource.PlayOneShot(correctOrderSound);
            GenerateOrder(completedOrder);
            Debug.LogWarningFormat("order {0} completed", completedOrder);
        }
        else
        {
            // Wrong order
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
        _possibleOrders = new[]
        {
            new Order(new[]
            {
                ITags.BottomBun,
                ITags.GrilledSteak,
                ITags.Cheese,
                ITags.TomatoSlice,
                ITags.LettuceSlice,
                ITags.TopBun
            }),
            new Order(new[]
            {
                ITags.BottomBun,
                ITags.GrilledSteak,
                ITags.Cheese,
                ITags.GrilledSteak,
                ITags.Cheese,
                ITags.TopBun
            })
        };
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
        InitializeOrders();
        // GenerateTestOrders();

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
        _activeOrders[0] = GenerateOrderEmpty();
        _activeOrders[1] = GenerateOrderFull();
    }

    private void InitializeOrders()
    {
        for (var i = 0; i < _activeOrders.Length; i++)
        {
            GenerateOrder(i);
        }
    }

    private Order GenerateOrderEmpty()
    {
        return new Order(new[]
        {
            ITags.BottomBun,
            ITags.TopBun
        });
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
        });
    }

    private void GenerateOrder(int forIndex)
    {
        var orderId = _random.Next(_possibleOrders.Length);
        var order = _possibleOrders[orderId];
        _activeOrders[forIndex] = order;
        // Update order UI
        displayedOrders[forIndex].GetComponent<Renderer>().material
            = orderMaterials[orderId];
    }

    private int FindCompletedOrder(FoodDetector foodDetector)
    {
        // Check a served order against all received orders
        // if no match is found, show negative feedback 
        for (var i = 0; i < _activeOrders.Length; i++)
        {
            if (IsCorrectOrder(foodDetector.BurgerIngredients(),
                    foodDetector.NumOfFries(), foodDetector.NumOfDrinks(),
                    _activeOrders[i]))
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