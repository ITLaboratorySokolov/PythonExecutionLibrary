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
        private bool _initializedOnce;
        private bool _setValidPythonDll;

        /// <summary> Error message of the last error </summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// Set python .dll must be called before executing any code
        /// </summary>
        /// <param name="path"> Path to python dll </param>
        public void SetPython(string path)
        {
            if (_initializedOnce)
            {
                return;
            }

            if (File.Exists(path) && path.EndsWith(".dll"))
            {
                Runtime.PythonDLL = path;
                _setValidPythonDll = true;
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
            var builder = new StringBuilder();
            CodeParser.CreateFuncHeader(builder, funcName, paramNames);
            builder.Append("\n");
            CodeParser.IndentFunctionText(builder, code);
            builder.Append("\nres = ");
            CodeParser.CreateFuncCall(builder, funcName, callParamNames);

            return builder.ToString();
        }

        /// <summary>
        /// Run python code
        /// </summary>
        /// <param name="code"> Code to run </param>
        /// <param name="varValues"> User function call parameters (names and their values - <"name", "John">, <"age", 18> etc) </param>
        /// <param name="returnClass"> Class into which the return values will be stored </param>
        /// <returns> True if successful, false if not - error message saved into ERROR_MSG </returns>
        public bool RunCode(string code, Dictionary<string, object> varValues, IReturnable returnClass, TextWriter stdout = null, TextWriter stderr = null)
        {
            if (!_initializedOnce)
            {
                ErrorMsg = "Python executor is not initialized";
                return false;
            }

            try
            {
                var parsing = true;

                // acquire the GIL before using the Python interpreter
                using (Py.GIL())
                {
                    // create a Python scope
                    using (var scope = Py.CreateScope())
                    {
                        dynamic sys = Py.Import("sys");

                        if (stdout != null)
                        {
                            var output = new Output(stdout);
                            sys.stdout = output;
                        }

                        if (stderr != null)
                        {
                            var output = new Output(stderr);
                            sys.stderr = output;
                        }

                        foreach (var varPair in varValues) 
                        {
                            scope.Set(varPair.Key, varPair.Value);
                        }
                        
                        scope.Exec(code);

                        var res = scope.Get("res");
                        if (returnClass != null)
                        {
                            parsing = returnClass.SetParameters(res);
                        }
                            
                        res.Dispose();
                    }
                }

                if (!parsing)
                {
                    throw new Exception("Return parameters couldn't be parsed");
                }
            } catch (Exception e)
            {
                ErrorMsg = e.Message;
                if (e.InnerException != null)
                {
                    ErrorMsg = e.InnerException.Message;
                }

                return false;
            }

            return true;
        }

        public void Initialize()
        {
            if (!_setValidPythonDll)
            {
                ErrorMsg = "No valid python.dll set";
                return;
            }

            if (!_initializedOnce)
            {
                PythonEngine.Initialize();
                _initializedOnce = true;
            }
        }

        public void Shutdown()
        {
            if (_initializedOnce)
            {
                PythonEngine.Shutdown();
                _initializedOnce = false;
            }
        }
    }
}
