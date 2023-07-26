using L10n;
using UnityEngine;

namespace Uins.Inventory
{
    public interface IContextViewTargetResolver
    {
        (LocalizedString, Sprite, bool) GetContextParams(IContextViewTargetWithParams target);
    }
}