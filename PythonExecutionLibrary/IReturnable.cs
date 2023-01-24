using Python.Runtime;

namespace PythonExecutionLibrary
{
    /// <summary>
    /// Interface for a class that can be modified by python code
    /// </summary>
    public interface IReturnable
    {
        /// <summary>
        /// Set parameters through python code
        /// </summary>
        /// <param name="lst"> Python list with parameters </param>
        /// <returns> True if successful, false if not </returns>
        bool SetParameters(PyObject lst);
    }
}
