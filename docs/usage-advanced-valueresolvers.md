## Value Resolvers

While Ditto covers the most common types of value resolution, (via the use of [attributes](usage-advanced-attributes), there may be scenarios where you may need a little help in resolving custom (or complex) values.

Traditionally any custom value resolution logic would be typically done within an MVC controller.  However, if the logic is only relevant to the mapping operation, then it may clutter your controller code and be better suited as a custom `ValueResolver`.

For example, let's look at having a calculated value during mapping, say that you wanted to display the number of days since a piece of content was last updated:

```csharp
public class MyModel
{
    [DittoValueResolver(typeof(MyCustomValueResolver))]
    public int DaysSinceUpdated { get; set; }
}

public class MyCustomValueResolver : DittoValueResolver
{
    public override object ResolveValue()
    {
        return (DateTime.UtcNow - Content.UpdateDate).Days;
    }
}
```

Once mapped, the value of `DaysSinceUpdated` would contain the number of days difference between the content item's last update date and today's date (UTC now).


