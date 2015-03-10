using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Veil.Mvc5
{
    internal class HttpFlushingTextWriter : TextWriter
    {
        private readonly TextWriter Writer;
        private readonly HttpResponseBase Response;

        public override Encoding Encoding { get { return Writer.Encoding; } }

        public override IFormatProvider FormatProvider { get { return Writer.FormatProvider; } }

        public HttpFlushingTextWriter(TextWriter wrappedWriter, HttpResponseBase response)
        {
            Writer = wrappedWriter;
            Response = response;
        }

        public override void Close()
        {
            Writer.Close();
        }

        protected override void Dispose(bool disposing)
        {
            Writer.Dispose();
        }

        public override void Flush()
        {
            Writer.Flush();
            Response.Flush();
        }

        public override async Task FlushAsync()
        {
            await Writer.FlushAsync();
            Response.Flush();
        }

        public override object InitializeLifetimeService()
        {
            return Writer.InitializeLifetimeService();
        }

        public override string NewLine
        {
            get { return Writer.NewLine; }
            set { Writer.NewLine = value; }
        }

        public override string ToString()
        {
            return Writer.ToString();
        }

        public override void Write(bool value)
        {
            Writer.Write(value);
        }

        public override void Write(char value)
        {
            Writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            Writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Writer.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            Writer.Write(value);
        }

        public override void Write(double value)
        {
            Writer.Write(value);
        }

        public override void Write(float value)
        {
            Writer.Write(value);
        }

        public override void Write(int value)
        {
            Writer.Write(value);
        }

        public override void Write(long value)
        {
            Writer.Write(value);
        }

        public override void Write(object value)
        {
            Writer.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            Writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Writer.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            Writer.Write(format, arg);
        }

        public override void Write(string value)
        {
            Writer.Write(value);
        }

        public override void Write(uint value)
        {
            Writer.Write(value);
        }

        public override void Write(ulong value)
        {
            Writer.Write(value);
        }

        public override Task WriteAsync(char value)
        {
            return Writer.WriteAsync(value);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Writer.WriteAsync(buffer, index, count);
        }

        public override Task WriteAsync(string value)
        {
            return Writer.WriteAsync(value);
        }

        public override void WriteLine()
        {
            Writer.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            Writer.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Writer.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            Writer.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Writer.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Writer.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Writer.WriteLine(format, arg);
        }

        public override void WriteLine(string value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(uint value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            Writer.WriteLine(value);
        }

        public override Task WriteLineAsync()
        {
            return Writer.WriteLineAsync();
        }

        public override Task WriteLineAsync(char value)
        {
            return Writer.WriteLineAsync(value);
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Writer.WriteLineAsync(buffer, index, count);
        }

        public override Task WriteLineAsync(string value)
        {
            return Writer.WriteLineAsync(value);
        }
    }
}