# FormatDax deprecation

The `FormatDax` method (which is one of the available [helper methods](/Advanced-Scripting.md#helper-methods) in Tabular Editor) has been deprecated with the release of Tabular Editor 2.13.0.

The reason for the deprecation is that the web service at https://www.daxformatter.com/ was starting to experience a heavy load of multiple request in quick succession, which were causing issues at their end. This is because the `FormatDax` method performs a web request each time it is called in a script, and many people have been using scripts such as the following:

**Don't do this!**
```csharp
foreach(var m in Model.AllMeasures)
{
    // DON'T DO THIS
    m.Expression = FormatDax(m.Expression);
}
```

This is fine for small models with a few tens of measures, but the traffic on www.daxformatter.com indicates that a script such as the above is being executed across multiple models with thousands of measures, even several times per day!

To address this issue, Tabular Editor 2.13.0 will show a warning when `FormatDax` is called more than three times in a row, using the syntax above. In addition, subsequent calls will be throttled with a 5 second delay between each call.

## Alternative syntax

Tabular Editor 2.13.0 introduces two different ways of calling FormatDax. The above script can be rewritten into either of the following:

```csharp
foreach(var m in Model.AllMeasures)
{
    m.FormatDax();
}
```

...or simply...:

```csharp
Model.AllMeasures.FormatDax();
```

Both these approaches will batch all www.daxformatter.com calls into a single request. You may also use the global method syntax if you prefer:

```csharp
foreach(var m in Model.AllMeasures)
{
    FormatDax(m);
}
```

...or simply...:

```csharp
FormatDax(Model.AllMeasures);
```

## More details

Technically, `FormatDax` has now been implemented as two overloaded extension methods:

1) `void FormatDax(this IDaxDependantObject obj)`
2) `void FormatDax(this IEnumerable<IDaxDependantObject> objects, bool shortFormat = false, bool? skipSpaceAfterFunctionName = null)`

Overload #1 above will queue the provided object for formatting when script execution completes, or when a call is made to the new `void CallDaxFormatter()` method. Overload #2 will immediately call www.daxformatter.com with a single web request that will format all DAX expressions for all the objects provided in the enumerable. You may use either of these methods as you see fit.

Note that the new method does not take any string arguments. It considers all DAX properties on the provided object for formatting (for measures, this is the Expression and DetailRowsExpression properties, for KPIs, this is the StatusExpression, TargetExpression and TrendExpression, etc.).
