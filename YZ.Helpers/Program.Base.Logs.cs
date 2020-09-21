using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using YZ;

namespace YZ {

    public abstract partial class ProgramBase {

        static Dictionary<LogPrefix, string> LogPrefixDescription = YZ.Helpers.EnumToDictionary<LogPrefix, string>(t => t.GetDescription());
        static Dictionary<LogPrefix, ConsoleColor> LogPrefixColor = YZ.Helpers.EnumToDictionary<LogPrefix, ConsoleColorAttribute, ConsoleColor>(false, a => a.Color, e => ConsoleColor.White);

        static object logLock = new object();
        protected static void Log(LogPrefix prefix, string s, ConsoleColor? fg = null, ConsoleColor? bg = null) {
            lock (logLock) {
                Console.ForegroundColor = fg ?? LogPrefixColor[prefix];
                if (bg != null) Console.BackgroundColor = bg.Value;
                Console.WriteLine($"{LogPrefixDescription[prefix]}{s}");
                Console.ResetColor();
            }
        }

        static LogStorage programLogStorage;
        protected static LogStorage ProgramLogStorage => programLogStorage ??= new LogStorage(0, Verbosity.Hint, "", (o, m) => LogMessage(m));

        protected static void LogMessage(LogStorage.Item message) {

            var pfx = message.Verbosity switch
            {
                Verbosity.Info => LogPrefix.Info,
                Verbosity.Important => LogPrefix.Info,
                Verbosity.Warning => LogPrefix.Info,
                Verbosity.Error => LogPrefix.Error,
                Verbosity.Critical => LogPrefix.Error,
                Verbosity.Fatal => LogPrefix.Error,
                _ => LogPrefix.Hint
            };
            Log(pfx, message.Message);
        }

        protected static void Hint(string s) => Log(LogPrefix.Hint, s);
        protected static void Info(string s) => Log(LogPrefix.Info, s);
        protected static void Warn(string s) => Log(LogPrefix.Info, s, fg: ConsoleColor.Yellow);
        protected static void Important(string s) => Log(LogPrefix.Info, s, fg: ConsoleColor.Green);
        protected static void Error(string s) => Log(LogPrefix.Error, s);
        protected static void Error(Exception ex) => Log(LogPrefix.Error, $"{ex.GetInner("  <==  ")}\n\nSTACK:\n\n{ex.StackTrace}");
        protected static void LogData(string s) => Log(LogPrefix.Data, s);
        protected static void LogJson(string s) => Log(LogPrefix.Json, s);

    }
}
