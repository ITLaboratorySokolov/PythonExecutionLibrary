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
        private const string ReturnVariableName = "res";

        public bool initializedOnce;

        /// <summary>
        /// Set python .dll must be called before executing any code
        /// </summary>
        /// <param name="path"> Path to python dll </param>
        public void SetPython(string path)
        {
            // Runtime.PythonDLL throws an exception if the engine is already initialized
            // There is no reason to change dll if engine is already running
            // This prevents the exception
            if (initializedOnce)
            {
                return;
            }

            if (File.Exists(path) && path.EndsWith(".dll"))
            {
                Runtime.PythonDLL = path;
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
            CodeParser.CreateFunctionHeader(builder, funcName, paramNames);
            CodeParser.CreateFunctionBody(builder, code);
            CodeParser.CreateVariableAssignment(builder, ReturnVariableName);
            CodeParser.CreateFunctionCall(builder, funcName, callParamNames);
            return builder.ToString();
        }

        /// <summary>
        /// Run python code
        /// </summary>
        /// <param name="code"> Code to run </param>
        /// <param name="varValues"> User function call parameters (names and their values - <"name", "John">, <"age", 18> etc) </param>
        /// <param name="returnClass"> Class into which the return values will be stored </param>
        /// <param name="stdout">Output stream</param>
        /// <param name="stderr">Error stream</param>
        public void RunCode(string code, Dictionary<string, object> varValues, IReturnable returnClass, TextWriter stdout = null, TextWriter stderr = null)
        {
            if (!initializedOnce)
            {
                throw new InvalidOperationException("Python engine is not initialized");
            }

            HandleException(() =>
            {
                // acquire the GIL before using the Python interpreter
                using (Py.GIL())
                {
                    // create a Python scope
                    using (var scope = Py.CreateScope())
                    {
                        RedirectStreams(stdout, stderr);
                        SetFunctionParameters(varValues, scope);

                        scope.Exec(code);

                        if (!TryParseReturnValue(returnClass, scope))
                        {
                            throw new FormatException("Return parameters couldn't be parsed");
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Initialize python engine.
        /// </summary>
        public void Initialize()
        {
            if (initializedOnce)
            {
                return;
            }

            HandleException(() =>
            {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                initializedOnce = true;
            });
        }

        /// <summary>
        /// Shutdown python engine.
        /// </summary>
        public void Shutdown()
        {
            if(!initializedOnce)
            {
                return;
            }

            HandleException(() =>
            {
                PythonEngine.Shutdown();
                initializedOnce = false;
            });
        }

        private void HandleException(Action action)
        {
            try
            {
                action();
            }
            catch (TypeInitializationException e)
            {
                initializedOnce = false;

                if (e.InnerException != null)
                {
                    while (e.InnerException is TypeInitializationException inner)
                    {
                        e = inner;
                    }

                    throw e.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }

        private static void RedirectStreams(TextWriter stdout, TextWriter stderr)
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
        }

        private static void SetFunctionParameters(Dictionary<string, object> paramValues, PyModule scope)
        {
            foreach (var param in paramValues)
            {
                scope.Set(param.Key, param.Value);
            }
        }

        private static bool TryParseReturnValue(IReturnable returnClass, PyModule scope)
        {
            bool parsing = true;

            var res = scope.Get(ReturnVariableName);
            if (returnClass != null)
            {
                parsing = returnClass.SetParameters(res);
            }

            res.Dispose();

            return parsing;
        }
    }
}
