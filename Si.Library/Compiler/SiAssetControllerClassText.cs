namespace Si.Library.Compiler
{
    public static class SiAssetControllerClassText
    {
        public static string Get(string baseClassName, string assetControllerClassName, string controllerCode)
        {
            return @$"
                using SharpDX.Direct2D1;
                using SharpDX.Mathematics.Interop;
                using SharpDX;
                using Si.Engine.AI.Logistics;
                using Si.Engine.Sprite._Superclass._Root;
                using Si.Engine.Sprite.Enemy._Superclass;
                using Si.Engine.Sprite.Weapon;
                using Si.Engine.Sprite;
                using Si.Engine;
                using Si.Library.Mathematics;
                using Si.Library;
                using static Si.Library.SiConstants;
                using System.Drawing;
                using System.Linq;
                using System;

                public class {assetControllerClassName}(EngineCore engine, string assetKey) : {baseClassName}(engine, assetKey), Si.Library.Compiler.ISiRuntimeCompiled
                {{
                    public string GetControllerName() => ""{assetControllerClassName}"";

                    {controllerCode}
                }}";
        }
    }
}
