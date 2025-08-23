using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using YZ;

namespace YZ {


    public enum Severity {
        [Description("Не ошибка"), BriefDescription("Нет")] NotError = 0,
        [Description("Замечание"), BriefDescription("Замеч.")] Hint = 1,
        [Description("Предупреждение"), BriefDescription("Предупр.")] Warning = 2,
        [Description("Ошибка"), BriefDescription("Ошибка")] Error = 3,
        [Description("Критическая ошибка"), BriefDescription("Крит.ош.")] Critical = 4,
        [Description("Фатальная ошибка"), BriefDescription("Фат.ош.")] Fatal = 5
    }

    public enum RussianCase {
        /// <summary>
        /// Именительный падеж
        /// </summary>
        [Description("Именительный")] Nominative = 1,

        /// <summary>
        /// Родительный падеж
        /// </summary>
        [Description("Родительный")] Genitive,
        /// <summary>
        /// Дательный падеж
        /// </summary>
        [Description("Дательный")] Dative,
        /// <summary>
        /// Винительный падеж
        /// </summary>
        [Description("Винительный")] Accusative,
        /// <summary>
        /// Творительный падеж
        /// </summary>
        [Description("Творительный")] Instrumental,
        /// <summary>
        /// Предложный падеж
        /// </summary>
        [Description("Предложный")] Prepositional
    }

    public enum RussianCount {
        /// <summary>
        /// Ноль, Пять, Шесть или много...
        /// </summary>
        Zero,
        /// <summary>
        /// Один, Двадцать Один... 
        /// </summary>
        One,
        /// <summary>
        /// Два, Три, Четыре...
        /// </summary>
        Two
    }


    public enum TimeUnit { Second, Minute, Hour, Day, Week, Month, Year }



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
        Json,
        [Description("INFO: "), ConsoleColor(ConsoleColor.Blue)]
        InfoTemp
    }

    [Flags] public enum GetDescriptionMode { Brief = 1, Full = 2, BriefOrFull = 3 }

    public enum SpeedUnits {
        [Suffix("m/s"),  Normalize(1,1)]                MetersPerSecond,
        [Suffix("km/h"), Normalize(0.277778,3.6)]       KilometersPerHour,
        [Suffix("kn/h"), Normalize(0.514444,1.94384)]   KnotsPerHour,
        [Suffix("mph"),  Normalize(0.44704,2.23694)]    MilesPerHour
    };

}
