## Upgrading to v0.9.0

With Ditto v0.9.0 the introduction of Processors has made an overall breaking change for developers who have built custom TypeConverters or ValueResolvers with previous Ditto versions. The intention of Processors are to combine the power and flexibility of both TypeConverters and ValueResolvers together.

When you first upgrade to Ditto v0.9.0 and try to compile your web-app project, you may see a whole bunch of compile errors. Don't be afraid, these are all fixable.


### TypeConverters

Here are some notes on how to refactor your custom TypeConverters to use the new Processors.

#### Functional changes

* Change the class inheritance from `DittoConverter` to `DittoProcessorAttribute`
* _If applicable_, remove the `CanConvertFrom` method, (this is no longer relevant to the processor workflow)
* Change the main `ConvertFrom` name and method signature from:

```csharp
public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
```
to

```csharp
public override object ProcessValue()
```

* Change any variable references (inside the `ConvertFrom` method) to the new inherited properties:
  * (`ITypeDescriptorContext`) `context` is now `Context` (capitalized)
  * (`CultureInfo`) `culture` is now accessible via the `Context` object, e.g. `Context.Culture`
  * (`object`) `value` is now `Value` (capitalized)

The rest of your old TypeConverter logic should remain the same.

> If you do encounter any major issues in refactoring a TypeConverter, please do let us know. We would like these notes to cover as much as they can.


#### Cosmetic changes

* If your custom TypeConverter class name has a "Converter" suffix, consider removing this (or consider changing it to have a "Processor" suffix). This makes no change in how Ditto uses the class, it is purely for cosmetic and conventional reasons.




### ValueResolvers

Here are some notes on how to refactor your custom ValueResolvers to use the new Processors.

If you have implemented a custom attribute wrapper for your ValueResolver, you can now combine both the `DittoValueResolverAttribute` and `DittoValueResolver` inherited classes into a single inherited class.

* Change the class inheritance from `DittoValueResolver` to `DittoProcessorAttribute`
* Change the main `ResolveValue` name and method signature from:

```csharp
public override object ResolveValue()
```

to

```csharp
public override object ProcessValue()
```

* If you are combining your ValueResolver and attribute classes, you can remove any references to the `Attribute` property.
* Change any property references for `Content` to `Context.Content`, (the `Context` object contains most references now).

The rest of your old ValueResolver logic should remain the same.

> If you do encounter any major issues in refactoring a ValueResolver, please do let us know. We would like these notes to cover as much as they can.

#### Cosmetic changes

* If your custom ValueResolver class name has a "Resolver" suffix, consider removing this (or consider changing it to have a "Processor" suffix). This makes no change in how Ditto uses the class, it is purely for cosmetic and conventional reasons.
