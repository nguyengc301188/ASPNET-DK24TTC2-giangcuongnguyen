using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLSanBongMini.Data;
using QLSanBongMini.Models;

namespace QLSanBongMini.Controllers
{
    public class BookingsController : Controller
    {
        private QLSanBongMiniContext db = new QLSanBongMiniContext();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Field).Include(b => b.User);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            ViewBag.FieldID = new SelectList(db.Fields, "FieldID", "FieldName");
            ViewBag.userID = new SelectList(db.Users, "userID", "FullName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,userID,FieldID,BookingDate,StartTime,EndTime,TotalPrice,Status")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FieldID = new SelectList(db.Fields, "FieldID", "FieldName", booking.FieldID);
            ViewBag.userID = new SelectList(db.Users, "userID", "FullName", booking.userID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.FieldID = new SelectList(db.Fields, "FieldID", "FieldName", booking.FieldID);
            ViewBag.userID = new SelectList(db.Users, "userID", "FullName", booking.userID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,userID,FieldID,BookingDate,StartTime,EndTime,TotalPrice,Status")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FieldID = new SelectList(db.Fields, "FieldID", "FieldName", booking.FieldID);
            ViewBag.userID = new SelectList(db.Users, "userID", "FullName", booking.userID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult CancelBooking(int id)
        {
            var booking = db.Bookings.Find(id);

            if (booking == null)
                return HttpNotFound();

            // Chỉ cho hủy khi chưa chơi
            if (booking.Status == 0 || booking.Status == 1)
            {
                booking.Status = 2;
                db.SaveChanges();
            }

            return RedirectToAction("BookingHistory");
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
