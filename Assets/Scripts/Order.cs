using System;
using JetBrains.Annotations;

public class Order
{
    public Order([NotNull] string[] burger, bool hasDrink = false,
        bool hasFries = false)
    {
        BurgerIngredients =
            burger ?? throw new ArgumentNullException(nameof(burger));
        HasDrink = hasDrink;
        HasFries = hasFries;
    }

    public string[] BurgerIngredients { get; }

    public bool HasDrink { get; }

    public bool HasFries { get; }

    public override string ToString()
    {
        return
            $"Burger {BurgerIngredients} HasDrink {HasDrink} HasFries {HasFries}";
    }
}