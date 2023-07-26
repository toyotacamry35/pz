using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    public class LocalizationConfigDef : BaseResource
    {
        /// <summary>
        /// Имя папки локализаций
        /// </summary>
        public string LocalesDirName { get; set; }

        /// <summary>
        /// Список локализаций
        /// </summary>
        public CultureData[] LocalizationCultures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CultureData DevCulture { get; set; }

        /// <summary>
        /// Локализация по ум. (если на машине пользователя стоит локаль, которой нет соответствия в списке локализаций, то при выбирается эта)
        /// </summary>
        public int DefaultLocalizationIndex { get; set; }

        /// <summary>
        /// Индекс языка, с которого начинается перевод (dev не в счет); ru_RU
        /// </summary>
        public int LocalizationPipelineIndex1 { get; set; }

        /// <summary>
        /// //Индекс языка первого перевода (dev не в счет); en_US
        /// </summary>
        public int LocalizationPipelineIndex2 { get; set; } 

        /// <summary>
        /// Имя файла языковой базы
        /// </summary>
        public string DomainFile { get; set; }

        /// <summary>
        /// Имена jdb-файлов, для которых при операциях обработки будет выводиться детальное логирование
        /// </summary>
        public string[] FilenamesForDebug { get; set; }

        public ResourceRef<XlsExportConfigDef> XlsExportConfig { get; set; }
    }
}