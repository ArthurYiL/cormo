﻿using System;
using Cormo.Injects;

namespace Cormo.Web.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CookieParamAttribute: QualifierAttribute
    {
        public string Name { get; private set; }
        public object Default { get; set; }

        public CookieParamAttribute()
        {
        }

        public CookieParamAttribute(string name)
        {
            Name = name;
        }
    }
}