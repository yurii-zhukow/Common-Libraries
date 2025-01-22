using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YZ {
    public static partial class Helpers {
        public static string GetPathEnding( this string path, int lastPathParts ) {
            if ( lastPathParts <= 0 || string.IsNullOrWhiteSpace( path ) || !path.Contains( Path.DirectorySeparatorChar ) ) return path;
            var start = path.Length - 1;
            while ( lastPathParts > 0 ) {
                start = path.LastIndexOf( Path.DirectorySeparatorChar, start - 1 );
                if ( start <= 0 ) return path;
                lastPathParts--;
            }

            return path.Substring( start );
        }
    }
}
