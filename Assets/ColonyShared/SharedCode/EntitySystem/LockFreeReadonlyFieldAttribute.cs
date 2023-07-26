using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    //один раз выставили и всё, больше никогда не можем выставлять
    [AttributeUsage(AttributeTargets.Property)]
    public class LockFreeReadonlyPropertyAttribute : Attribute
    {
    }
    //можем менять но сами отвечаем за атомарность, либо делая лок в имплементации проперти, либо используя на атомарных значениях
    //использовать ТОЛЬКО в очень хай-перфоманс и простых вещах, вроде частого опроса позиции, когда хочется избежать перелока постоянного, при наличии захваченной энтити
    [AttributeUsage(AttributeTargets.Property)]
    public class LockFreeMutablePropertyAttribute : Attribute
    {
        public bool NonAtomicThreadsafe = false;
    }
}
