using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace YZ {



    public class ToleranceAttribute : Attribute {

        public double Tolerance { get; set; }

        public ToleranceAttribute(double tolerance = 0.0) {
            this.Tolerance = tolerance;
        }

    }

    public class ConsoleColorAttribute : Attribute {
        public ConsoleColor Color { get; }
        public ConsoleColorAttribute(ConsoleColor color) { Color = color; }

    }

    public class DeepCopyAttribute : Attribute {

        public bool DeepCopy { get; set; }

        public DeepCopyAttribute(bool deepCopy = true) {
            this.DeepCopy = deepCopy;
        }

    }

    public class EditableAttribute : Attribute {

        bool editable = true;

        public bool Editable {
            get { return editable; }
            set { editable = value; }
        }

        public EditableAttribute(bool editable = true) {
            this.editable = editable;
        }

    }

    public class BriefDescriptionAttribute : Attribute {

        public string BriefDescription { get; set; }

        public BriefDescriptionAttribute(string value = "") {
            BriefDescription = value;
        }

        public override string ToString() {
            return BriefDescription;
        }

    }


    public class SeverityAttribute : Attribute {
        public Severity Severity { get; set; }
        public SeverityAttribute(Severity severity) {
            Severity = severity;
        }

        public static readonly SeverityAttribute NotError = new SeverityAttribute(Severity.NotError);

    }

    public class IsFatalAttribute : Attribute {
        public bool IsFatal { get; }
        public IsFatalAttribute(bool isFatal) => IsFatal = isFatal;
        public IsFatalAttribute() => IsFatal = true;
    }


    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    //public class DefaultValueAttribute : Attribute {

    //    public string DefaultValue { get; set; }

    //    public DefaultValueAttribute(string value) {
    //        DefaultValue = value;
    //    }

    //}


}
