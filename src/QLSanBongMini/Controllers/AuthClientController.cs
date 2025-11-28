using QLSanBongMini.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace QLSanBongMini.Controllers
{
    public class AuthClientController : Controller
    {
        QLSanBongMiniEntities1 db = new QLSanBongMiniEntities1();

        // GET: Register
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                var exist = db.Users.FirstOrDefault(u => u.Phone == model.Phone);
                if (exist != null)
                {
                    ModelState.AddModelError("", "Số điện thoại đã đăng ký");
                    return View(model);
                }

                model.Role = 0; // user
                model.Status = true;
                db.Users.Add(model);
                db.SaveChanges();

                Session["UserID"] = model.userID;
                Session["FullName"] = model.FullName;
                Session["Role"] = model.Role;

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Login
        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string Phone, string Password)
        {
            // Kiểm tra xem input có rỗng không
            if (string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Vui lòng nhập số điện thoại và mật khẩu.";
                return View();
            }

            // Tìm user theo Phone và Password
            var user = db.Users.FirstOrDefault(u => u.Phone == Phone && u.PasswordHash == Password);

            if (user == null)
            {
                // Login thất bại
                //ViewBag.Error = "Sai số điện thoại hoặc mật khẩu!";
                TempData["Error"] = "Sai số điện thoại hoặc mật khẩu!";
                return View();
            }

            // Login thành công => lưu session
            Session["UserID"] = user.userID;
            Session["FullName"] = user.FullName;
            Session["Role"] = user.Role;

            // Thông báo chào mừng user
            TempData["LoginSuccess"] = $"Chào mừng {user.FullName}!";

            // Phân quyền
            if (user.Role == 1)
            {
                // ADMIN
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Role == 0)
            {
                // USER
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Role không hợp lệ
                ViewBag.Error = "Tài khoản không hợp lệ!";
                return View();
            }
        }


        public ActionResult Logout()
        {
            Session.Clear(); // xóa toàn bộ session
            Session.Abandon(); // huỷ session
            TempData["Message"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}
