namespace L10n.Loaders
{
    /// <summary>
    /// Represents an abstract loader that loads required data to the catalog.
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Loads translations to the specified catalog using catalog's culture info.
        /// </summary>
        /// <param name="textCatalog">A catalog instance to load translations to.</param>
        void Load(TextCatalog textCatalog);
    }
}