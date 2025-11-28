using QLSanBongMini.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLSanBongMini.Controllers
{
    public class AuthController : Controller
    {
        QLSanBongMiniEntities1 db = new QLSanBongMiniEntities1();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string phone, string password)
        {
            var user = db.Users
                .Where(u => u.Phone == phone && u.PasswordHash == password)
                .FirstOrDefault();

            if (user != null)
            {
                Session["UserID"] = user.userID;
                Session["FullName"] = user.FullName;
                Session["Role"] = user.Role;
                if (user.Role == 1)
                    Session["Admin"] = true;
                return RedirectToAction("Index", user.Role == 1 ? "Admin" : "Home");
            }

            ViewBag.Error = "Sai số điện thoại hoặc mật khẩu!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
