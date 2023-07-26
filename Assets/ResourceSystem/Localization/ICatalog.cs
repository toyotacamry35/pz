namespace L10n
{
    public interface ICatalog
    {
        /// <summary>
        /// Получение строки по ключу
        /// </summary>
        string GetStringAuto(string key, long pluralNum, object[] args);

        /// <summary>
        /// Получение строки из предоставленного translationData
        /// </summary>
        string GetStringFromCustomData(TranslationData translationData, long pluralNum, object[] args);
    }
}