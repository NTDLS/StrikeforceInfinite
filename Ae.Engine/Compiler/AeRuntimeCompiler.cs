using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ae.Engine.Compiler
{
    /// <summary>
    /// Provides methods for compiling C# source code at runtime and generating assemblies, as well as utility functions
    /// for asset key conversion.
    /// </summary>
    /// <remarks>This class is intended for scenarios where dynamic compilation of C# code is required, such
    /// as plugin systems or runtime code generation. All members are static and thread-safe. The class does not
    /// maintain any internal state.</remarks>
    public static class AeRuntimeCompiler
    {
        /// <summary>
        /// Converts an asset key string to a valid class name by replacing invalid characters with underscores.
        /// </summary>
        /// <remarks>This method replaces slashes, periods, and spaces in the asset key with underscores
        /// to ensure the resulting class name is valid and does not contain illegal characters.</remarks>
        /// <param name="assetKey">The asset key to convert. Can be null; if null, an empty string is returned.</param>
        /// <returns>A string representing the class name derived from the asset key. Returns an empty string if the input is
        /// null.</returns>
        public static string AssetKeyToClassName(string? assetKey)
        {
            return assetKey?.Replace('/', '_').Replace('.', '_').Replace(' ', '_') ?? string.Empty;
        }

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

        /// <summary>
        /// Compiles the specified C# source code into a dynamic assembly and optionally loads it into the current
        /// application domain.
        /// </summary>
        /// <remarks>If compilation fails, error messages are logged using the provided <paramref
        /// name="writeLog"/> delegate, if any. The method does not return the compiled assembly; it only indicates
        /// success or failure and optionally loads the assembly.</remarks>
        /// <param name="assetKey">The unique key identifying the asset for which the assembly is being compiled. Used for logging and naming
        /// purposes.</param>
        /// <param name="sourceCode">The C# source code to compile into an assembly.</param>
        /// <param name="loadAssembly">A value indicating whether the compiled assembly should be loaded into the current application domain.
        /// Specify <see langword="true"/> to load the assembly; otherwise, <see langword="false"/>.</param>
        /// <param name="writeLog">An optional delegate used to log compilation errors. If provided, errors encountered during compilation are
        /// sent to this delegate.</param>
        /// <returns>A value indicating whether the compilation succeeded. <see langword="true"/> if the assembly was
        /// successfully compiled (and loaded if requested); otherwise, <see langword="false"/>.</returns>
        public static bool CompileToAssembly(string assetKey, string sourceCode, bool loadAssembly, WriteLogDelegate? writeLog = null)
        {
            var assetClassName = AssetKeyToClassName(assetKey);

            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var assemblyName = $"Ae{assetClassName}_" + Guid.NewGuid().ToString("N");
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

                foreach (var error in errors)
                {
                    writeLog?.Invoke(error, AeLoggingLevel.Error, assetKey);
                }

                return false;
            }

            peStream.Position = 0;
            if (loadAssembly)
            {
                Assembly.Load(peStream.ToArray());
                return true;
            }
            return true;
        }
    }
}
