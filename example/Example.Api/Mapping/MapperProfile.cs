﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Example.Api.Mapping
{
    /// <summary>
    /// The mapping profile.
    /// </summary>
    public class MappingProfile : Profile
    {
        private readonly Type _mappingType = typeof(IMapFrom<>);
        private readonly string _mappingMethodName = nameof(IMapFrom<object>.Mapping);

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public MappingProfile(Assembly assembly)
        {
            ApplyMappingsFromAssembly(assembly);
        }

        /// <summary>
        /// Applies the mappings from assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                var methods = GetMappingMethods(type);

                if (methods.Count > 0)
                {
                    RemoveInheritedMappingMethods(methods, type.BaseType);

                    if (methods.Count > 0)
                    {
                        var instance = Activator.CreateInstance(type);

                        foreach (var method in methods)
                        {
                            method.Invoke(instance, new object[] { this });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the inherited mapping methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="baseType">The base type.</param>
        private void RemoveInheritedMappingMethods(List<MethodInfo> methods, Type? baseType)
        {
            while (baseType != null)
            {
                foreach (var method in GetMappingMethods(baseType))
                {
                    methods.Remove(method);
                }

                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// Gets the mapping methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A list of MethodInfos.</returns>
        private List<MethodInfo> GetMappingMethods(Type type)
        {
            return type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == _mappingType)
                .Select(x => x.GetMethod(_mappingMethodName)!)
                .ToList();
        }
    }
}
