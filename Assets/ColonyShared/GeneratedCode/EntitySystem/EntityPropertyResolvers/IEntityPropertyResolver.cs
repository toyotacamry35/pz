using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem.EntityPropertyResolvers
{
    public interface IEntityPropertyResolver
    {
        T Resolve<T>(IEntity entity, PropertyAddress address);

        int GetPropertyAddress(IEntity entity, object property);
    }
}
