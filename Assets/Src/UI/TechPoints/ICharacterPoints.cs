using System;
using System.Collections.Generic;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;

namespace Uins
{
    public interface ICharacterPoints
    {
        //PlayerPoints StartPlayerPoints { get; }
        DictionaryStream<CurrencyResource, int> CurrenciesStream { get; }

        DictionaryStream<ScienceDef, int> SciencesStream { get; }

        [Obsolete("Use CurrenciesStream instead this")]
        IStream<DictionaryStream<CurrencyResource, int>> TechPointsChangesStream { get; }

        [Obsolete("Use SciencesStream instead this")]
        IStream<DictionaryStream<ScienceDef, int>> SciencesChangesStream { get; }

        [Obsolete]
        int GetTechPointsCount(CurrencyResource techPoint);

        [Obsolete]
        int GetSciencesCount(ScienceDef scienceDef);

        [Obsolete("Use AvailableCurrencies instead")]
        List<CurrencyResource> GetAvailableTechPoints();

        [Obsolete("Use AvailableCurrencies instead")]
        List<ScienceDef> GetAvailableSciences();

        ListStream<CurrencyResource> AvailableCurrencies { get; }

        ListStream<ScienceDef> AvailableSciences { get; }

        [Obsolete("Use CurrenciesStream instead this")]
        event TechPointsCountChangedDelegate TechPointsCountChanged;

        [Obsolete("Use SciencesStream instead this")]
        event SciencePointsCountChangedDelegate SciencePointsCountChanged;
    }
}