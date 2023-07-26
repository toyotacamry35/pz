using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L10n;
using SharedCode.Entities.GameObjectEntities;

namespace Uins
{
    public static class DefExtensions
    {
        public static LocalizedString GetTitle(this IEntityObjectDef def)
        {
            return def != null ? def.NameLs : LsExtensions.EmptyWarning; //чтобы не писать везде эту конструкцию
        }

    }
}
