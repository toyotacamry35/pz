using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GeneratedCode.Manual.Repositories
{
    public static class MigrationIdHolder
    {
        private static readonly AsyncLocal<Guid> _currentMigrationId = new AsyncLocal<Guid>();
        public static Guid CurrentMigrationId { get => _currentMigrationId.Value; }
        public static void SetCurrentMigrationId(ref Guid migrationId)
        {
            _currentMigrationId.Value = migrationId;
        }
    }
}
