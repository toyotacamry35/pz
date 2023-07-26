using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Entities.Service;

namespace SharedCode.EntitySystem
{
    public class CheatRpcAttribute: Attribute
    {
        public AccountType AccountType { get; }

        public CheatRpcAttribute(AccountType accountType)
        {
            AccountType = accountType;
        }
    }

    // TODO Похорошему нужно переделать accountType на динамические роли
    public class ManualCheatRpcAttribute : Attribute
    {
        public AccountType AccountType { get; }

        public ManualCheatRpcAttribute(AccountType accountType)
        {
            AccountType = accountType;
        }
    }
}
