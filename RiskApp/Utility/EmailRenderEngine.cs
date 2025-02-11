
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Utility
{
    /// <summary>
    /// A dummy class to store TempData as we don't want the email rendering to be affected by 
    /// the web context, and vice versa
    /// </summary>
    public class DummyDataProvider : Dictionary<string, object>, ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return this;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
    // stolen and hacked (a little) from https://stackoverflow.com/questions/33123998/razor-view-page-as-email-template
    // also a good article that explains how to embed images using similar concepts https://long2know.com/2017/08/rendering-and-emailing-embedded-razor-views-with-net-core/

    public class EmailRenderEngine
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EmailRenderEngine(IRazorViewEngine razorViewEngine, IHttpContextAccessor httpContextAccessor)
        {
            this.razorViewEngine = razorViewEngine;
            this.httpContextAccessor = httpContextAccessor;
        }

        public  async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            ViewEngineResult viewEngineResult = razorViewEngine.GetView(null, viewName, false);
            if (viewEngineResult.View == null)
            {
                throw new Exception("Could not find the View file. Searched locations:\r\n" + string.Join("\r\n", viewEngineResult.SearchedLocations));
            }

            IView view = viewEngineResult.View;
            var actionContext = new ActionContext(httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
    
            using var outputStringWriter = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                new TempDataDictionary(actionContext.HttpContext, new DummyDataProvider()),
                outputStringWriter,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return outputStringWriter.ToString();
        }
    }
}
