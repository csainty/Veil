using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Web.Razor;

namespace Veil.Benchmark
{
    internal static class Razor
    {
        public static Action<TextWriter, T> Compile<T>(string template)
        {
            return Compile<T>(new StringReader(template));
        }

        public static Action<TextWriter, T> Compile<T>(TextReader template)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = string.Format("Veil.Benchmark.RazorBase<{0}>", typeof(T) == typeof(object) ? "dynamic" : typeof(T).FullName);
            host.DefaultNamespace = "VeilRazor";
            host.DefaultClassName = "Template";

            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.IO");
            host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            var engine = new RazorTemplateEngine(host);
            var genResult = engine.GenerateCode(template);
            using (var provider = new Microsoft.CSharp.CSharpCodeProvider())
            {
                var parameters = new CompilerParameters(new[] {
                    GetAssemblyPath(typeof(T)),
                    GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder)),
                    GetAssemblyPath(typeof(RazorBase<>)),
                    GetAssemblyPath(typeof(INotifyPropertyChanged)),
                    GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite))
                });
                parameters.GenerateInMemory = true;

                var compileResult = provider.CompileAssemblyFromDom(parameters, genResult.GeneratedCode);

                if (compileResult.Errors.HasErrors) throw new InvalidOperationException(compileResult.Errors[0].ErrorText);

                var viewType = compileResult.CompiledAssembly.GetType("VeilRazor.Template");
                var view = Expression.Variable(viewType, "view");
                var init = viewType.GetMethod("Initialize");
                var exec = viewType.GetMethod("Execute");
                var writer = Expression.Parameter(typeof(TextWriter));
                var model = Expression.Parameter(typeof(T));

                return Expression.Lambda<Action<TextWriter, T>>(Expression.Block(
                    new[] { view },
                    new Expression[] {
                        Expression.Assign(view, Expression.New(viewType)),
                        Expression.Call(view, init, writer, model),
                        Expression.Call(view, exec)
                    }
                ), writer, model).Compile();
            }
        }

        private static string GetAssemblyPath(Type type)
        {
            return GetAssemblyPath(type.Assembly);
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }
    }

    public abstract class RazorBase<TModel> : IView
    {
        private TextWriter writer;

        public abstract void Execute();

        public TModel Model { get; private set; }

        public virtual void Initialize(TextWriter writer, object model)
        {
            this.Model = (TModel)model;
            this.writer = writer;
        }

        public virtual void WriteLiteral(object value)
        {
            writer.Write(value);
        }

        public virtual void Write(object value)
        {
            HtmlEncode(value);
        }

        private void HtmlEncode(object value)
        {
            if (value == null)
            {
                return;
            }

            WebUtility.HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture), this.writer);
        }
    }

    public interface IView
    {
        void Initialize(TextWriter writer, object model);

        void Execute();
    }
}