using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Eid.Microservices.Core.Infrastructure;
using Eid.Microservices.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Eid.Microservices.Core.StartupTasks
{
    public class AnonymousControllerActionsStartupTask : IStartupTask
    {

        // TODO: ADD DI SERVICES ????
        public int Order => 0;

        public void Execute()
        {

            List<Assembly> allAssemblies = new List<Assembly>();
            string path = AppContext.BaseDirectory;

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                allAssemblies.Add(Assembly.LoadFile(dll));

            var anonymousControllerActionList = new List<List<AnonymousControllerActionList>>();

            //KOSTAS
            //TODO : FILTER BY CONSTRUCTOR /Parameter !!!!
            allAssemblies.ForEach(c =>
            {
                var list = c.GetTypes()
                    .Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => m.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any())
                    // .Where(p => p.GetParameters().FirstOrDefault()?.ParameterType == typeof(RbacAuthorizationServiceFilter))
                    .Select(x => new AnonymousControllerActionList()
                    {
                        Controller = x.DeclaringType.Name,
                        Action = x.Name,
                        ReturnType = x.ReturnType.Name,
                        Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")))
                    })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
                if (list.Count > 0)
                    anonymousControllerActionList.Add(list);
            });

            Singleton<List<AnonymousControllerActions>>.Instance = anonymousControllerActionList.SelectMany(x => x).ToList().ToLookup(k => k.Controller, x => x.Action)
                .Select(k =>
                    new AnonymousControllerActions
                    {
                        Controller = k.Key,
                        Actions = k.ToList()
                    }).ToList();

        }
    }


    public class AnonymousControllerActionList
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ReturnType { get; set; }
        public string Attributes { get; set; }
    }
}
