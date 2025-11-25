using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Eid.Microservices.Administration.Models;
using Eid.Microservices.Core.Options;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eid.Microservices.Administration.Services
{
    public class ServiceFilterResolverService : IServiceFilterResolverService
    {
        private readonly ApplicationCoreOptions _applicationCoreOptions;

        public ServiceFilterResolverService(IOptions<ApplicationCoreOptions> applicationCoreOptions) =>
            _applicationCoreOptions = applicationCoreOptions.Value;


        public List<AssemblyModel> RbacResolve()
        {
            List<Assembly> allAssemblies = new List<Assembly>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                allAssemblies.Add(Assembly.LoadFile(dll));

            var controllersActions = new List<List<ControllerAction>>();

            //KOSTAS
            //TODO : FILTER BY CONSTRUCTOR !!!!
            allAssemblies.ForEach(c =>
            {
                var list = c.GetTypes()
                    .Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => m.GetCustomAttributes(typeof(ServiceFilterAttribute), true).Any())
                    // .Where(p => p.GetParameters().FirstOrDefault()?.ParameterType == typeof(RbacAuthorizationServiceFilter))
                    .Select(x => new ControllerAction() {
                        Controller = x.DeclaringType.Name,
                        Action = x.Name,
                        ReturnType = x.ReturnType.Name,
                        Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")))
                    })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
                if (list.Count > 0)
                    controllersActions.Add(list);
            });

            return controllersActions.SelectMany(x => x).ToList().ToLookup(k => k.Controller, x => x.Controller + "." + x.Action)
                .Select(k => 
                    new AssemblyModel()
                    {
                        Parent = _applicationCoreOptions.ApplicationName,
                        Members = k.ToList()
                    }).ToList();
        }
    }
}
