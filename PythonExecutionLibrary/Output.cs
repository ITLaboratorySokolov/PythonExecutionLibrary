using System;
using System.IO;

namespace ZCU.PythonExecutionLibrary
{
    public class Output
    {
        private readonly TextWriter _writer;
        
        public Output(TextWriter writer)
        {
            _writer = writer;
        }

        public void write(string str)
        {
            _writer.Write(str.Replace("\n", Environment.NewLine));
            _writer.Flush();
        }

        public void writelines(string[] str)
        {
            foreach (var line in str)
            {
                _writer.Write(line);
            }

            _writer.Flush();
        }

        public void flush()
        {
            _writer.Flush();
        }

        public void close()
        {
            _writer.Close();
        }
    }
}
