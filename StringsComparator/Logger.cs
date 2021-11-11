using System;
using System.Text;
using System.IO;

namespace StringsComparator
{
    class Logger
    {
        private static Logger _logger;
        private static StringBuilder _builder;
        public static Logger Log
        {
            get
            {
                if (_logger == null)
                    _logger = new Logger();
                return _logger;
            }
        }

        private Logger()
        {
            _builder = new StringBuilder();
        }

        public void Write(string str)
        {
            Console.Write(str);
            _builder.Append(str);
        }

        public void WriteLine(string str)
        {
            Console.WriteLine(str);
            _builder.Append(str).Append(Environment.NewLine);
        }

        public void Output(string path)
        {
            File.WriteAllText(path, _builder.ToString());
        }
    }
}
