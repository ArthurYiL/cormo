﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cormo.Impl.Weld.Components;

namespace Cormo.Impl.Weld.Resolutions
{
    public class ComponentResolver : TypeSafeResolver<IWeldComponent, ComponentResolvable>
    {
        private readonly ConcurrentDictionary<Type, IWeldComponent[]> _typeComponents = new ConcurrentDictionary<Type, IWeldComponent[]>();

        public ComponentResolver(WeldComponentManager manager, IEnumerable<IWeldComponent> allComponents) 
            : base(manager, allComponents)
        {
        }

        protected override IEnumerable<IWeldComponent> Resolve(ComponentResolvable resolvable, ref IEnumerable<IWeldComponent> components)
        {
            var results = components.ToArray();
            var unwrappedType = UnwrapType(resolvable.Type);
            var isWrapped = unwrappedType != resolvable.Type;

            results = _typeComponents.GetOrAdd(unwrappedType, t =>
                results.Select(x => x.Resolve(t)).Where(x => x != null).ToArray());

            results = results.Where(c => c.Qualifiers.CanSatisfy(resolvable.Qualifiers)).ToArray();

            if (results.Length > 1)
            {
                var onMissings = results.Where(x => x.IsConditionalOnMissing).ToArray();
                var others = results.Except(onMissings).ToArray();

                results = others.Any() ? others : onMissings.Take(1).ToArray();
            }

            foreach (var result in results)
                result.Touch();
            components = results;
            
            return isWrapped
                ? new IWeldComponent[] {new InstanceComponent(unwrappedType, new Binders(resolvable.Qualifiers), Manager, results)}
                : results;
        }
    }
}