using System.Collections.Generic;
using System.Text;

namespace ZCU.PythonExecutionLibrary
{
    /// <summary>
    /// Class providing methods used for formatting python code
    /// </summary>
    internal class CodeParser
    {
        /// <summary>
        /// Create header of the definition of a python function
        /// </summary>
        /// <param name="funcName"> Function name </param>
        /// <param name="names"> Function parameters ("string name", "int age", etc) </param>
        /// <returns></returns>
        public static string CreateFuncHeader(string funcName, List<string> names)
        {
            var res = new StringBuilder($"def {funcName}(");
            for (var i = 0; i < names.Count - 1; i++)
            {
                res.Append(names[i]);
                res.Append(", ");
            }

            res.Append(names[names.Count - 1] + "):");
            return res.ToString();
        }

        /// <summary>
        /// Create function call of a python function
        /// </summary>
        /// <param name="funcName"> Function name </param>
        /// <param name="names"> Parameters ("name", "age", etc) </param>
        /// <returns></returns>
        public static string CreateFuncCall(string funcName, List<string> names)
        {
            var res = new StringBuilder(funcName + '(');

            for (var i = 0; i < names.Count - 1; i++)
            {
                res.Append(names[i]);
                res.Append(", ");
            }

            res.Append(names[names.Count - 1] + ')');
            return res.ToString();
        }

        /// <summary>
        /// Format python code into a python function body
        /// </summary>
        /// <param name="code"> Code </param>
        /// <returns> Formatted code </returns>
        public static string IndentFunctionText(string code)
        {
            string res = "\t" + code.Replace("\n", "\n\t");
            
            //TODO could be split into lines, then trim each line and then indent
            // depends if return statement will be in required in user code or not

            return res;
        }
    }
}
