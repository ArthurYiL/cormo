using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alpaca.Contexts;
using Alpaca.Injects;
using Alpaca.Utils;
using Alpaca.Weld.Components;
using Alpaca.Weld.Utils;

namespace Alpaca.Weld.Injections
{
    public abstract class AbstractInjectionPoint : IWeldInjetionPoint
    {
        protected readonly bool IsCacheable;
        
        protected AbstractInjectionPoint(IComponent declaringComponent, MemberInfo member, Type type, QualifierAttribute[] qualifiers)
        {
            qualifiers = qualifiers.DefaultIfEmpty(DefaultAttribute.Instance).ToArray();

            DeclaringComponent = declaringComponent;
            Member = member;
            ComponentType = type;
            Qualifiers = qualifiers;
            IsCacheable = IsCacheableType(type);
            _lazyComponents = new Lazy<IComponent>(ResolveComponents);
            _lazyInjectPlan = new Lazy<InjectPlan>(()=> BuildInjectPlan(Component));
            _lazyGetValuePlan = new Lazy<BuildPlan>(BuildGetValuePlan);
        }

        private static bool IsCacheableType(Type type)
        {
            return !typeof(IInjectionPoint).IsAssignableFrom(type) && !typeof(IInstance<>).IsAssignableFrom(GenericUtils.OpenIfGeneric(type));
        }

        private BuildPlan BuildGetValuePlan()
        {
            var manager = DeclaringComponent.Manager;
            var component = Component;
            if (IsCacheable)
            {
                return CacheUtils.Cache(context => manager.GetReference(component, context));
            }

            return context => manager.GetInjectableReference(this, component, context);
        }

        public MemberInfo Member { get; private set; }
        public IComponent DeclaringComponent { get; private set; }
        public Type ComponentType { get; set; }
        public IEnumerable<QualifierAttribute> Qualifiers { get; set; }
        public abstract IWeldInjetionPoint TranslateGenericArguments(IComponent component, IDictionary<Type, Type> translations);
        protected abstract InjectPlan BuildInjectPlan(IComponent components);
        private readonly Lazy<InjectPlan> _lazyInjectPlan;
        private readonly Lazy<IComponent> _lazyComponents;
        private readonly Lazy<BuildPlan> _lazyGetValuePlan;

        public object GetValue(ICreationalContext context)
        {
            return _lazyGetValuePlan.Value(context);
        }

        private IComponent ResolveComponents()
        {
            return DeclaringComponent.Manager.GetComponent(this);
        }

        public IComponent Component
        {
            get { return _lazyComponents.Value; }    
        }

        public Type Scope
        {
            get { return Component.Scope; }
        }

        public void Inject(object target, ICreationalContext context)
        {
            _lazyInjectPlan.Value(target, context);
        }
    }
}