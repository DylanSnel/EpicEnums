﻿using System.Runtime.CompilerServices;

namespace EpicEnums.Tests;
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
