using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using YZ;

namespace YZ {

    public enum LogPrefix {
        [Description(""), ConsoleColor(ConsoleColor.White)]
        None,
        [Description("HINT: "), ConsoleColor(ConsoleColor.DarkGray)]
        Hint,
        [Description("INFO: "), ConsoleColor(ConsoleColor.White)]
        Info,
        [Description("WARN: "), ConsoleColor(ConsoleColor.Yellow)]
        Warning,
        [Description("ERROR: "), ConsoleColor(ConsoleColor.Red)]
        Error,
        [Description("DATA: "), ConsoleColor(ConsoleColor.Green)]
        Data,
        [Description("JSON: "), ConsoleColor(ConsoleColor.DarkGreen)]
        Json
    }


}
