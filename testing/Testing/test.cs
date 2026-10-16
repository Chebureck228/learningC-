using Microsoft.VisualBasic;
using TestingLib;
using static System.Console;

class Program
{
    static void Main(string[] args)
    {
        var usd = new Currency("USD", "$", 2);
        var eur = new Currency("EUR", "€", 2);
        var cny = new Currency("CNY", "¥", 2);
        var gbp = new Currency("GBP", "£", 2);
        var rub = new Currency("RUB", "₽", 2);

        var wallet = new Wallet();
        var money = new Money(10, usd);
        var money2 = new Money(5, usd);
        var money3 = new Money(4, cny);
        var money4 = new Money(10, gbp);
        var money5 = new Money(100, rub);
        var money6 = new Money(400, eur);

        var a = new Money(10, usd);
        var b = new Money(5, usd);
        var c = new Money(15, eur);
        var d = new Money(20, eur);

        Throws<InvalidOperationException>("USD + EUR", () => { var _ = a + c; });
        Throws<InvalidOperationException>("USD - EUR", () => { var _ = a - c; });
        Throws<InvalidOperationException>("EUR - USD", () => { var _ = c - b; });        Throws<InvalidOperationException>("USD - EUR", () => { var _ = a - d; });


        wallet.Add(money);
        Check("Count после первого Add", 1, wallet.Count);
        wallet.Add(money2);
        Check("Count после второго Add той же валюты", 1, wallet.Count);
        Throws<InsufficientFundsException>("0 EUR balance - 15 EUR", () => wallet.Remove(c));
        wallet.Add(money3);
        wallet.Add(money4);
        wallet.Add(money5);
        wallet.Add(money6);
        Check("Count с пятью валютами", 5, wallet.Count);
        wallet.Remove(b);
        Check("Баланс USD", new Money(10, usd), wallet[usd]);

        Check("2 * USD",new Money(20, usd), a * 2);
        Check("USD * 2", new Money(20, usd), a * 2);
        Check("10$ > 5$", true, a > b );
        Check("USD 10.005 => 10.01 ToEven", new Money(10.005m, usd), new Money(10.01m, usd));
        Check("USD 10.015 => 10.02 ToEven", new Money(10.015m, usd), new Money(10.02m, usd));
        Throws<ArgumentOutOfRangeException>("USD = -1", () => { var _ = new Money(-1, usd); });
        Throws<ArgumentException>("USD is US", () => { var _ = new Currency("US", "€", 2); });
        Check("USD == USD", new Currency("usd", "$", 2), usd);
    }

    static void Throws<TException>(string name, Action action)
        where TException : Exception
    {
        try
        {
            action();
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine($"FAIL: expected exeption, but it wasn't");
            WriteLine($"{name}\n");
            Console.ResetColor();
        }
        catch (TException ex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine($"Exception {ex.GetType().Name} is PASS");
            WriteLine($"{name}\n");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine($"FAIL: expected {typeof(TException).Name}, got {ex.GetType().Name}");
            WriteLine($"{name}\n");
            Console.ResetColor();
        }
    }

    static void Check<T>(string name, T expected, T actual)
        where T : IEquatable<T>
    {
        if (!expected.Equals(actual))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine($"FAIL: expected {expected}, got {actual}");
            WriteLine($"{name}\n");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine($"PASS: expected {expected}, got {actual}");
            WriteLine($"{name}\n");
            Console.ResetColor();
        }
    }
}
