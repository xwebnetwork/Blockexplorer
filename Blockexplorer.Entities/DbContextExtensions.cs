using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

namespace Blockexplorer.Entities
{
    public static class DbContextExtensions
    {
        public static readonly string CurrentIsolationLevelSql = $@"
        SELECT
            CASE transaction_isolation_level
                WHEN 0 THEN N'{IsolationLevel.Unspecified}'
                WHEN 1 THEN N'{IsolationLevel.ReadUncommitted}''
                WHEN 2 THEN N'{IsolationLevel.ReadCommitted}''
                WHEN 3 THEN N'{IsolationLevel.RepeatableRead}''
                WHEN 4 THEN N'{IsolationLevel.Serializable}''
                WHEN 5 THEN N'{IsolationLevel.Snapshot}''
            END
        FROM sys.dm_exec_sessions
        WHERE session_id = @@SPID";

        public static string CurrentIsolationLevel(this DbContext context)
        {
            using (DbCommand command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = CurrentIsolationLevelSql;
                command.Transaction = context.Database.CurrentTransaction.GetDbTransaction();
                return (string)command.ExecuteScalar();
            }
        }
    }
}
