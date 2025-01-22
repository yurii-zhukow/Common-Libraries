using System.Collections.Generic;
using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Linq.Dynamic.Core.Exceptions;
//using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
//using Microsoft.CodeAnalysis.CSharp.Scripting;
//using Microsoft.CodeAnalysis.Scripting;

namespace YZ {

    public static partial class Helpers {

        //public static Script<T> Eval<T>(this string s, ScriptOptions options, Type globalsType) => CSharpScript.Create<T>(s, options, globalsType);

        //public static TResult Eval<TResult>(this string expr, IEnumerable<(string Name, object Value)> vars) {

        //    vars = vars.OrderBy(t => t.Name).Distinct(new GenericEqualityComparer<(string Name, object Value), string>(t => t.Name)).ToArray();
        //    var prms = vars.Select(t => Expression.Parameter(t.Value.GetType(), t.Name)).ToArray();
        //    var vals = vars.Select(t => t.Value).ToArray();

        //    Expression body = new ExpressionParser(prms, expr, prms, new ParsingConfig()).Parse(typeof(TResult));

        //    LambdaExpression fn = Expression.Lambda(body, prms);

        //    //var fn = System.Linq.Dynamic.DynamicExpression.ParseLambda(prms, typeof(TResult), expr);
        //    var res = fn.Compile().DynamicInvoke(vals);
        //    if (res is TResult r) return r;
        //    throw new ParseException($"Failed to parse expression {expr} with args [{vars.ToString(",", nv => $"{nv.Value.GetType().Name} {nv.Name} = {nv.Value.ToString().Ellipsis(100)}")}]", 0);
        //}
    }

}
