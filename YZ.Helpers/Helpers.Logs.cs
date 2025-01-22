#define WRITE_LOG_TO_FILE
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace YZ {


    public enum Verbosity : byte {

        [Description(""), ConsoleColor(ConsoleColor.Gray)]
        Any = 0,

        [Description("HINT"), ConsoleColor(ConsoleColor.DarkGray)]
        Hint,

        [Description("INFO"), ConsoleColor(ConsoleColor.Gray)]
        Info,

        [Description("INFO"), ConsoleColor(ConsoleColor.Green)]
        Important,

        [Description("WARNING"), ConsoleColor(ConsoleColor.Yellow)]
        Warning,

        [Description("ERROR"), ConsoleColor(ConsoleColor.Red)]
        Error,

        [Description("CRITICAL ERROR"), ConsoleColor(ConsoleColor.Red)]
        Critical,

        [Description("FATAL ERROR"), ConsoleColor(ConsoleColor.Red)]
        Fatal,

        [Description("DISABLE"), ConsoleColor(ConsoleColor.Red)]
        Disable,

        [Description("INFO"), ConsoleColor(ConsoleColor.Blue)]
        Temp

    }



    public class LogStorage : IDisposable {

        public static readonly Dictionary<Verbosity, ConsoleColor> VerbosityConsoleColors = Helpers.EnumToDictionary<Verbosity, ConsoleColorAttribute, ConsoleColor>(true, resFunc: a => a.Color, dflt: _ => ConsoleColor.DarkGray);
        public static readonly Dictionary<Verbosity, string> VerbosityDescriptions = Helpers.EnumToDictionary<Verbosity, DescriptionAttribute, string>(true, resFunc: a => a.Description, dflt: t => t.ToString());
#if WRITE_LOG_TO_FILE
        static object curLogLock = new object();
        static string curLogFilename = null;
        static Stream curLogStream = null;
        static TextWriter curLogWriter = null;
        static DateTime curLogStarted = DateTime.MinValue;
        static int curLogLinesSaved = 0;
        static DateTime curLogLastWritten = DateTime.Now;
        static Timer autoCloseTimer = new Timer(_ => {
            lock (curLogLock) { if ((DateTime.Now - curLogLastWritten).TotalMinutes < 1) return; }
            closeCurLogFile("Log is not active. Closed.");

        }, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));

        static void closeCurLogFile( string s ) {
            try {
                if ( curLogWriter != null && !string.IsNullOrWhiteSpace( s ) ) {
                    curLogWriter.WriteLine( $"\n\n[{DateTime.Now:g}] {s}" );
                }
                curLogWriter?.Close();
                curLogWriter?.Dispose();
                curLogStream?.Close();
                curLogStream?.Dispose();
            } finally {
                curLogStream = null;
                curLogWriter = null;
            }
        }

        static void newCurLogFile() {
            var n = DateTime.Now;
            var dir = Path.Combine( "Temp".GetFullAppPath(),"Logs");
            Directory.CreateDirectory( dir );
            var newLogFilename = Path.Combine( dir,$"{n:yyyy-MM-dd HH-mm-ss}.log");

            closeCurLogFile( $"File closed. Next log will be created: {newLogFilename}" );
            try {
                curLogFilename = newLogFilename;
                for ( var x = 0; x < 1000; x++ )
                    try {
                        curLogStream = new FileStream( curLogFilename, FileMode.CreateNew, FileAccess.Write );
                        curLogWriter = curLogStream == null ? null : new StreamWriter( curLogStream, Encoding.UTF8 ) { AutoFlush = true };
                        curLogStarted = n;
                        curLogLinesSaved = 0;
                        break;
                    } catch ( Exception ex ) {
                        Console.WriteLine( ex.Message );
                        curLogFilename = Path.Combine( dir, $"{n:yyyy-MM-dd HH-mm-ss} ({++x}).log" );
                        continue;
                    }
            } catch {
                curLogStream = null;
                curLogWriter = null;
            }

        }

        static void checkCurLogFile() {
            if ( curLogStream == null || ( DateTime.Now - curLogStarted > TimeSpan.FromHours( 1 ) ) || curLogLinesSaved > 10000 ) newCurLogFile();
        }



        static void writeLineToCurLogFile( string s ) {
            lock ( curLogLock ) {
                checkCurLogFile();
                curLogWriter?.WriteLine( s );
                curLogLastWritten = DateTime.Now;
                curLogLinesSaved++;
            }
        }
        public static Verbosity DefaultVerbosity { get; set; } = Verbosity.Any;
        public static Verbosity DefaultFileWriteVerbosity { get; set; } = Verbosity.Disable;

#endif

        static object consoleLocker = new object();
        public static void DefaultLogEvent( LogStorage sender, Item message ) {
            var pfx = $"{sender.Prefix}.{message.Prefix}".Trim('.');
            var s = $"{DateTime.Now:T} [{VerbosityDescriptions[message.Verbosity]}] {pfx} : {message.Message}";
            lock ( consoleLocker ) {
                Console.ForegroundColor = VerbosityConsoleColors[ message.Verbosity ];
                Console.WriteLine( s );
                Console.ResetColor();
            }
        }

        public delegate void LogEvent( LogStorage sender, Item message );
        public event LogEvent OnLog;

        readonly short bufferSize;

        readonly Queue<Item> items;

        readonly object itemsLocker = new object();

        //        short cnt;
        ulong ix;

        public LogStorage( short bufferSize, Verbosity verbosity, string prefix = "", LogEvent onLog = null ) {
            this.bufferSize = bufferSize;
            items = new Queue<Item>( bufferSize + 1 );
            Verbosity = verbosity;
            Prefix = prefix;
            onLog ??= DefaultLogEvent;
            OnLog += onLog;
        }
        public string Prefix { get; set; }
        public Verbosity Verbosity { get; set; } = DefaultVerbosity;
        public Verbosity FileWriteVerbosity { get; set; } = DefaultFileWriteVerbosity;
        public List<Item.Dto> GetItems( ulong startFromId ) {
            lock ( itemsLocker ) {
                return items.SkipWhile( l => l.Index < startFromId ).Take( 100 ).Select( v => new Item.Dto( v ) ).ToList();
            }
        }
        public static List<Item.Dto> GetItemsError( string err ) => new List<Item.Dto> { new Item.Dto( (byte)Verbosity.Error, 0, DateTime.Now.FromJan2020( Helpers.OffsetTimeUnit.Seconds ), "Error", err ) };

        public void Log( string s, Verbosity v = Verbosity.Info, string prefix = "" ) {
            if ( v < Verbosity && v < FileWriteVerbosity ) return;
            if ( v >= Verbosity ) {
                ix++;
                var item = new Item(v, ix, DateTime.Now, prefix, s);
                //lock (itemsLocker) {
                //    if (cnt < bufferSize) cnt++;
                //    else items.Dequeue();
                //    items.Enqueue(item);
                //}
                OnLog?.Invoke( this, item );
            }
#if WRITE_LOG_TO_FILE
            if ( v >= FileWriteVerbosity ) {
                var pfx = $"{Prefix}.{prefix}".Trim('.');
                writeLineToCurLogFile( $"{DateTime.Now:T} [{VerbosityDescriptions[ v ]}] {pfx} : {s}" );
            }
#endif



        }

        public void Error( Exception ex, Verbosity v = Verbosity.Error, string prefix = "" ) => Log( $"{ex.GetInner( "   <==   " )} \n\nStack:\n{ex.StackTrace}\n\n", v, prefix );
        public void Error( string s, Verbosity v = Verbosity.Error, string prefix = "" ) => Log( s, v, prefix );
        public void Dispose() {
            autoCloseTimer?.Dispose();
            autoCloseTimer = null;
        }

        public class Item {

            public Item( Verbosity verbosity, ulong index, DateTime date, string prefix, string message ) {
                Verbosity = verbosity;
                Index = index;
                Date = date;
                Prefix = prefix;
                Message = message;
            }

            public Verbosity Verbosity { get; }
            public string Prefix { get; }
            public ulong Index { get; }
            public DateTime Date { get; }
            public string Message { get; }

            public struct Dto {

                public Dto( Item src ) : this( (byte)src.Verbosity, src.Index, src.Date.FromJan2020( Helpers.OffsetTimeUnit.Seconds ), src.Prefix, src.Message ) { }

                public Dto( byte v, ulong x, long d, string p, string m ) {
                    this.v = v;
                    this.x = x;
                    this.d = d;
                    this.p = p;
                    this.m = m;
                }

                public byte v;
                public ulong x;
                public long d;
                public string p;
                public string m;

            }

        }



    }

    public interface ILoggerClient {
        void Log( string s, Verbosity v, string prefix = "", [CallerMemberName] string n = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 );
        void Error( Exception ex, Verbosity v, string prefix = "", [CallerMemberName] string n = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 );
        LogStorage Storage { get; }

    }

    public class LoggerClient : ILoggerClient, IDisposable {

        public LoggerClient( LogStorage storage, string prefix = null ) {
            Storage = storage;
            Prefix = prefix ?? GetType().Name;
        }

        public LogStorage Storage { get; }
        public string Prefix { get; set; }
        public void Log( string s, Verbosity v = Verbosity.Info, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Storage.Log( s, v, $"{Prefix}.{prefix}.{callerName}{( v == Verbosity.Critical || v == Verbosity.Fatal || v == Verbosity.Error ? $" ({file.GetPathEnding( 2 )} : {line})" : "" )}".Replace( "..", "." ).Trim( '.' ) );
        public void Hint( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Hint, prefix, callerName, file, line );
        public void Info( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Info, prefix, callerName, file, line );
        public void Warning( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Warning, prefix, callerName, file, line );
        public void Fatal( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Fatal, prefix, callerName, file, line );
        public void Critical( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Critical, prefix, callerName, file, line );
        public void Important( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Important, prefix, callerName, file, line );
        public void Error( Exception ex, Verbosity v = Verbosity.Error, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Storage.Error( ex, v, $"{Prefix}.{prefix}.{callerName} ({file.GetPathEnding( 2 )} : {line})".Replace( "..", "." ).Trim( '.' ) );
        public void Error( string s, Verbosity v = Verbosity.Error, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Storage.Error( s, v, $"{Prefix}.{prefix}.{callerName} ({file.GetPathEnding( 2 )} : {line})".Replace( "..", "." ).Trim( '.' ) );

        public void InfoTemp( string s, string prefix = "", [CallerMemberName] string callerName = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = -1 ) => Log( s, Verbosity.Temp, prefix, callerName, file, line );

        public virtual void Dispose() {
        }
    }


}
