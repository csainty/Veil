using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Veil.Benchmark.GCBenchmarkCases
{
    public class NullTextWriter : TextWriter
    {
        private static readonly Task FinishedTask = Task.FromResult(0);

        public static readonly TextWriter Instance = new NullTextWriter();

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync()
        {
            return FinishedTask;
        }

        public override void Write(bool value)
        {
        }

        public override void Write(char value)
        {
        }

        public override void Write(char[] buffer)
        {
        }

        public override void Write(char[] buffer, int index, int count)
        {
        }

        public override void Write(decimal value)
        {
        }

        public override void Write(double value)
        {
        }

        public override void Write(float value)
        {
        }

        public override void Write(int value)
        {
        }

        public override void Write(long value)
        {
        }

        public override void Write(object value)
        {
        }

        public override void Write(string format, object arg0)
        {
        }

        public override void Write(string format, object arg0, object arg1)
        {
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
        }

        public override void Write(string format, params object[] arg)
        {
        }

        public override void Write(string value)
        {
        }

        public override void Write(uint value)
        {
        }

        public override void Write(ulong value)
        {
        }

        public override Task WriteAsync(char value)
        {
            return FinishedTask;
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return FinishedTask;
        }

        public override Task WriteAsync(string value)
        {
            return FinishedTask;
        }

        public override void WriteLine()
        {
        }

        public override void WriteLine(bool value)
        {
        }

        public override void WriteLine(char value)
        {
        }

        public override void WriteLine(char[] buffer)
        {
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
        }

        public override void WriteLine(decimal value)
        {
        }

        public override void WriteLine(double value)
        {
        }

        public override void WriteLine(float value)
        {
        }

        public override void WriteLine(int value)
        {
        }

        public override void WriteLine(long value)
        {
        }

        public override void WriteLine(object value)
        {
        }

        public override void WriteLine(string format, object arg0)
        {
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
        }

        public override void WriteLine(string format, params object[] arg)
        {
        }

        public override void WriteLine(string value)
        {
        }

        public override void WriteLine(uint value)
        {
        }

        public override void WriteLine(ulong value)
        {
        }

        public override Task WriteLineAsync()
        {
            return FinishedTask;
        }

        public override Task WriteLineAsync(char value)
        {
            return FinishedTask;
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return FinishedTask;
        }

        public override Task WriteLineAsync(string value)
        {
            return FinishedTask;
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}