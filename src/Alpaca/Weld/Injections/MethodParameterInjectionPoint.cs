using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alpaca.Injects;
using Alpaca.Utils;
using Alpaca.Weld.Components;
using Alpaca.Weld.Utils;

namespace Alpaca.Weld.Injections
{
    public class MethodParameterInjectionPoint : AbstractInjectionPoint
    {
        private readonly ParameterInfo _param;
        
        public MethodParameterInjectionPoint(IComponent declaringComponent, ParameterInfo paramInfo, QualifierAttribute[] qualifiers) 
            : base(declaringComponent, paramInfo.Member, paramInfo.ParameterType, qualifiers)
        {
            _param = paramInfo;
            IsConstructor = _param.Member is ConstructorInfo;
            
        }

        public bool IsConstructor { get; private set; }
        public int Position { get { return _param.Position; } }

        public override IWeldInjetionPoint TranslateGenericArguments(IComponent component, IDictionary<Type, Type> translations)
        {
            if (IsConstructor)
            {
                var ctor = (ConstructorInfo) _param.Member;
                ctor = GenericUtils.TranslateConstructorGenericArguments(ctor, translations);
                var param = ctor.GetParameters()[_param.Position];
                return new MethodParameterInjectionPoint(component, param, Qualifiers.ToArray());
            }
            else
            {
                var method = (MethodInfo)_param.Member;
                method = GenericUtils.TranslateMethodGenericArguments(method, translations);
                var param = method.GetParameters()[_param.Position];
                return new MethodParameterInjectionPoint(component, param, Qualifiers.ToArray());
            }
        }

        protected override InjectPlan BuildInjectPlan(IComponent component)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            // TODO prettify
            return _param.ToString();
        }
    }
}