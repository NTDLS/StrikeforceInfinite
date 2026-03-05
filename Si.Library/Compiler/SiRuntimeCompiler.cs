using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Si.Library.Compiler
{
    public static class SiRuntimeCompiler
    {
        private static IEnumerable<MetadataReference> GetTrustedReferences()
        {
            // Reference what we've already loaded.
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>()
                .ToList();

            //Add other crap we know we'll need that might not be loaded yet.
            var additionalReferences = new Type[]
            {
                typeof(NTDLS.Helpers.Converters),
            };

            foreach (var reference in additionalReferences)
            {
                if (!string.IsNullOrWhiteSpace(reference.Assembly.Location)
                    && !references.OfType<PortableExecutableReference>().Any(r => r.FilePath == reference.Assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(reference.Assembly.Location));
                }
            }

            return references;
        }

        public static Assembly CompileToAssembly(string sourceCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var assemblyName = "Dynamic_" + Guid.NewGuid().ToString("N");
            var references = GetTrustedReferences();

            var compilation = CSharpCompilation.Create(
                assemblyName,
                [syntaxTree],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithNullableContextOptions(NullableContextOptions.Enable)
            );

            using var peStream = new MemoryStream();
            var emitResult = compilation.Emit(peStream);

            if (!emitResult.Success)
            {
                var errors = emitResult.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString());

                throw new Exception("Compilation failed:\n" + string.Join("\n", errors));
            }

            peStream.Position = 0;
            return Assembly.Load(peStream.ToArray());
        }
    }
}
