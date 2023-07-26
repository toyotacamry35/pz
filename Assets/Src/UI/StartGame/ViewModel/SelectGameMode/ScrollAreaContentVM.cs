namespace Uins
{
    public class ScrollAreaContentVM:BindingVmodel
    {
        public SelectGameModeContentVM SelectGameModeContentVM { get; }

        public ScrollAreaContentVM(SelectGameModeContentVM selectGameModeContentVM)
        {
            SelectGameModeContentVM = selectGameModeContentVM;
        }
    }
}