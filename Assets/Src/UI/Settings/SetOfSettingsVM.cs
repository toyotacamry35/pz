using ReactivePropsNs;

namespace Uins.Settings
{
    // #note: Expected lifetime - infinite
    public class SetOfSettingsVM : BindingVmodel
    {
        // Rp текущего выбранного раздела setting'ов.
        //   Оригинал лежит в `SettingsGuiWindowVM`,
        //   передаётся паре (Ctrl-VM) классов SettingsGroupButton, чтобы оттуда вещёлось в этот стрим.
        //   Также передаётся паре `SetOfSettings` - там:
        //    a) слушается и трансформируется в ListStream `SettingHandlerDefinitions`ов,
        //      который подключен к pool'у (тупому без реюза) параметризованному парой `SettingsListElem`.
        //    б) вещается в него `null`, когда нажата кнопка "Back" (так очищается список setting'ов)
        internal IStream<SettingsGroupButtonVM> ContentStream; //а)
        internal ReactiveProperty<SettingsGroupButtonVM> ContentRp; //б)

        internal SetOfSettingsVM(IStream<SettingsGroupButtonVM> contentStream, ReactiveProperty<SettingsGroupButtonVM> contentRp)
        {
            ContentStream = contentStream;
            ContentRp = contentRp;
        }
    }
}
