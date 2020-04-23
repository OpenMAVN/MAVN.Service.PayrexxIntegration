﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MAVN.Service.PayrexxIntegration.Client;
using Refit;
using Xunit;

namespace MAVN.Service.PayrexxIntegration.Tests
{
    public class SensitiveDataTests
    {
        private readonly Type _refitGetAttrType = typeof(GetAttribute);

        private readonly List<string> _sensitiveParamsNames = new List<string>
        {
            "name", "email", "phone", "login"
        };

        [Fact]
        public void CheckSensitiveParamsTest()
        {
            var clientInterface = typeof(IPayrexxIntegrationClient);

            var apiInterfaces = clientInterface
                .GetProperties()
                .Where(p => p.CanRead && p.PropertyType.IsInterface)
                .Select(p => p.PropertyType)
                .ToHashSet();

            var sensitiveDataParams = new List<string>();

            foreach (var apiInterface in apiInterfaces)
            {
                var interfaceMethods = apiInterface.GetMethods(
                    BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var apiMethod in interfaceMethods)
                {
                    var refitGetAttr = apiMethod.CustomAttributes.FirstOrDefault(a => _refitGetAttrType == a.AttributeType);
                    if (refitGetAttr == null)
                        continue;

                    var methodParams = apiMethod.GetParameters();
                    var paramsWithSensitiveData = methodParams.Where(p => _sensitiveParamsNames.Any(s => p.Name.ToLower().Contains(s)));
                    sensitiveDataParams.AddRange(
                        paramsWithSensitiveData.Select(i => $"{i.Name} from {apiInterface.Name}.{apiMethod.Name}"));
                }
            }

            Assert.True(
                sensitiveDataParams.Count == 0,
                "These parameters might lead to exposing sensitive data when building url via refit: " + string.Join(", ", sensitiveDataParams));
        }
    }
}
