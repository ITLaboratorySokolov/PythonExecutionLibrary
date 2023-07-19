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
        var pe = new PythonExecutor();

        //var p = @"E:\Programs\Python\python311.dll"; 
        //var p = @"E:\Programs\Unity\2022.3.0f1\modules.json";
        //var p = @"""C:\Users\dom-dev\.nuget\packages\microsoft.windowsdesktop.app.ref\6.0.16\ref\net6.0\PresentationUI.dll""";
        var p = @"D:\Instalace\VS\Shared"; //\Python37_64\python37.dll";
        pe.SetPython(p);

        var funcName = "userCode";
        var paramNames = new List<string>();
        var varValues = new Dictionary<string, object>();

        paramNames.Add("nm");
        paramNames.Add("snm");
        paramNames.Add("age");

        varValues.Add("name", "Tomas");
        varValues.Add("surname", "Blby");
        varValues.Add("age", 13);

        
        var code = "import sys\n" +
                   "print(\"test test test\")\n" +
                   "return [nm, snm, age]";
        var prepCode = pe.CreateCode(funcName, paramNames, varValues.Keys.ToList(), code);
        Console.WriteLine(prepCode);

        try
        {
            pe.Initialize();

            var pers = new Person();
            pe.RunCode(prepCode, varValues, pers, new StreamWriter(Console.OpenStandardOutput()));
            Console.WriteLine(pers.ToString());
            pe.Shutdown();
        }
        catch (PythonException e)
        {
            Console.WriteLine(e.Message);
            pe.Shutdown();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if(e.InnerException != null)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }

        try
        {
            pe.Initialize();

            var ps = new Person();
            pe.RunCode(prepCode, varValues, ps, new StreamWriter(Console.OpenStandardOutput()));
            Console.WriteLine(ps.ToString());
            pe.Shutdown();
        }
        catch (PythonException e)
        {
            Console.WriteLine(e.Message);
            pe.Shutdown();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }

        try
        {
            pe.Initialize();

            var ps = new Person();
            pe.RunCode(prepCode, varValues, ps, new StreamWriter(Console.OpenStandardOutput()));
            Console.WriteLine(ps.ToString());
            pe.Shutdown();
        }
        catch (PythonException e)
        {
            Console.WriteLine(e.Message);
            pe.Shutdown();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }

        try
        {
            pe.Initialize();

            var ps = new Person();
            pe.RunCode(prepCode, varValues, ps, new StreamWriter(Console.OpenStandardOutput()));
            Console.WriteLine(ps.ToString());
            pe.Shutdown();
        }
        catch (PythonException e)
        {
            Console.WriteLine(e.Message);
            pe.Shutdown();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }

        try
        {
            pe.Initialize();

            var ps = new Person();
            pe.RunCode(prepCode, varValues, ps, new StreamWriter(Console.OpenStandardOutput()));
            Console.WriteLine(ps.ToString());
            pe.Shutdown();
        }
        catch (PythonException e)
        {
            Console.WriteLine(e.Message);
            pe.Shutdown();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
            {
                Console.WriteLine(e.InnerException.Message);
            }
        }
    }
}