/*
RangeList - written by Ingo Karstein
(C) 2025 MIT License

Here are ranges... They have start, end and value

e.g. 
    4-21, value "A"
   28-42, value "B" 

They are added over time to the "RangeList" object. If they intersect, the newer range overwrites parts of the older ranges. Overridden ranges are removed. Partially overridden ranges are split into two ranges, one with the new value and one with the old value.

    `var l = new RangeList<int, string>();`

First Insert:
    ```
    l.Add(4, 21, "A");
    l.Add(28, 42, "B");
    l.Add(72, 85, "I");
    ```

         111111111122222222223333333333444444444455555555556666666666777777777788888888889 
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
   |------A---------|      |------B------|                             |-----I------|

Second Insert:
    ```
    l.Add(7, 13, "C");
    l.Add(18, 29, "D");
    l.Add(38, 51, "E");
    l.Add(54, 66, "F");
    ```

         111111111122222222223333333333444444444455555555556666666666777777777788888888889 
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
      |--C--|    |----D------|       |-E----------|  |--F--------|

Third Insert:
    ```
    l.Add(1, 24, "G");
    l.Add(36, 69, "H");
    ```

         111111111122222222223333333333444444444455555555556666666666777777777788888888889 
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
|-----------G----------|           |---------H----------------------|

Result:
|-----------G----------||-D--||-B-||---------H----------------------|  |-----I------|


Method "Output()" shows:
[(1,24),G][(25,30),D][(31,35),B][(36,69),H][(72,85),I]

If you add another range:

    ```
    l.Add(7, 13, "K");
    ```
         111111111122222222223333333333444444444455555555556666666666777777777788888888889
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890

      |--K--|

Result:
|--G-||--K--||--G------||-D--||-B-||---------H----------------------|  |-----I------|

`Output()` shows:
[(1,6),G][(7,13),K][(14,24),G][(25,30),D][(31,35),B][(36,69),H][(72,85),I]


*/


using System.Numerics;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class RangeListKey<T> : IComparable<RangeListKey<T>>
    where T : notnull, IComparable<T>
{
    public T Start { get; set; }
    public T End { get; set; }

    public int CompareTo(RangeListKey<T> other)
    {
        if (other == null)
            return 1;

        var c1 = Start.CompareTo(other.Start);
        if (c1 < 0)
            return -1;
        if (c1 > 0)
            return 1;
        var c2 = End.CompareTo(other.End);
        if (c2 < 0)
            return -1;
        if (c2 > 0)
            return 1;
        return 0;
    }
}

public class RangeList<T, V> : IEnumerable<KeyValuePair<RangeListKey<T>, V>>
    where T : IComparable<T>, IIncrementOperators<T>, IAdditionOperators<T, int, T>, ISubtractionOperators<T, int, T>
{
    private SortedDictionary<RangeListKey<T>?, V> data = new();

    public void Add(T start, T end, V value)
    {
        var datakeys = data.Keys.ToList();

        // start fits in range
        var startfits = datakeys.Where(k => k.Start.CompareTo(start) < 0 && k.End.CompareTo(start) >= 0).ToList();
        var endfits = datakeys.Where(k => k.End.CompareTo(end) > 0 && k.Start.CompareTo(end) <= 0).ToList();

        var remove = datakeys.Where(t => t.Start.CompareTo(start) >= 0 && t.End.CompareTo(end) <= 0).Distinct().ToList();
        remove.ForEach(t => data.Remove(t));

        var remove2 = new List<RangeListKey<T>>();

        startfits.ForEach(startfit =>
        {
            if (startfit.Start.CompareTo(start) < 0)
            {
                var newSegBefore = new RangeListKey<T> { Start = startfit.Start, End = start - 1 };
                data.Add(newSegBefore, data[startfit]);
            }

            remove2.Add(startfit);
        });

        endfits.ForEach(endfit =>
        {
            if (endfit.End.CompareTo(end) > 0)
            {
                var newSegAfter = new RangeListKey<T> { Start = end + 1, End = endfit.End };
                data.Add(newSegAfter, data[endfit]);
            }

            remove2.Add(endfit);
        });

        data.Add(new RangeListKey<T> { Start = start, End = end }, value);

        remove2.Distinct().ToList().ForEach(t => data.Remove(t));
    }

    public void Remove(T start, T end)
    {
        var datakeys = data.Keys.ToList();

        // start fits in range
        var startfits = datakeys.Where(k => k.Start.CompareTo(start) < 0 && k.End.CompareTo(start) >= 0).ToList();
        var endfits = datakeys.Where(k => k.End.CompareTo(end) > 0 && k.Start.CompareTo(end) <= 0).ToList();

        var remove = datakeys.Where(t => t.Start.CompareTo(start) > 0 && t.End.CompareTo(end) < 0).Distinct().ToList();
        remove.ForEach(t => data.Remove(t));

        var remove2 = new List<RangeListKey<T>>();

        startfits.ForEach(startfit =>
        {
            if (startfit.Start.CompareTo(start) < 0)
            {
                var newSegBefore = new RangeListKey<T> { Start = startfit.Start, End = start - 1 };
                data.Add(newSegBefore, data[startfit]);
            }

            remove2.Add(startfit);
        });

        endfits.ForEach(endfit =>
        {
            if (endfit.End.CompareTo(end) > 0)
            {
                var newSegAfter = new RangeListKey<T> { Start = end + 1, End = endfit.End };
                data.Add(newSegAfter, data[endfit]);
            }

            remove2.Add(endfit);
        });

        remove2.Distinct().ToList().ForEach(t => data.Remove(t));
    }

    public string Output()
    {
        var sb = new StringBuilder();
        foreach (var key in data.Keys)
        {
            var item = data[key];
            sb.Append($"[({key.Start},{key.End}),{item}]");
        }
        return sb.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return data.GetEnumerator();
    }

    public IEnumerator<KeyValuePair<RangeListKey<T>, V>> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    internal void Remove(RangeListKey<T> t)
    {
        Remove(t.Start, t.End);
    }
}
