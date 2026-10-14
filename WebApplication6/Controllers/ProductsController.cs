using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class ProductsController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "ProductID,ProductName,Description,CategoryID,Price,ImageUrl,StockQuantity,IsActive")] Product product,
            HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageUrl", "Chỉ chấp nhận file JPG, JPEG hoặc PNG.");
                }
                else if (imageFile.ContentLength > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageUrl", "Dung lượng ảnh tối đa là 2 MB.");
                }
                else
                {
                    product.ImageUrl = SaveImage(imageFile);
                }
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ProductID,ProductName,Description,CategoryID,Price,ImageUrl,StockQuantity,IsActive")] Product product,
            HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageUrl", "Chỉ chấp nhận file JPG, JPEG hoặc PNG.");
                }
                else if (imageFile.ContentLength > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageUrl", "Dung lượng ảnh tối đa là 2 MB.");
                }
                else
                {
                    product.ImageUrl = SaveImage(imageFile);
                }
            }
            else
            {
                // Nếu không chọn ảnh mới thì lấy lại ảnh cũ
                var oldProduct = db.Products
                    .AsNoTracking()
                    .FirstOrDefault(p => p.ProductID == product.ProductID);

                if (oldProduct != null)
                {
                    product.ImageUrl = oldProduct.ImageUrl;
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private string SaveImage(HttpPostedFileBase imageFile)
        {
            string originalFileName = Path.GetFileName(imageFile.FileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            string extension = Path.GetExtension(originalFileName).ToLower();

            string folder = Server.MapPath("~/Content/images");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = fileNameWithoutExtension + extension;
            string path = Path.Combine(folder, fileName);

            int count = 1;
            while (System.IO.File.Exists(path))
            {
                fileName = fileNameWithoutExtension + "-" + count + extension;
                path = Path.Combine(folder, fileName);
                count++;
            }

            imageFile.SaveAs(path);
            return fileName;
        }
    }
}
