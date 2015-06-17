namespace Our.Umbraco.Ditto
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    public enum RelationDirection
    {
        Bidirectional,

        ChildToParent,

        ParentToChild
    }

    public class UmbracoRelationResolver : DittoValueResolver<UmbracoRelationAttribute>
    {
        public override object ResolveValue(ITypeDescriptorContext context, UmbracoRelationAttribute attribute, CultureInfo culture)
        {
            var content = context.Instance as IPublishedContent;
            if (content == null)
            {
                return Enumerable.Empty<IRelation>();
            }

            IEnumerable<IRelation> relations;

            switch (attribute.RelationDirection)
            {
                case RelationDirection.ChildToParent:
                    relations = ApplicationContext.Current.Services.RelationService.GetByChildId(content.Id);
                    break;

                case RelationDirection.ParentToChild:
                    relations = ApplicationContext.Current.Services.RelationService.GetByParentId(content.Id);
                    break;

                case RelationDirection.Bidirectional:
                default:
                    relations = ApplicationContext.Current.Services.RelationService.GetByParentOrChildId(content.Id);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(attribute.RelationTypeAlias))
            {
                relations = relations.Where(x => x.RelationType.Alias.InvariantEquals(attribute.RelationTypeAlias));
            }

            return relations;
        }
    }
}