using Python.Runtime;
using System.Drawing;
using System.Globalization;
using System.Text;
using ZCU.PythonExecutionLibrary;

namespace PythonExectution;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World! Lets program in python!");
        PythonExecutor pe = new PythonExecutor();

        string p = @"E:\Programs\Python\python311.dll";
        pe.SetPython(p);

        string funcName = "userCode";
        List<string> paramNames = new List<string>();
        Dictionary<string, object> varValues = new Dictionary<string, object>();

        paramNames.Add("nm");
        paramNames.Add("snm");
        paramNames.Add("age");

        varValues.Add("name", "Tomas");
        varValues.Add("surname", "Blby");
        varValues.Add("age", 13);

        
        string code = "import sys\nprint(\"test test test\")\nreturn [nm, snm, age]";
        string prepCode = pe.CreateCode(funcName, paramNames, varValues.Keys.ToList(), code);
        Console.WriteLine(prepCode);

        pe.Initialize();

        Person ps = new Person();
        bool callFunc = pe.RunCode(prepCode, varValues, ps, new StreamWriter(Console.OpenStandardOutput()));
        if (callFunc)
            Console.WriteLine(ps.ToString());
        else
        {
            Console.WriteLine(pe.ErrorMsg);

            p = @"D:\Instalace\VS\Shared\Python37_64\python37.dll";
            pe.SetPython(p);
            callFunc = pe.RunCode(prepCode, varValues, ps);
            Console.WriteLine(ps.ToString());
        }

        pe.Shutdown();
    }
}