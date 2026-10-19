using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.Models
{
    public class StoreHomeViewModel
    {

        // Danh sách danh mục lấy trực tiếp từ bảng Category.
        public List<Category> Categories { get; set; }

        // Danh sách sản phẩm đang bán để Trang Chủ tiếp tục hiển thị.
        public List<ProductCardViewModel> Products { get; set; }

        // Mã danh mục người dùng đang chọn.
        // int? cho phép null = không lọc danh mục.
        public int? SelectedCategoryId { get; set; }

        // Từ khóa người dùng đang tìm kiếm.
        public string SearchTerm { get; set; }
    }
}