using Python.Runtime;
using System;
using System.Collections.Generic;

namespace ZCU.PythonExecutionLibrary
{
    /// <summary>
    /// Class that executes python code
    /// </summary>
    public class PythonExecutor
    {
        /// <summary> Error message of the last error </summary>
        public string ERROR_MSG;

        /// <summary>
        /// Set python .dll must be called before executing any code
        /// </summary>
        /// <param name="path"> Path to python dll </param>
        public void SetPython(string path)
        {
            Runtime.PythonDLL = path;
        }

        /// <summary>
        /// Create python code
        /// </summary>
        /// <param name="funcName"> User function name </param>
        /// <param name="paramNames"> User function parameter names ("int name", "int age" etc) </param>
        /// <param name="varValues"> User function call parameters (names and their valeus - <"name", "John">, <"age", 18> etc) </param>
        /// <param name="code"> User function python code </param>
        /// <returns> Created python code </returns>
        public string CreateCode(string funcName, List<string> paramNames, Dictionary<string, object> varValues, string code)
        {
            List<string> keyList = new List<string>(varValues.Keys);

            code = CodeParser.IndentFunctionText(code);
            string res = CodeParser.CreateFuncHeader(funcName, paramNames) + "\n" + code;
            res += "\nres = " + CodeParser.CreateFuncCall(funcName, keyList);
            return res;
        }

        /// <summary>
        /// Run python code
        /// </summary>
        /// <param name="code"> Code to run </param>
        /// <param name="varValues"> User function call parameters (names and their valeus - <"name", "John">, <"age", 18> etc) </param>
        /// <param name="returnClass"> Class into which the return values will be stored </param>
        /// <returns> True if successful, false if not - error message saved into ERROR_MSG </returns>
        public bool RunCode(string code, Dictionary<string, object> varValues, IReturnable returnClass)
        {
            try
            {
                PythonEngine.Initialize();
                bool parsing = true;

                // acquire the GIL before using the Python interpreter
                using (Py.GIL())
                {
                    // create a Python scope
                    using (PyModule scope = Py.CreateScope())
                    {
                        foreach (string name in varValues.Keys) 
                        {
                            object v;
                            varValues.TryGetValue(name, out v);
                            scope.Set(name, v);
                        }
                        
                        scope.Exec(code);

                        PyObject res = scope.Get("res");
                        if (returnClass != null)
                            parsing = returnClass.SetParameters(res);
                    }
                }
                PythonEngine.Shutdown();

                if (!parsing)
                    throw new Exception("Return parameters couldn't be parsed");
            } catch (Exception e)
            {
                PythonEngine.Shutdown();

                ERROR_MSG = e.Message;
                if (e.InnerException != null)
                    ERROR_MSG = e.InnerException.Message;

                return false;
            }

            return true;
        }
    }
}
