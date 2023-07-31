using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class RoomSetsController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: RoomSets
        public ActionResult Index()
        {
            return View(db.RoomSet.ToList());
        }

        // GET: RoomSets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomSet roomSet = db.RoomSet.Find(id);
            if (roomSet == null)
            {
                return HttpNotFound();
            }
            return View(roomSet);
        }

        // GET: RoomSets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoomSets/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,number,endTime")] RoomSet roomSet)
        {
            if (ModelState.IsValid)
            {
                db.RoomSet.Add(roomSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roomSet);
        }

        // GET: RoomSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomSet roomSet = db.RoomSet.Find(id);
            if (roomSet == null)
            {
                return HttpNotFound();
            }
            return View(roomSet);
        }

        // POST: RoomSets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,number,endTime")] RoomSet roomSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roomSet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roomSet);
        }

        // GET: RoomSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomSet roomSet = db.RoomSet.Find(id);
            if (roomSet == null)
            {
                return HttpNotFound();
            }
            return View(roomSet);
        }

        // POST: RoomSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoomSet roomSet = db.RoomSet.Find(id);
            db.RoomSet.Remove(roomSet);
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
    }
}
