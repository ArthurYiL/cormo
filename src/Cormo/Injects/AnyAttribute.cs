﻿namespace Cormo.Injects
{
    public sealed class AnyAttribute: QualifierAttribute
    { 
        public static readonly AnyAttribute Instance = new AnyAttribute();
    }
}