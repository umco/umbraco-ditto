namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// The public facade for non extension method Ditto actions
    /// </summary>
    public class Ditto
    {
        public static void RegisterValueResolverContext<TContextType>(TContextType ctx)
            where TContextType : DittoValueResolverContext
        {
            DittoValueResolver.RegisterContext(ctx);
        }
    }
}
