using System;
using System.Threading;

namespace GeneratedCode.Manual.Repositories
{
    public static class CallbackRepositoryHolder
    {
        private static readonly AsyncLocal<Guid> _callbackRepoId = new AsyncLocal<Guid>();
        public static Guid CurrentCallbackRepositoryId { get => _callbackRepoId.Value; set => _callbackRepoId.Value = value; }
    }
}
