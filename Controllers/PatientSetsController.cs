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
    public class PatientSetsController : Controller
    {
        private const int session_time = 1;
        private Model1Container db = new Model1Container();

        // GET: PatientSets
        public ActionResult Index()
        {
            return View(db.PatientSet.ToList());
        }

        // GET: PatientSets/Details/5
        public ActionResult Details(int? id)
        {
            /*            if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        PatientSet patientSet = db.PatientSet.Find(id);
                        if (patientSet == null)
                        {
                            return HttpNotFound();
                        }*/
            if (Session["StartTime"] is DateTime startTime && Session["patientSet"] is PatientSet patientSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name && p.pwd == patientSet.pwd);
                if (existingPatient != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    return View(existingPatient);
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            return RedirectToAction("SignIn", "PatientSets");
        }

        // GET: PatientSets/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: PatientSets/SignIn
        public ActionResult SignIn()
        {
#pragma warning disable CS0252 // 可能非有意的引用比较；左侧需要强制转换
            if (HttpContext.Application["GlobalVar"] != null && HttpContext.Application["GlobalVar"] != "")
            {
                TempData["Success"] = "Successfully signed out";
            }
#pragma warning restore CS0252 // 可能非有意的引用比较；左侧需要强制转换
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            return View();
        }
        // POST: PatientSets/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,pwd,confirm_pwd")] PatientSet patientSet)
        {
            if (patientSet.name.Length < 1 || patientSet.name.Length > 50)
            {
                ModelState.AddModelError("name", "Name must be longer then 1 and shorter then 50.");
            }
            PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name);
            if (existingPatient != null)
            {
                ModelState.AddModelError("name", "This name has been used.");
            }
            if (patientSet.pwd.Length < 6 || patientSet.pwd.Length > 50)
            {
                ModelState.AddModelError("pwd", "Password must be longer then 6 and shorter then 50.");
            }
            if (patientSet.pwd != patientSet.confirm_pwd)
            {
                ModelState.AddModelError("confirm_pwd", "Password and Confirm Password do not match.");
            }
            if (ModelState.IsValid)
            {
                db.PatientSet.Add(patientSet);
                db.SaveChanges();
                Session["patientSet"] = patientSet;
                Session["StartTime"] = DateTime.Now;
                HttpContext.Application["GlobalVar"] = "patient";
                return RedirectToAction("Index", "ProcessSets");
            }

            return View(patientSet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Id,name,pwd")] PatientSet patientSet)
        {
            patientSet.confirm_pwd = patientSet.pwd;
            PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name && p.pwd == patientSet.pwd);
            if (existingPatient == null)
            {
                // If the patient already exists, add a model error and return to the view
                ModelState.AddModelError("pwd", "Patient doesn't exist or password is incorrect.");
                return View(existingPatient);
            }
            TempData["Success"] = "Successfully signed in";
            TempData.Remove("Error");
            Session["patientSet"] = patientSet;
            Session["StartTime"] = DateTime.Now;
            HttpContext.Application["GlobalVar"] = "patient";
            return RedirectToAction("Index", "ProcessSets");
        }

        // GET: PatientSets/Edit/5
        public ActionResult Edit(int? id)
        {
            /*            if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        PatientSet patientSet = db.PatientSet.Find(id);
                        if (patientSet == null)
                        {
                            return HttpNotFound();
                        }*/
            if (Session["StartTime"] is DateTime startTime && Session["patientSet"] is PatientSet patientSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name && p.pwd == patientSet.pwd);
                if (existingPatient != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    return View(existingPatient);
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            return RedirectToAction("SignIn", "PatientSets");
        }

        // POST: PatientSets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,pwd,confirm_pwd")] PatientSet newPatientSet)
        {
/*            if (ModelState.IsValid)
            {
                db.Entry(newPatientSet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newPatientSet);*/
            if (Session["StartTime"] is DateTime startTime && Session["patientSet"] is PatientSet patientSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name && p.pwd == patientSet.pwd);
                if (existingPatient != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    if (newPatientSet.name.Length < 1 || newPatientSet.name.Length > 50)
                    {
                        ModelState.AddModelError("name", "Name must be longer then 1 and shorter then 50.");
                    }
                    existingPatient = db.PatientSet.FirstOrDefault(p => p.name == newPatientSet.name);
                    if (existingPatient != null && existingPatient.name != patientSet.name)
                    {
                        ModelState.AddModelError("name", "This name has been used.");
                    }
                    if (newPatientSet.pwd.Length < 6 || newPatientSet.pwd.Length > 50)
                    {
                        ModelState.AddModelError("pwd", "Password must be longer then 6 and shorter then 50.");
                    }
                    if (newPatientSet.pwd != newPatientSet.confirm_pwd)
                    {
                        ModelState.AddModelError("confirm_pwd", "Password and Confirm Password do not match.");
                    }
                    if (ModelState.IsValid)
                    {
                        var existingPatientSets = db.PatientSet.Where(p => p.Id == newPatientSet.Id).ToList();
                        if (existingPatientSets != null && existingPatientSets.Count > 0)
                        {
                            // 遍历existingPatientSets中的全部个体，并将它们都设为Detached状态
                            foreach (var existingPatientSet in existingPatientSets)
                            {
                                db.Entry(existingPatientSet).State = System.Data.Entity.EntityState.Detached;
                            }
                        }
                        var processSets = db.ProcessSet.Where(p => p.Patient == patientSet.name).ToList();
                        // Update the Patient field to newPatientSet.name for all fetched instances
                        foreach (var processSet in processSets)
                        {
                            processSet.Patient = newPatientSet.name;
                        }
                        db.Entry(newPatientSet).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["patientSet"] = newPatientSet;
                        return RedirectToAction("Details", "PatientSets");
                    }
                    return View(newPatientSet);
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            return RedirectToAction("SignIn", "PatientSets");
        }

        // GET: PatientSets/Delete/5
        public ActionResult Delete(int? id)
        {
            /*            if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        PatientSet patientSet = db.PatientSet.Find(id);
                        if (patientSet == null)
                        {
                            return HttpNotFound();
                        }
                        return View(patientSet);*/
            if (Session["StartTime"] is DateTime startTime && Session["patientSet"] is PatientSet patientSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name && p.pwd == patientSet.pwd);
                if (existingPatient != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    return View(existingPatient);
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            return RedirectToAction("SignIn", "PatientSets");
        }

        // POST: PatientSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ?id)
        {
            if (Session["StartTime"] is DateTime startTime && Session["patientSet"] is PatientSet patientSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                PatientSet existingPatient = db.PatientSet.FirstOrDefault(p => p.name == patientSet.name && p.pwd == patientSet.pwd);
                if (existingPatient != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session.Clear();
                    db.PatientSet.Remove(existingPatient);
                    db.SaveChanges();
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Success"] = "Successfully deactivated";
            return RedirectToAction("SignIn", "PatientSets");
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
