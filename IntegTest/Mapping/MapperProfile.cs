using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IntegTest.Mapping
{
    public class MappingProfile : Profile
    {
        private readonly Type _mappingType = typeof(IMapFrom<>);
        private readonly string _mappingMethodName = nameof(IMapFrom<object>.Mapping);

        public MappingProfile(Assembly assembly)
        {
            ApplyMappingsFromAssembly(assembly);
        }

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

        private List<MethodInfo> GetMappingMethods(Type type)
        {
            return type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == _mappingType)
                .Select(x => x.GetMethod(_mappingMethodName)!)
                .ToList();
        }
    }
}
