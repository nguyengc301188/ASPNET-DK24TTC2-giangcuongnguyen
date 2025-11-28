using QLSanBongMini.Models;
using QLSanBongMini.Models.ViewModels;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLSanBongMini.Controllers
{
    public class AdminController : Controller
    {
        QLSanBongMiniEntities1 db = new QLSanBongMiniEntities1();

        // Kiểm tra admin đã đăng nhập
        private bool IsAdmin()
        {
            return Session["Role"] != null && (int)Session["Role"] == 1;
        }

        #region Dashboard
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var model = new DashboardViewModel
            {
                TotalFields = db.Fields.Count(),
                TotalBookings = db.Bookings.Count(),
                TotalUsers = db.Users.Count(u => u.Role == 0),
                TotalRevenue = db.Payments.Where(p => p.Status == 1)
                                          .Sum(p => (decimal?)p.Amount) ?? 0
            };

            return View(model);
        }
        #endregion

        #region Quản lý sân
        public ActionResult Fields()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            // Chỉ lấy sân đang hoạt động
            var fields = db.Fields.OrderBy(f => f.FieldID).ToList();
            return View(fields);
        }
        public ActionResult RestoreField(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");
            var field = db.Fields.Find(id);
            if (field == null) return HttpNotFound();
            field.Status = true;
            db.SaveChanges();
            return RedirectToAction("Fields");
        }

        public ActionResult CreateField()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateField(Field field)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "AuthClient");

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu admin không nhập hình, dùng hình mặc định
                    if (string.IsNullOrEmpty(field.ImageURL))
                    {
                        field.ImageURL = "/Content/FieldImages/default.jpg";
                    }

                    field.Status = true; // active
                    db.Fields.Add(field);
                    db.SaveChanges();

                    TempData["Success"] = "Thêm sân thành công!";
                    return RedirectToAction("Fields");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(field);
        }

        public ActionResult EditField(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var field = db.Fields.Find(id);
            if (field == null) return HttpNotFound();
            return View(field);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditField(Field field)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu URL trống, giữ hình mặc định
                    if (string.IsNullOrEmpty(field.ImageURL))
                        field.ImageURL = "/Content/FieldImages/default.jpg";

                    db.Entry(field).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["Success"] = "Cập nhật sân thành công!";
                    return RedirectToAction("Fields");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(field);
        }

        public ActionResult DeleteField(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var field = db.Fields.Find(id);
            if (field == null) return HttpNotFound();

            // Chuyển trạng thái sân thành không hoạt động (soft delete)
            field.Status = false;
            db.SaveChanges();

            return RedirectToAction("Fields");
        }


        #endregion

        #region Quản lý booking
        public ActionResult Bookings()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            // Load tất cả Booking kèm User và Field
            var bookingList = db.Bookings
                .Include("User")
                .Include("Field")
                .OrderByDescending(b => b.BookingDate)
                .ToList(); // load dữ liệu trước

            // Chuyển sang ViewModel trong C#
            var bookings = bookingList.Select(b => new BookingViewModel
            {
                BookingID = b.BookingID,
                UserFullName = b.User.FullName,
                FieldName = b.Field.FieldName,
                BookingDate = b.BookingDate,
                StartTime = b.StartTime.TimeOfDay,
                EndTime = b.EndTime.TimeOfDay,
                Status = b.Status,
                IsPaid = db.Payments.Any(p => p.BookingID == b.BookingID && p.Status == 1),
                AmountPaid = db.Payments
                                .Where(p => p.BookingID == b.BookingID && p.Status == 1)
                                .Select(p => (decimal?)p.Amount)
                                .FirstOrDefault() ?? 0
            }).ToList();

            return View(bookings);
        }


        public ActionResult AcceptBooking(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            booking.Status = 1; // accepted
            db.SaveChanges();
            return RedirectToAction("Bookings");
        }

        public ActionResult CancelBooking(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var booking = db.Bookings.Find(id);
            if (booking == null) return HttpNotFound();

            booking.Status = 2; // cancelled
            db.SaveChanges();
            return RedirectToAction("Bookings");
        }
        #endregion

        #region Quản lý user
        public ActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");
            var users = db.Users.Where(u => u.Role == 0).ToList();
            return View(users);
        }

        public ActionResult ToggleUserStatus(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            user.Status = !user.Status; // khóa/mở tài khoản
            db.SaveChanges();
            return RedirectToAction("Users");
        }
        public ActionResult MarkPaid(int bookingId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "AuthClient");

            var booking = db.Bookings.Find(bookingId);
            if (booking == null) return HttpNotFound();

            // Nếu chưa có payment thì thêm
            var existing = db.Payments.FirstOrDefault(p => p.BookingID == bookingId && p.Status == 1);

            if (existing == null)
            {
                var payment = new Payment
                {
                    BookingID = bookingId,
                    Amount = booking.TotalPrice,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Tiền mặt",
                    Status = 1 // đã thanh toán
                };

                db.Payments.Add(payment);
            }

            // Cập nhật Booking.IsPaid = true
            booking.IsPaid = true;
            booking.AmountPaid = booking.TotalPrice;

            db.SaveChanges();

            TempData["PaidSuccess"] = "Đã thanh toán thành công!";
            return RedirectToAction("Bookings");
        }


        #endregion
    }
}
