using System;
using System.Collections.Generic;
using System.Text;


namespace YZ {


    public static partial class Helpers {

        public static bool IsEmptyOrSame(this Guid guid, Guid target) => guid == Guid.Empty || guid == target;

    }

}
