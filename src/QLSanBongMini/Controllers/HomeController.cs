using QLSanBongMini.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QLSanBongMini.Controllers
{
    public class HomeController : Controller
    {
        QLSanBongMiniEntities1 db = new QLSanBongMiniEntities1();

        // Trang chủ hiển thị 3 sân gần đây
        public ActionResult Index()
        {
            var fields = db.Fields
                           .Where(f => f.Status == true)
                           .OrderByDescending(f => f.FieldID)
                           .Take(3)
                           .ToList();
            return View(fields);
        }


        public ActionResult ViewField(int id)
        {
            var field = db.Fields.Find(id);
            if (field == null) return HttpNotFound();
            return View(field);
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult BookingHistory()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "AuthClient");

            int userId = (int)Session["UserID"];

            var history = db.Bookings
                .Include("Field")
                .Where(b => b.userID == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(history);
        }
        public ActionResult CancelBooking(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            if (booking.Status != 0)
            {
                TempData["Error"] = "Chỉ được hủy khi đang chờ duyệt!";
                return RedirectToAction("BookingHistory");
            }

            booking.Status = 2; // cancelled
            db.SaveChanges();

            TempData["Success"] = "Hủy đặt sân thành công!";
            return RedirectToAction("BookingHistory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookField(int fieldID, DateTime date, DateTime startTime, DateTime endTime)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "AuthClient");

            var field = db.Fields.Find(fieldID);
            if (field == null) return HttpNotFound();

            // Kiểm tra trùng giờ (chi tiết ở bước 3)
            var conflict = db.Bookings.Any(b => b.FieldID == fieldID
                                             && b.BookingDate == date.Date
                                             && b.Status != 2 // không tính cancelled
                                             && ((startTime >= b.StartTime && startTime < b.EndTime)
                                              || (endTime > b.StartTime && endTime <= b.EndTime)
                                              || (startTime <= b.StartTime && endTime >= b.EndTime)));
            if (conflict)
            {
                TempData["Error"] = "Thời gian bạn chọn đã bị trùng, vui lòng chọn khung giờ khác!";
                return RedirectToAction("ViewField", new { id = fieldID });
            }

            Booking booking = new Booking
            {
                FieldID = fieldID,
                userID = (int)Session["UserID"],
                BookingDate = date.Date,
                StartTime = startTime,
                EndTime = endTime,
                TotalPrice = (decimal)(endTime - startTime).TotalHours * field.PricePerHour,
                Status = 0 // pending
            };

            db.Bookings.Add(booking);
            db.SaveChanges();

            TempData["Success"] = "Đặt sân thành công, chờ admin duyệt!";
            return RedirectToAction("ViewField", new { id = fieldID });
        }

    }
}
