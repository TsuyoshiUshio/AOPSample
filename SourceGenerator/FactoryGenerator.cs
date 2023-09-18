using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SourceGenerator
{
    [Generator]
    public class FactoryGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // Register the Class that has Function Attribute
            // If the Attribute says logging enabled, Register the class with LoggingAdapter
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("AOPLib.FunctionAttribute");
            var targetClasses = context.Compilation.SyntaxTrees
                .SelectMany(st => st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>());
            foreach(var clazz in targetClasses)
            {
                var name = clazz.Identifier.ValueText;
                var name2 = clazz.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.Name.NormalizeWhitespace().ToFullString();
            }

            var selectedTargetClasses = targetClasses.Where(p => p.AttributeLists.FirstOrDefault()?.Attributes.FirstOrDefault()?.Name.NormalizeWhitespace().ToFullString() == "Function");

            var registeredClasses = new Dictionary<string, FunctionMetadata>();
            // Next Step 1. Extract the classes with the Attribute is logging. Fully Qualified Name with isLogging property will help.
            foreach (var clazz in selectedTargetClasses)
            {
                var attribute = clazz.AttributeLists.First()?.Attributes.FirstOrDefault();
                var attributeArgument = attribute.ArgumentList.Arguments.FirstOrDefault();
                var isLogging = attributeArgument.Expression.NormalizeWhitespace().ToFullString().Contains("true");
                var typeSymbol = context.Compilation.GetSemanticModel(clazz.SyntaxTree).GetDeclaredSymbol(clazz);
                var className = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var classNameOnly = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                registeredClasses.Add(classNameOnly, new FunctionMetadata { FullyQualifiedName = className, Name = classNameOnly, IsLogging = isLogging });
            }


            // Create Factory
            context.AddSource("FunctionFactory.g.cs", SourceText.From($@"
namespace AOPLib{{
  public class FunctionFactory {{
    public static IFunction CreateFunction(string name) {{
          switch(name) {{
            {GetFunctionsSection(registeredClasses)}
            default:
              return new SampleFunction();
          }}
    }}
  }}
}}
", Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
        }

        private string GetFunctionsSection(Dictionary<string, FunctionMetadata> metadata)
        {
            var sb = new StringBuilder();
            foreach (var item in metadata)
            {
                sb.AppendLine($"case \"{item.Key}\":");
                if (item.Value.IsLogging)
                {
                    sb.AppendLine($"return new LoggingAdapter(new {item.Value.FullyQualifiedName}());");
                }
                else
                {
                    sb.AppendLine($"return new {item.Value.FullyQualifiedName}();");
                }
            }
            return sb.ToString();
        }
    }

    class FunctionMetadata
    {
        public string Name { get; set; }
        public string FullyQualifiedName { get; set; }
        public bool IsLogging { get; set; }
    }
}
