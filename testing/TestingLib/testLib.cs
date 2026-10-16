using System.Globalization;
namespace TestingLib;

public readonly struct Currency : IEquatable<Currency>
{
    public string Code { get; }
    public string Symbol { get; }
    public int DecimalDigits { get; }

    public Currency(string code, string symbol, int decimalDigits)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
            throw new ArgumentException("Code must be exactly 3 characters", nameof(code));
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("symbol cannot be empty", nameof(symbol));
        if (decimalDigits < 0 || decimalDigits > 4)
            throw new ArgumentOutOfRangeException(nameof(decimalDigits), "Decimal digits must be 0..4");

        Code = code.ToUpperInvariant();
        Symbol = symbol;
        DecimalDigits = decimalDigits;
    }
    public override string ToString()
    {
        return $"{Code} ({Symbol})";
    }
    public static bool operator ==(Currency a, Currency b) => a.Equals(b);
    public static bool operator !=(Currency a, Currency b) => !a.Equals(b);

    public override bool Equals(object? obj)
       => obj is Currency other && Equals(other);
    public bool Equals(Currency other) => other.Code == Code;
    public override int GetHashCode() => Code?.GetHashCode() ?? 0;
}


public readonly struct Money : IComparable<Money>, IEquatable<Money>
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative");
        Currency = currency;
        var rounded = Math.Round(amount, Currency.DecimalDigits, MidpointRounding.ToEven);
        Amount = rounded;
    }

    public static Money operator +(Money a, Money b)
    {
        if (!a.Currency.Equals(b.Currency))
            throw new InvalidOperationException($"Cannot add {a} and {b}: different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (!a.Currency.Equals(b.Currency))
            throw new InvalidOperationException($"Cannot subtract {a} and {b}: different currencies");
        if (a.Amount < b.Amount)
            throw new InvalidOperationException($"Cannot subtract {b} from {a}: result would be negative");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money a, decimal b)
     => new Money(a.Amount * b, a.Currency);

    public static Money operator *(decimal b, Money a)
     => a * b;

    public static bool operator <(Money a, Money b) => a.CompareTo(b) < 0;
    public static bool operator >(Money a, Money b) => a.CompareTo(b) > 0;
    public static bool operator <=(Money a, Money b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Money a, Money b) => a.CompareTo(b) >= 0;

    public static bool operator ==(Money a, Money b) => a.Equals(b);
    public static bool operator !=(Money a, Money b) => !a.Equals(b);

    public bool Equals(Money other)
        => Amount == other.Amount && Currency.Code.Equals(other.Currency.Code);

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public int CompareTo(Money other)
    {
        var byCurrency = string.Compare(Currency.Code, other.Currency.Code, StringComparison.Ordinal);
        if (byCurrency != 0) return byCurrency;
            return Amount.CompareTo(other.Amount);
    }

    public override string ToString()
    {
        string format = "F" + Currency.DecimalDigits;
        return $"{Currency.Symbol}{Amount.ToString(format, CultureInfo.InvariantCulture)}";
    }
}

public class Wallet
{
    private Money[] _balances = new Money[4];
    private int _count;
    public int Count => _count;

    public delegate void BalanceChangedHandler(Money other);
    public event BalanceChangedHandler? BalanceChanged;
    public Currency[] Currencies
    {
        get
        {
            Currency[] curr = new Currency[_count];
            for (int i = 0; i < _count; i++)
                curr[i] = _balances[i].Currency;
            return curr;
        }
    }

    public Money this[Currency currency]
    {
        get { return GetBalance(currency); }
    }

    public Wallet() { }

    private int IndexOf(Currency currency)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_balances[i].Currency.Equals(currency))
                return i;
        }
        return -1;
    }

    public void Add(Money money)
    {
        int tempIdx = IndexOf(money.Currency);
        if (tempIdx == -1)
        {
            if (_count == _balances.Length)
            {
                Money[] newBalances = new Money[_balances.Length * 2];
                Array.Copy(_balances, newBalances, _balances.Length);
                _balances = newBalances;
            }
            _balances[_count] = money;
            BalanceChanged?.Invoke(_balances[_count]);
            _count++;
        }
        else
        {
            _balances[tempIdx] = _balances[tempIdx] + money;
            BalanceChanged?.Invoke(_balances[tempIdx]);
        }
    }

    public void operator +=(Money other)
    {
        Add(other);
    }

    public void Remove(Money money)
    {
        int tempIdx = IndexOf(money.Currency);
        if (tempIdx == -1)
            throw new InsufficientFundsException(new Money(0, money.Currency), money);
        if (_balances[tempIdx] < money)
            throw new InsufficientFundsException(_balances[tempIdx], money);
        _balances[tempIdx] -= money;
        BalanceChanged?.Invoke(_balances[tempIdx]);
    }

    public Money GetBalance(Currency currency)
    {
        int tempIdx = IndexOf(currency);
        if (tempIdx == -1)
            return new Money(0, currency);
        return _balances[tempIdx];
    }
}

public class InsufficientFundsException : Exception
{
    public Money Available { get; }
    public Money Requested { get; }
    public Currency Currency => Requested.Currency;

    public InsufficientFundsException(Money available, Money requested)
        : base($"Insufficient funds: {available} available, {requested} requested")
    {
        Available = available;
        Requested = requested;
    }
}
