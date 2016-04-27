## Processors

The key feature of Ditto is the ability to process a value (typically from an `IPublishedContent` property) and set it to the property of the target view-model. To do this we use a Processor (or a combination of Processors).

While Ditto covers the most common types of processing, (via the use of [attributes](usage-advanced-attributes), there may be scenarios where you may need a little help in processing custom (or complex) values.

Traditionally any custom processor logic would be typically done within an MVC controller.  However, if the logic is only relevant to the mapping operation, then it may clutter your controller code and be better suited as a custom `Processor`.

For example, let's look at having a calculated value during mapping, say that you wanted to display the number of days since a piece of content was last updated:

```csharp
public class MyModel
{
    [MyCustomProcessor]
    public int DaysSinceUpdated { get; set; }
}

public class MyCustomProcessor : DittoProcessorAttribute
{
    public override object ProcessValue()
    {
        var content = Value as IPublishedContent;
        if (content == null) return null;

        return (DateTime.UtcNow - content.UpdateDate).Days;
    }
}
```

Once mapped, the value of `DaysSinceUpdated` would contain the number of days difference between the content item's last update date and today's date (UTC now).
