namespace Our.Umbraco.Ditto
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UmbracoRelationAttribute : DittoValueResolverAttribute
    {
        public UmbracoRelationAttribute()
            : base(typeof(UmbracoRelationResolver))
        {
        }

        public UmbracoRelationAttribute(string relationTypeAlias, RelationDirection relationDirection = RelationDirection.ChildToParent)
            : base(typeof(UmbracoRelationResolver))
        {
            this.RelationDirection = relationDirection;
            this.RelationTypeAlias = relationTypeAlias;
        }

        public RelationDirection RelationDirection { get; set; }

        public string RelationTypeAlias { get; set; }
    }
}