namespace Ae.Library.Compiler
{
    public static class AeAssetCodeClassText
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
            { "AIStateMachine", new ConstructorSignatures("AeEngine engine, SpriteInteractive owner, List<SpriteBase> ? observedObjects = null", "engine, owner, observedObjects") },
        };

        public static string Get(string? baseClassName, string assetControllerClassName, string controllerCode)
        {
            if (ConstructorSignaturesByBaseClass.TryGetValue(baseClassName ?? string.Empty, out var constructorSignatures) == false)
            {
                //Default constructor signature if the base class is not found in the dictionary.
                constructorSignatures = new("AeEngine engine, string assetKey", "engine, assetKey");
            }

            var usings = @$"
                using System.Collections.Generic;
                using NTDLS.Helpers;
                using SharpDX.Direct2D1;
                using SharpDX.Mathematics.Interop;
                using SharpDX;
                using Ae.Engine.Sprite._Superclass.Animation;
                using Ae.Engine.Sprite._Superclass.Interactive.Ship;
                using Ae.Engine.Sprite._Superclass.Interactive;
                using Ae.Engine.Sprite._Superclass.MenuItem;
                using Ae.Engine.Sprite._Superclass.Munition;
                using Ae.Engine.Sprite._Superclass._Root;
                using Ae.Engine.Sprite._Superclass.TextBlock;
                using Ae.Engine.Sprite._Superclass;
                using Ae.Engine;
                using Ae.Library.ExtensionMethods;
                using Ae.Library.Mathematics;
                using Ae.Library;
                using Ae.Rendering;
                using static Ae.Library.AeConstants;
                using System.Drawing;
                using System.Linq;
                using Ae.Engine.AI;
                using System;" + Environment.NewLine;

            if (string.IsNullOrEmpty(baseClassName))
            {
                return usings + $@"public class {assetControllerClassName}(AeEngine engine, string assetKey)
                    : Ae.Library.Compiler.IAeRuntimeCompiled
                {{
                    public string GetControllerName() => ""{assetControllerClassName}"";

                    {controllerCode}
                }}";
            }

            return usings + $@"public class {assetControllerClassName}({constructorSignatures.Signature})
                    : {baseClassName}({constructorSignatures.Parameters}), Ae.Library.Compiler.IAeRuntimeCompiled
                {{
                    public string GetControllerName() => ""{assetControllerClassName}"";

                    {controllerCode}
                }}";
        }
    }
}
