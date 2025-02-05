# RangeList - written by Ingo Karstein
## (C) 2025 MIT License

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

```
         111111111122222222223333333333444444444455555555556666666666777777777788888888889 
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
   |------A---------|      |------B------|                             |-----I------|
```

Second Insert:
    ```
    l.Add(7, 13, "C");
    l.Add(18, 29, "D");
    l.Add(38, 51, "E");
    l.Add(54, 66, "F");
    ```

```
         111111111122222222223333333333444444444455555555556666666666777777777788888888889 
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
      |--C--|    |----D------|       |-E----------|  |--F--------|
```

Third Insert:
    ```
    l.Add(1, 24, "G");
    l.Add(36, 69, "H");
    ```

```
         111111111122222222223333333333444444444455555555556666666666777777777788888888889 
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
|-----------G----------|           |---------H----------------------|
```

Result:
```
|-----------G----------||-D--||-B-||---------H----------------------|  |-----I------|
```

Method "Output()" shows:
`[(1,24),G][(25,30),D][(31,35),B][(36,69),H][(72,85),I]`

If you add another range:

    ```
    l.Add(7, 13, "K");
    ```

```    
         111111111122222222223333333333444444444455555555556666666666777777777788888888889
123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890

      |--K--|
```

Result:
`|--G-||--K--||--G------||-D--||-B-||---------H----------------------|  |-----I------|`

`Output()` shows:
`[(1,6),G][(7,13),K][(14,24),G][(25,30),D][(31,35),B][(36,69),H][(72,85),I]`


