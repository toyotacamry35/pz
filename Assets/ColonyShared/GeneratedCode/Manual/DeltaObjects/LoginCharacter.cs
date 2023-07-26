using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Cloud;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Interfaces;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace GeneratedCode.DeltaObjects
{
    public partial class LoginCharacter
    {
        public async Task<bool> TestMethod1Impl(int a, Guid itemId)
        {
            return a > 5;
        }

        public async Task TestMethodSimpleImpl()
        {
            throw new NotImplementedException();
        }

        public async Task TestMethodSimpleParamImpl(string param1)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestMethod2222Impl(int a, Guid itemId)
        {
            throw new NotImplementedException();
        }
    }
}
