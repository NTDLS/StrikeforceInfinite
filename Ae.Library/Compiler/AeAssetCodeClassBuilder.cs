namespace Ae.Library.Compiler
{
    public static class AeAssetCodeClassBuilder
    {
        private class ConstructorSignatures
        {
            public string Signature { get; set; }
            public string Parameters { get; set; }

            public ConstructorSignatures(string signature, string parameters)
            {
                Signature = signature;
                Parameters = parameters;
            }
        }

        private readonly static Dictionary<string, ConstructorSignatures> ConstructorSignaturesByBaseClass = new()
        {
            { "AIStateMachine", new ConstructorSignatures("AeEngine engine, SpriteInteractive owner", "engine, owner") },
            { "SpriteWeapon", new ConstructorSignatures("AeEngine engine, SpriteBase owner, string assetKey", "engine, owner, assetKey") },
        };

        public static string Get(string? baseClassName, string className, string userCode)
        {
            if (ConstructorSignaturesByBaseClass.TryGetValue(baseClassName ?? string.Empty, out var constructorSignature) == false)
            {
                //Default constructor signature if the base class is not found in the dictionary.
                constructorSignature = new("AeEngine engine, string assetKey", "engine, assetKey");
            }

            var imports = EmbeddedResource.Load("Compiler/Templates/DynamicCompileImports.cs");

            string? codeTemplate;

            if (string.IsNullOrEmpty(baseClassName))
            {
                codeTemplate = EmbeddedResource.Load("Compiler/Templates/SimpleClassWithoutBase.cs");
            }
            else
            {
                codeTemplate = EmbeddedResource.Load("Compiler/Templates/SimpleClassWithBase.cs");
            }

            return codeTemplate.Replace("[[imports]]", imports)
                    .Replace("[[className]]", className)
                    .Replace("[[userCode]]", userCode)
                    .Replace("[[baseClassName]]", baseClassName)
                    .Replace("[[constructorSignature]]", constructorSignature.Signature)
                    .Replace("[[constructorParameters]]", constructorSignature.Parameters)
                    .Replace("[[userCode]]", userCode);
        }
    }
}
