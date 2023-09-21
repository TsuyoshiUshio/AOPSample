using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class FactoryGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Debugger.Launch();
            var source = context.SyntaxProvider.ForAttributeWithMetadataName(
                "AOPLib.FunctionAttribute",
                (node, token) => true,
                (ctx, token) => ctx
                )
                .Select((s, token) => { 
                    var typeSymbol = (INamedTypeSymbol)s.TargetSymbol;
                    var fullType = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    var isLogging = (bool)(s.Attributes.FirstOrDefault()?.ConstructorArguments.FirstOrDefault().Value ?? false);
                    return new FunctionMetadata { FullyQualifiedName = fullType, Name = name, IsLogging = isLogging };
            }).Collect();

            context.RegisterSourceOutput(source, Emit);
        }

        private void Emit(SourceProductionContext context, ImmutableArray<FunctionMetadata> source)
        {
            // Create Factory
            context.AddSource("FunctionFactory.g.cs", SourceText.From($@"
namespace AOPLib{{
  public class FunctionFactory {{
    public static IFunction CreateFunction(string name) {{
          switch(name) {{
            {GetFunctionsSection(source)}
            default:
              return new SampleFunction();
          }}
    }}
  }}
}}
", Encoding.UTF8));
        }

        private string GetFunctionsSection(ImmutableArray<FunctionMetadata> metadata)
        {
            var sb = new StringBuilder();
            foreach (var item in metadata)
            {
                sb.AppendLine($"case \"{item.Name}\":");
                if (item.IsLogging)
                {
                    sb.AppendLine($"return new LoggingAdapter(new {item.FullyQualifiedName}());");
                }
                else
                {
                    sb.AppendLine($"return new {item.FullyQualifiedName}();");
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
