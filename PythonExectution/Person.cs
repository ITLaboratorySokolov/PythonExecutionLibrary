using Python.Runtime;
using ZCU.PythonExecutionLibrary;
using System.Globalization;

namespace PythonExectution
{
    internal class Person : IReturnable
    {
        public string name;
        public string surname;
        public int age;
        private CultureInfo baseCulture = new CultureInfo("en-US"); 

        public bool SetParameters(PyObject obj)
        {
            try
            {
                name = obj[0].ToString();
                surname = obj[1].ToString();
                age = obj[2].ToInt32(baseCulture);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                return false; 
            }

            return true;
        }

        public override string ToString()
        {
            return name + " " + surname + ", " + age;
        }
    }
}
