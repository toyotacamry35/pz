using System;

namespace Assets.Src.Effects.FX
{
    public interface IFXElementController
    {
        event Action<IFXElementController> Completed;
        bool Init();
        void Tick();
        void Hide();
        void HideImmediately();
        void Show(FXElementParams fxElementParams);
    }
}