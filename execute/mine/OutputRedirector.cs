using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// credit to eyossi:
// http://stackoverflow.com/a/10918320
namespace POSH_sharp.executing
{
    public class OutputRedirector : TextWriter
    {
        private TextWriter m_OriginalConsoleStream;
        private FileStream fs;
        private StreamWriter sw;

        public OutputRedirector(TextWriter consoleTextWriter)
        {

            fs = new FileStream("Test.txt", FileMode.Create);

            sw = new StreamWriter(fs);
            sw.AutoFlush = true;

            m_OriginalConsoleStream = consoleTextWriter;
        }

        public override void WriteLine(string value)
        {
            m_OriginalConsoleStream.WriteLine(value);

            // Fire event here with value
            sw.WriteLine(value);
        }


        public static void SetToConsole()
        {
            Console.SetOut(new OutputRedirector(Console.Out));
        }

        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }
    }
}
