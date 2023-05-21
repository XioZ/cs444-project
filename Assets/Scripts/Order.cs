using System;
using JetBrains.Annotations;

public class Order
{
    public Order([NotNull] string[] burger, bool hasDrink, bool hasFries)
    {
        HasDrink = hasDrink;
        HasFries = hasFries;
        BurgerIngredients = burger ?? throw new ArgumentNullException(nameof(burger));
    }

    public string[] BurgerIngredients { get; }

    public bool HasDrink { get; }

    public bool HasFries { get; }
}