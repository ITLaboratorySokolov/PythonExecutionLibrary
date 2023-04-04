using ZCU.PythonExecutionLibrary;

namespace PythonExectution;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World! Lets program in python!");
        PythonExecutor pe = new PythonExecutor();

        string p = @"D:\Instalace\VS\Shared\Python37_64\python37.dll";
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

        string code = "return [nm, snm, age]";
        string prepCode = pe.CreateCode(funcName, paramNames, varValues, code);
        Console.WriteLine(prepCode);

        Person ps = new Person();
        bool callFunc = pe.RunCode(prepCode, varValues, ps);
        Console.WriteLine(ps.ToString());
    }
}