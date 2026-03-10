using Ae.Engine.Helpers;
using System;
using System.Collections.Generic;

namespace Ae.Engine.Compiler
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
            { "AeAIStateMachine", new ConstructorSignatures("AeEngine engine, AeSpriteInteractive owner", "engine, owner") },
            { "AeSpriteWeapon", new ConstructorSignatures("AeEngine engine, AeSprite owner, string assetKey", "engine, owner, assetKey") },
        };

        public static string Get(string? baseClassName, string className, string userCode, Type interfaceType, string friendlyName)
        {
            if (ConstructorSignaturesByBaseClass.TryGetValue(baseClassName ?? string.Empty, out var constructorSignature) == false)
            {
                //Default constructor signature if the base class is not found in the dictionary.
                constructorSignature = new("AeEngine engine, string assetKey", "engine, assetKey");
            }

            var imports = AeEmbeddedTextResource.Load("Compiler/Templates/DynamicCompileImports.txt");

            string? codeTemplate;

            if (string.IsNullOrEmpty(baseClassName))
            {
                codeTemplate = AeEmbeddedTextResource.Load("Compiler/Templates/SimpleClassWithoutBase.txt");
            }
            else
            {
                codeTemplate = AeEmbeddedTextResource.Load("Compiler/Templates/SimpleClassWithBase.txt");
            }

            return codeTemplate.Replace("[[imports]]", imports)
                    .Replace("[[className]]", className)
                    .Replace("[[userCode]]", userCode)
                    .Replace("[[friendlyName]]", friendlyName)
                    .Replace("[[interfaceType]]", interfaceType.FullName)
                    .Replace("[[baseClassName]]", baseClassName)
                    .Replace("[[constructorSignature]]", constructorSignature.Signature)
                    .Replace("[[constructorParameters]]", constructorSignature.Parameters)
                    .Replace("[[userCode]]", userCode);
        }
    }
}
