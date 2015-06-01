namespace Our.Umbraco.Ditto
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CurrentContentAttribute : DittoValueResolverAttribute
    {
        public CurrentContentAttribute()
            : base(typeof(CurrentContentValueResolver))
        {
        }
    }
}