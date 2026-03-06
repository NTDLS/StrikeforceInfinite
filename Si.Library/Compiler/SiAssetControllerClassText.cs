namespace Si.Library.Compiler
{
    public static class SiAssetControllerClassText
    {
        public static string Get(string baseClassName, string assetControllerClassName, string controllerCode)
        {
            return @$"
                using Si.Engine.Sprite.Weapon; //We will be deleteing this once its all moved to the asset pack.
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


                public class {assetControllerClassName}(EngineCore engine, string assetKey) : {baseClassName}(engine, assetKey), Si.Library.Compiler.ISiRuntimeCompiled
                {{
                    public string GetControllerName() => ""{assetControllerClassName}"";

                    {controllerCode}
                }}";
        }
    }
}
