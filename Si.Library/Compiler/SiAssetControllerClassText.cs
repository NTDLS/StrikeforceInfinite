namespace Si.Library.Compiler
{
    public static class SiAssetControllerClassText
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

        private static Dictionary<string, ConstructorSignatures> ConstructorSignaturesByBaseClass = new()
        {
            { "SpriteWeapon", new ConstructorSignatures("EngineCore engine, SpriteBase owner, string assetKey", "engine, owner, assetKey") },
        };

        public static string Get(string baseClassName, string assetControllerClassName, string controllerCode)
        {
            if (ConstructorSignaturesByBaseClass.TryGetValue(baseClassName, out var constructorSignatures) == false)
            {
                //Default constructor signature if the base class is not found in the dictionary.
                constructorSignatures = new("EngineCore engine, string assetKey", "engine, assetKey");
            }

            return @$"
                using NTDLS.Helpers;
                using SharpDX.Direct2D1;
                using SharpDX.Mathematics.Interop;
                using SharpDX;
                using Si.Engine.AI.Logistics;
                using Si.Engine.Sprite._Superclass.Animation;
                using Si.Engine.Sprite._Superclass.Interactive.Ship;
                using Si.Engine.Sprite._Superclass.Interactive;
                using Si.Engine.Sprite._Superclass.MenuItem;
                using Si.Engine.Sprite._Superclass.Munition;
                using Si.Engine.Sprite._Superclass._Root;
                using Si.Engine.Sprite._Superclass.TextBlock;
                using Si.Engine.Sprite._Superclass;
                using Si.Engine;
                using Si.Library.ExtensionMethods;
                using Si.Library.Mathematics;
                using Si.Library;
                using Si.Rendering;
                using static Si.Library.SiConstants;
                using System.Drawing;
                using System.Linq;
                using System;

                public class {assetControllerClassName}({constructorSignatures.Signature})
                    : {baseClassName}({constructorSignatures.Parameters}), Si.Library.Compiler.ISiRuntimeCompiled
                {{
                    public string GetControllerName() => ""{assetControllerClassName}"";

                    {controllerCode}
                }}";
        }
    }
}
