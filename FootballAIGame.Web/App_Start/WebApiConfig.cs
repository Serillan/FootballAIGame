using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using FootballAIGame.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace FootballAIGame.Web
{
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers web api configuration.
        /// </summary>
        /// <param name="config">The configuration object, which we use to
        /// set configurations.</param>
        public static void Register(HttpConfiguration config)
        {
            // set camel case for json objects
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
            settings.Converters.Add(new CustomDateTimeConverter());

            // configure routing
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                    name: "ActionWithoutIdApi",
                    routeTemplate: "api/{controller}/{action}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }

    }
}
