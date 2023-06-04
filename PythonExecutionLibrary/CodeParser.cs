using System.Collections.Generic;
using System.Reflection;
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
        /// <param name="builder">String builder that contains final code</param>
        /// <param name="funcName"> Function name </param>
        /// <param name="names"> Function parameters ("string name", "int age", etc) </param>
        public static void CreateFunctionHeader(StringBuilder builder, string funcName, List<string> names)
        {
            builder.Append($"def {funcName}(");

            if (names.Count > 0)
            {
                for (var i = 0; i < names.Count - 1; i++)
                {
                    builder.Append(names[i]);
                    builder.Append(", ");
                }

                builder.Append(names[names.Count - 1]);
            }

            builder.Append("):\n");
        }

        /// <summary>
        /// Create function call of a python function
        /// </summary>
        /// <param name="builder">String builder that contains final code</param>
        /// <param name="funcName"> Function name </param>
        /// <param name="names"> Parameters ("name", "age", etc) </param>
        public static void CreateFunctionCall(StringBuilder builder, string funcName, List<string> names)
        {
            builder.Append(funcName + '(');

            if (names.Count > 0)
            {
                for (var i = 0; i < names.Count - 1; i++)
                {
                    builder.Append(names[i]);
                    builder.Append(", ");
                }

                builder.Append(names[names.Count - 1]);
            }

            builder.Append(')');
        }

        /// <summary>
        /// Format python code into a python function body
        /// </summary>
        /// <param name="builder">String builder that contains final code</param>
        /// <param name="code"> Code </param>
        public static void CreateFunctionBody(StringBuilder builder, string code)
        {
            //TODO could be split into lines, then trim each line and then indent
            // depends if return statement will be in required in user code or not
            builder.Append("\t");
            builder.Append(code.Replace("\n", "\n\t"));
            builder.Append("\n");
        }

        /// <summary>
        /// Create assignment of a variable (var = )
        /// </summary>
        /// <param name="builder">String builder that contains final code</param>
        /// <param name="variableName">Name of the variable</param>
        public static void CreateVariableAssignment(StringBuilder builder, string variableName)
        {
            builder.Append(variableName);
            builder.Append(" = ");
        }
    }
}
