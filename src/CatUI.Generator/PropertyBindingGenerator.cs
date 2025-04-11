using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CatUI.Generator
{
    [Generator]
    public class PropertyBindingGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BindingSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not BindingSyntaxReceiver receiver)
            {
                return;
            }

            Console.WriteLine("TEST");

            foreach (ObjectCreationExpressionSyntax objectCreation in receiver.Candidates)
            {
                SemanticModel semanticModel = context.Compilation.GetSemanticModel(objectCreation.SyntaxTree);

                InitializerExpressionSyntax? initializer = objectCreation.Initializer;
                if (initializer == null)
                {
                    continue;
                }

                var typeSymbol = semanticModel.GetSymbolInfo(objectCreation.Type).Symbol as INamedTypeSymbol;

                //ensure it is Element or a subclass of Element
                INamedTypeSymbol? baseType = typeSymbol;
                bool isElementOrDerived = false;
                while (baseType != null)
                {
                    if (baseType.Name == "Element" &&
                        baseType.ContainingNamespace.ToDisplayString() == "CatUI.Elements")
                    {
                        isElementOrDerived = true;
                        break;
                    }

                    baseType = baseType.BaseType;
                }

                if (!isElementOrDerived)
                {
                    continue;
                }

                //the first element is the property, the second one is the expression (what is after the =)
                List<(string Prop, string Expr)> bindings = [];
                //get user assignments from the initializer
                IEnumerable<AssignmentExpressionSyntax> assignments =
                    initializer.Expressions.OfType<AssignmentExpressionSyntax>();

                foreach (AssignmentExpressionSyntax assignment in assignments)
                {
                    string left = assignment.Left.ToString();
                    ExpressionSyntax right = assignment.Right;

                    if (right is InvocationExpressionSyntax invoke &&
                        invoke.Expression.ToString().StartsWith("El.Bind"))
                    {
                        //get the ObservableProperty given as an argument to El.Bind 
                        ArgumentSyntax? arg = invoke.ArgumentList.Arguments.FirstOrDefault();
                        if (arg == null)
                        {
                            continue;
                        }

                        bindings.Add((left, arg.ToString()));
                    }
                }

                if (bindings.Count == 0)
                {
                    continue;
                }

                string filePath =
                    objectCreation.SyntaxTree.FilePath
                                  .Replace('\\', '_').Replace('/', '_').Replace('.', '_');
                int line = objectCreation.GetLocation().GetLineSpan().StartLinePosition.Line;
                string methodName = $"__Init_{Path.GetFileNameWithoutExtension(filePath)}_Line{line}";
                string className = typeSymbol!.Name;

                List<string> initStatements =
                    bindings
                        .Select(b => $"el.{b.Prop}Property.BindBidirectional({b.Expr});")
                        .ToList();
                initStatements.Insert(0, $"var el = ({className})e;");
                string initCode = string.Join("\n ", initStatements);

                string ns = typeSymbol.ContainingNamespace.ToDisplayString();

                // ReSharper disable once UseRawString
                string source = $@"
namespace {ns}
{{
    public partial class {className}
    {{
        private static readonly System.Action<Element> {methodName} = e =>
        {{
            {initCode}
        }};

        protected override void OnInitGenerated()
        {{
            this.Init = {methodName};
        }}
    }}
}}
";
                context.AddSource($"{className}_Init_Line{line}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private sealed class BindingSyntaxReceiver : ISyntaxReceiver
        {
            public List<ObjectCreationExpressionSyntax> Candidates { get; } = [];

            public void OnVisitSyntaxNode(SyntaxNode node)
            {
                if (node is ObjectCreationExpressionSyntax obj && obj.Initializer != null)
                {
                    Candidates.Add(obj);
                }
            }
        }
    }
}
