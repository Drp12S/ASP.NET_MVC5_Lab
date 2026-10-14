using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class StoreController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index()
        {
            var products = db.Products 
                .Where(p => p.IsActive == true)
                .Select(p => new ProductCardViewModel
                {
                    ProductID = p.ProductID,
                    NamePro = p.ProductName,
                    CategoryName = p.Category.CategoryName,
                    Price = p.Price,
                    ImagePro = p.ImageUrl
                })
                .ToList();

            return View(products);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}