using System.ComponentModel;

public interface INotifyPropertyChangedExt : INotifyPropertyChanged
{
    void NotifyPropertyChanged(string propertyName = null);
}