﻿using System;

namespace Cormo.Injects
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter)]
    public abstract class QualifierAttribute : Attribute
    {
        public override string ToString()
        {
            var name = GetType().Name;
            if(name.EndsWith("Attribute"))
            {
                return name.Substring(0, name.Length - "Attribute".Length);
            }
            return name;
        }
    }
}