using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing.Structure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace YZ.EFCore {

    public static partial class Helpers {
        public static DbContextOptionsBuilder AddSqlLogger<T>(this DbContextOptionsBuilder<T> dbContext) where T : DbContext {
            dbContext.AddInterceptors(new SqlLogger());
            return dbContext;
        }

        public static DbContextOptionsBuilder AddSqlLogger(this DbContextOptionsBuilder dbContext) {
            dbContext.AddInterceptors(new SqlLogger());
            return dbContext;
        }

        public class SqlLogger : DbCommandInterceptor {

            static void WriteLine(CommandEventData data, [CallerMemberName] string caller = "") { Trace.WriteLine($@"EF {caller}: {data}", "YZ.Helpers.SQL"); }

            public override void CommandFailed(DbCommand command, CommandErrorEventData data) { WriteLine(data); }

            public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData data, CancellationToken cancellation) {
                WriteLine(data);
                return Task.CompletedTask;
            }

            public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData data, DbDataReader result) {
                WriteLine(data);
                return result;
            }

            public override object ScalarExecuted(DbCommand command, CommandExecutedEventData data, object result) {
                WriteLine(data);
                return result;
            }

            public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData data, int result) {
                WriteLine(data);
                return result;
            }

            public override Task<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData data, DbDataReader result, CancellationToken cancellation) {
                WriteLine(data);
                //return new ValueTask<DbDataReader>(result);
                return Task.FromResult(result);

            }

            public override Task<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData data, object result, CancellationToken cancellation) {
                WriteLine(data);
                //return new ValueTask<object>(result);
                return Task.FromResult(result);
            }

            public override Task<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData data, int result, CancellationToken cancellation) {
                WriteLine(data);
                //return new ValueTask<int>(result);
                return Task.FromResult(result);
            }
        }

    }
}