using DotNetCachingDemo.Models;
using System.Web.Mvc;
using System.Runtime.Caching;
using System;
using System.Collections.Generic;

namespace DotNetCachingDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CacheItemPolicy policy; // Política del objeto 'cache'.
            IList<string> filePaths; // Rutas de los archivos de texto
            string cachedFilePath; // Ruta del archivo en 'cache'.
            ObjectCache cache = MemoryCache.Default; // Objeto 'cache'.

            if (!(cache["filecontents"] is string fileContents))
            {
                policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(10.0),
                };

                filePaths = new List<string>();
                cachedFilePath = Server.MapPath("~\\App_Data") + "\\SomeTextFile.txt";
                filePaths.Add(cachedFilePath);

                policy.ChangeMonitors.Add(new HostFileChangeMonitor(filePaths)); // Patrón 'Observer'

                // Añado la fecha al contenido del archivo para verificar en el ejemplo
                // que el objeto 'cache' está funcionando correctamente.
                // Antes de los 10 segundos asignados a la política de expiración la fecha no
                // cambiará.
                fileContents = String.Format("Content: {0} Time: {1}", System.IO.File.ReadAllText(cachedFilePath), DateTime.Now.ToString());

                cache.Set("filecontents", fileContents, policy);
            }

            Article article = new Article()
            {
                Title = "Something Awesome",
                Content = fileContents
            };

            return View(article);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}