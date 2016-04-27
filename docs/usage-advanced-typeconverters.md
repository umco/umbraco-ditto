## Type Converters

As of Ditto v0.9.0 TypeConverters have been deprecated and replaced with [Processors][usage-advanced-processors].

If you have developed a custom TypeConverters against a previous version of Ditto, then [please refer to this migration guide](upgrade-090).

> **Note:** It is important to note that when getting a property-value from Umbraco, any associated TypeConverters (for the target value-type) will still be called.  In a nutshell, this means that you can use TypeConverters with Umbraco, but not Ditto.
