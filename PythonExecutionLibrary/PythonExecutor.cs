using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZCU.PythonExecutionLibrary
{
    /// <summary>
    /// Class that executes python code
    /// </summary>
    public class PythonExecutor
    {
        /// <summary> Error message of the last error </summary>
        public string ERROR_MSG;

        public bool initializedOnce;
        bool setValidPythonDll;

        /// <summary>
        /// Set python .dll must be called before executing any code
        /// </summary>
        /// <param name="path"> Path to python dll </param>
        public void SetPython(string path)
        {
            if (initializedOnce)
                return;

            if (File.Exists(path) && path.EndsWith(".dll"))
            {
                Runtime.PythonDLL = path;
                setValidPythonDll = true;
            }
        }

        /// <summary>
        /// Create python code
        /// </summary>
        /// <param name="funcName"> User function name </param>
        /// <param name="paramNames"> User function parameter names ("int name", "int age" etc) </param>
        /// <param name="callParamNames"> Names of user function call parameters ("name", "age", etc) </param>
        /// <param name="code"> User function python code </param>
        /// <returns> Created python code </returns>
        public string CreateCode(string funcName, List<string> paramNames, List<string> callParamNames, string code)
        {
            code = CodeParser.IndentFunctionText(code);
            string res = CodeParser.CreateFuncHeader(funcName, paramNames) + "\n" + code;
            res += "\nres = " + CodeParser.CreateFuncCall(funcName, callParamNames);
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
            if (!setValidPythonDll)
            {
                ERROR_MSG = "No valid python.dll set";
                return false;
            }

            try
            {
                bool parsing = true;

                // acquire the GIL before using the Python interpreter
                using (Py.GIL())
                {
                    // create a Python scope
                    using (PyModule scope = Py.CreateScope())
                    {
                        foreach (var varPair in varValues) 
                        {
                            scope.Set(varPair.Key, varPair.Value);
                        }
                        
                        scope.Exec(code);

                        PyObject res = scope.Get("res");
                        if (returnClass != null)
                        {
                            parsing = returnClass.SetParameters(res);
                        }
                            
                        res.Dispose();
                    }
                }
                
                if (!parsing)
                    throw new Exception("Return parameters couldn't be parsed");
            } catch (Exception e)
            {
                ERROR_MSG = e.Message;
                if (e.InnerException != null)
                {
                    ERROR_MSG = e.InnerException.Message;
                }

                return false;
            }

            return true;
        }

        public void Initialize()
        {
            PythonEngine.Initialize();
            initializedOnce = true;
        }

        public void Shutdown()
        {
            PythonEngine.Shutdown();
        }
    }
}
