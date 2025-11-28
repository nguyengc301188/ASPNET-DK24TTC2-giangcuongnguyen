using QLSanBongMini.Data;
using QLSanBongMini.Models;
using QLSanBongMini.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QLSanBongMini.Controllers
{
    public class UsersController : Controller
    {
        private QLSanBongMiniContext db = new QLSanBongMiniContext();

        public ActionResult BookingHistory()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "AuthClient");
            }

            int userId = (int)Session["UserID"];

            var history = db.Bookings
                            .Include("Field")
                            .Where(b => b.userID == userId)
                            .OrderByDescending(b => b.BookingDate)
                            .ToList();

            return View(history);
        }


    }
}
