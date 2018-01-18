using BookService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string uri = "http://" + Request.Url.Host + ':' + Request.Url.Port + "/api/books";
            using (HttpClient httpClient = new HttpClient())
            {
                Task<String> response = httpClient.GetStringAsync(uri);
                return View(Task.Factory.StartNew(() =>
                            JsonConvert.DeserializeObject<List<BookDTO>>(response.Result)).Result);
            }
        }

        public ActionResult IndexVue()
        {
            return View();
        }
    }
}
