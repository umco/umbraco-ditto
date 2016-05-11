title: Installation
---

## <a name="requirements" title="Requirements"></a>Requirements

> **_Note:_** Ditto has been developed against **Umbraco v6.2.5** and will support that version and above.

Ditto can be installed from either Our Umbraco or NuGet package repositories, or build manually from the source-code:

---------------------------------------------------------------------------------------

## <a name="our-umbraco" title="Our Umbraco Installation"></a>Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [http://our.umbraco.org/projects/developer-tools/ditto](http://our.umbraco.org/projects/developer-tools/ditto) 

---------------------------------------------------------------------------------------

## <a name="nuget" title="Nuget Installation"></a>NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.Ditto), you can run the following command from within Visual Studio:

```bash
PM> Install-Package Our.Umbraco.Ditto
```

We also have a [MyGet package repository](https://www.myget.org/gallery/umbraco-packages) - for bleeding-edge / development releases.

---------------------------------------------------------------------------------------

## <a name="manual" title="Manual Installation"></a>Manual build

If you prefer, you can compile Ditto yourself, you'll need:

* [Visual Studio 2012 (or above, including Community Editions)](https://www.visualstudio.com/downloads/)
* Microsoft Build Tools 2013 (aka [MSBuild 12](https://www.microsoft.com/en-us/download/details.aspx?id=40760))

To clone it locally run the following git commands:

```bash
git clone https://github.com/leekelleher/umbraco-ditto.git umbraco-ditto
cd umbraco-ditto
.\build.cmd
```

---------------------------------------------------------------------------------------

## <a name="upgrade-09" title="Umbraco to 0.9"></a>Umbraco to 0.9

With Ditto v0.9.0 the introduction of Processors has made an overall breaking change for developers who have built custom TypeConverters or ValueResolvers with previous Ditto versions. The intention of Processors are to combine the power and flexibility of both TypeConverters and ValueResolvers together.

### TypeConverters

Here are some notes on how to refactor your custom TypeConverters to use the new Processors.

#### Functional changes

* Change the class inheritance from `DittoConverter` to `DittoProcessor`
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

* If your custom TypeConverter class name has a "Converter" suffix, consider changing this to use the "Processor" suffix. This makes no change in how Ditto uses the class, it is purely for cosmetic and conventional reasons.


### ValueResolvers

Here are some notes on how to refactor your custom ValueResolvers to use the new Processors.

* Change the class inheritance from `DittoValueResolver` to `DittoProcessor`
* Change the main `ResolveValue` name and method signature from:

```csharp
public override object ResolveValue()
```

to

```csharp
public override object ProcessValue()
```

The rest of your old ValueResolver logic should remain the same.

> If you do encounter any major issues in refactoring a ValueResolver, please do let us know. We would like these notes to cover as much as they can.
