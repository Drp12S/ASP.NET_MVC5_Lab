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

        public ActionResult Index(int? categoryId, string keyword, decimal? minPrice, decimal? maxPrice, string sortOrder, int page = 1)
        {
            // Quy định số sản phẩm mỗi trang.
            const int pageSize = 6;

            // Không cho page nhỏ hơn 1.
            if (page < 1)
            {
                page = 1;
            }

            // Bắt đầu từ các sản phẩm đang bán.
            var products = db.Products.Where(p => p.IsActive);

            // Lọc theo Category.
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }

            // Tìm kiếm.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                products = products.Where(p => p.ProductName.Contains(keyword) || p.Description.Contains(keyword));
            }

            // MỚI: lọc cận dưới của giá.
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            // MỚI: lọc cận trên của giá.
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }


            // Đếm SAU khi đã áp dụng Category + Search + Price Filter.
            int totalItems = products.Count();

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    // Phải sắp xếp trước khi Skip/Take.
                    products = products.OrderByDescending(p => p.ProductID);
                    break;
            }

            // Tính số trang cần có.
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Khi không có kết quả vẫn để trang hiện tại = 1.
            if (totalPages == 0)
            {
                totalPages = 1;
            }

            // Nếu URL truyền page quá lớn thì đưa về trang cuối.
            if (page > totalPages)
            {
                page = totalPages;
            }

            var model = new StoreHomeViewModel
            {
                Categories = db.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToList(),

                Products = products
                    // Bỏ qua sản phẩm của các trang trước.
                    .Skip((page - 1) * pageSize)

                    // Chỉ lấy đúng số sản phẩm của trang hiện tại.
                    .Take(pageSize)

                    .Select(p => new ProductCardViewModel
                    {
                        ProductID = p.ProductID,
                        NamePro = p.ProductName,
                        CategoryName = p.Category.CategoryName,
                        Price = p.Price,
                        ImagePro = p.ImageUrl,
                        StockQuantity = p.StockQuantity,
                    })
                    .ToList(),

                // Giữ lại toàn bộ điều kiện hiện tại.
                SelectedCategoryId = categoryId,
                SearchTerm = keyword,

                // MỚI: giữ lại khoảng giá để View hiển thị lại.
                MinPrice = minPrice,
                MaxPrice = maxPrice,

                TotalItems = totalItems,
                SortOrder = sortOrder,

                // Giữ nguyên thông tin phân trang.
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
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