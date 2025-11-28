using QLSanBongMini.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLSanBongMini.Controllers
{
    public class FieldController : Controller
    {
        QLSanBongMiniEntities1 db = new QLSanBongMiniEntities1();

        // List sân
        public ActionResult Index()
        {
            var fields = db.Fields.ToList();
            return View(fields);
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        public ActionResult Create(Field f)
        {
            if (ModelState.IsValid)
            {
                db.Fields.Add(f);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(f);
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            var field = db.Fields.Find(id);
            if (field == null) return HttpNotFound();
            return View(field);
        }

        // POST: Edit
        [HttpPost]
        public ActionResult Edit(Field f)
        {
            if (ModelState.IsValid)
            {
                db.Entry(f).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(f);
        }

        // DELETE
        public ActionResult Delete(int id)
        {
            var f = db.Fields.Find(id);
            if (f == null) return HttpNotFound();

            db.Fields.Remove(f);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
