using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.OurSimpleIoC
{
    public interface IServices
    {
        T Get<T>(object context = null);
        void Register<T>(object service);
        void Inject(object target);
    }
}
