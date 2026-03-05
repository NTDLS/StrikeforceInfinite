using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace Si.Library.Compiler
{
    public static class SiRuntimeCompiler
    {
        public static Assembly CompileToAssembly(string sourceCode)
        {
            // Parse
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            // Give it a unique assembly name so you can compile multiple times
            var assemblyName = "Dynamic_" + Guid.NewGuid().ToString("N");

            // References: use currently loaded assemblies (simple and works well in apps)
            var references = GetTrustedReferences();

            // Compile options
            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithNullableContextOptions(NullableContextOptions.Enable)
            );

            // Emit to memory
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

        private static IEnumerable<MetadataReference> GetTrustedReferences()
        {
            // This is a pragmatic approach: reference what your app already loaded.
            // If you need more control, explicitly reference specific assemblies.
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>()
                .ToList();
        }
    }
}
