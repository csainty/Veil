using System;
using System.IO;
using System.Reflection;
using Veil.Compiler;
using Veil.Parser;

namespace Veil
{
    /// <summary>
    /// Compiles templates for execution
    /// </summary>
    public class VeilEngine : IVeilEngine
    {
        private static readonly MethodInfo genericCompileMethod = typeof(VeilEngine).GetMethod("Compile");
        private readonly IVeilContext context;

        /// <summary>
        /// Creates a VeilEngine that does not support Includes/Partials/MasterPages.
        /// </summary>
        public VeilEngine()
            : this(new NullVeilContext())
        {
        }

        /// <summary>
        /// Creates a VeilEngine with an <see cref="IVeilContext"/> to enable support for Includes/Partials/MasterPages.
        /// </summary>
        public VeilEngine(IVeilContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Parses and compiles the specified template
        /// </summary>
        /// <typeparam name="T">The type of the model that will be passed to the template</typeparam>
        /// <param name="parserKey">Key of the <see cref="ITemplateParser"/> to use to parse the template.</param>
        /// <param name="templateContents">The contents of the template to compile</param>
        /// <returns>A compiled action ready to be executed as needed to render the template</returns>
        public Action<TextWriter, T> Compile<T>(string parserKey, TextReader templateContents)
        {
            if (String.IsNullOrEmpty(parserKey)) throw new ArgumentNullException("templateType");
            if (templateContents == null) throw new ArgumentNullException("templateContents");
            if (!VeilStaticConfiguration.IsParserRegistered(parserKey)) throw new ArgumentException("A parser for templateType '{0}' is not registered.".FormatInvariant(parserKey), "templateType");

            var parser = VeilStaticConfiguration.GetParserInstance(parserKey);
            var syntaxTree = parser.Parse(templateContents, typeof(T));
            return new VeilTemplateCompiler<T>(CreateIncludeParser(parserKey, context)).Compile(syntaxTree);
        }

        /// <summary>
        /// Parses and compiles the specified template when the model type is not known
        /// </summary>
        /// <param name="parserKey">Key of the <see cref="ITemplateParser"/> to use to parse the template.</param>
        /// <param name="templateContents">The contents of the template to compile</param>
        /// <param name="modelType">The type of the model that will be passed to the template</param>
        /// <returns>A compiled action that will cast the model before execution</returns>
        public Action<TextWriter, object> CompileNonGeneric(string parserKey, TextReader templateContents, Type modelType)
        {
            var typedCompileMethod = genericCompileMethod.MakeGenericMethod(modelType);
            var compiledTemplate = typedCompileMethod.Invoke(this, new object[] { parserKey, templateContents });
            var compiledTemplateType = compiledTemplate.GetType();

            // TODO: Optimize this without casting from object? Perhaps using Expressions
            var emitter = Emit<Action<TextWriter, object, object>>.NewDynamicMethod();
            emitter.LoadArgument(2);
            emitter.CastClass(compiledTemplateType);
            emitter.LoadArgument(0);
            emitter.LoadArgument(1);
            emitter.CastClass(modelType);
            emitter.Call(compiledTemplateType.GetMethod("Invoke"));
            emitter.Return();
            var del = emitter.CreateDelegate();
            return new Action<TextWriter, object>((w, m) => del(w, m, compiledTemplate));
        }

        private static Func<string, Type, SyntaxTreeNode> CreateIncludeParser(string templateType, IVeilContext context)
        {
            return (includeName, modelType) =>
            {
                var template = context.GetTemplateByName(includeName, templateType);
                if (template == null) throw new InvalidOperationException("Unable to load template '{0}' using parser '{1}'".FormatInvariant(includeName, templateType));
                return VeilStaticConfiguration.GetParserInstance(templateType).Parse(template, modelType);
            };
        }
    }
}