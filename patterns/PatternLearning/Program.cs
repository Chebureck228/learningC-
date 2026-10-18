using static System.Console;
using PatternLib;


class Program
{
    static void Main(string[] args)
    {
        Check($"отрицательное число == DescribeIf({-5})", "отрицательное число", DescribeIf(-5));
        Check("ноль == DescribeIf(0)", $"ноль", DescribeIf(0));
        Check($"число 101 == DescribeIf(101)", "число 101", DescribeIf(101));
        Check($"пустая строка == DescribeIf(\"\")", "пустая строка", DescribeIf(""));
        Check($"строка длиной {"hello".Length} == DescribeIf(\"hello\")", 
                $"строка длиной {"hello".Length}", DescribeIf("hello"));
        Check($"дробное 5.4 == DescribeIf(5.4)", "дробное 5,4", DescribeIf(5.4));
        Check($"пусто == DescribeIf(null)", "пусто", DescribeIf(null));

        Check($"отрицательное число == DescribeSwitch(-5)", "отрицательное число", DescribeSwitch(-5));
        Check("ноль == DescribeSwitch(0)", $"ноль", DescribeSwitch(0));
        Check($"число 101 == DescribeSwitch(101)", "число 101", DescribeSwitch(101));
        Check($"пустая строка == DescribeSwitch(\"\")", "пустая строка", DescribeSwitch(""));
        Check($"строка длиной {"hello".Length} == DescribeSwtich(\"hello\")", 
                $"строка длиной {"hello".Length}", DescribeSwitch("hello"));
        Check($"дробное 5.4 == DescribeSwitch(5.4)", "дробное 5,4", DescribeSwitch(5.4));
        Check($"пусто == DescribeSwitch({null})", "пусто", DescribeSwitch(null));

        Check("15 = FizzBuzz", "FizzBuzz", FizzBuzz(15));
        Check("Fizz = FizzBuzz(3)", "Fizz", FizzBuzz(3));
        Check("Buzz = FizzBuzz(5)", "Buzz", FizzBuzz(5));
        Check("7 = FizzBuzz(7)", "7", FizzBuzz(7));
    }

    static void Check<T>(string name, T expected, T got) where T : IEquatable<T>
    {
        if (expected.Equals(got))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(name);
            WriteLine($"PASS expected: {expected}, got: {got}\n");
            Console.ResetColor();
        }
            
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(name);
            WriteLine($"FAIL expected: {expected}, got: {got}\n");
            Console.ResetColor();
        }
    }

    static string DescribeIf(object? o)
    {
        if (o is int i && i < 0) 
            return "отрицательное число";
        else if (o is int j && j == 0)
            return "ноль";
        else if (o is int k)
            return $"число {k}";
        else if (o is string m && m == "")
            return "пустая строка";
        else if (o is string v)
            return $"строка длиной {v.Length}";
        else if (o is double d)
            return $"дробное {d}";
        else if (o is null)
            return "пусто";
        else
            return "неизвестный тип данных";
    }
    
    static string DescribeSwitch(object? o) => o switch
    {
        int i when i < 0 => "отрицательное число",
        int i when i == 0 => "ноль",
        int i => $"число {i}",
        string i when i == "" => "пустая строка",
        string str => $"строка длиной {str.Length}",
        double d => $"дробное {d}",
        null => "пусто",
        _ => "неизвестный тип данных"
    };

    static string FizzBuzz(int n) => (n % 3, n % 5) switch
    {
        (0, 0) => "FizzBuzz",
        (0, _) => "Fizz",
        (_, 0) => "Buzz",
        _ => $"{n}"
    };
}