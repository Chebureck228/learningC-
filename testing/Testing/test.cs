using TestingLib;
using static System.Console;

class Program
{
    static void Main(string[] args)
    {
        var usd = new Currency("USD", "$", 2);
        var eur = new Currency("EUR", "€", 2);
        var wallet = new Wallet();
        var money = new Money(10, usd);
        var money2 = new Money(5, usd);

        var a = new Money(10, usd);
        var b = new Money(5, usd);
        var c = new Money(15, eur);

        Check("Сложение двух сумм в одной валюте", new Money(15, usd), a + b);
        try
        {
            Check("Сложение двух сумм в разных валютах", new Money(25, usd), a + c);
        }
        catch (InvalidOperationException ex)
        {
            WriteLine($"PASS: expected exception, got {ex.Message}");
        }
        Check("Вычитание двух сумм в одной валюте", new Money(5, usd), a - b);
        try
        {
            var d = a - c;
            Check("Вычитание двух сумм в разных валютах", new Money(0, usd), d);
        }
        catch (InvalidOperationException ex)
        {
            WriteLine($"PASS: expected exception, got {ex.Message}");
        }

        wallet.Add(money);
        Check("Count после первого Add", 1, wallet.Count);
        wallet.Add(money2);
        Check("Count после второго Add той же валюты", 1, wallet.Count);
        Check("Баланс USD", new Money(15, usd), wallet[usd]);
    }

    static void Check<T>(string name, T expected, T actual)
        where T : IEquatable<T>
    {
        WriteLine($"{name}");
        if (!expected.Equals(actual))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine($"FAIL: expected {expected}, got {actual}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine($"PASS: expected {expected}, got {actual}");
            Console.ResetColor();
        }
    }
}
