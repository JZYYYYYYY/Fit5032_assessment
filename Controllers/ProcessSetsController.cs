using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProcessSetsController : Controller
    {
        private const int session_time = 1;
        private Model1Container db = new Model1Container();

        // GET: ProcessSets
        public ActionResult Index()
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
                    Session["StartTime"] = currentTime;
                    return View(db.ProcessSet.Where(p => p.Patient == patientSet.name));
                }
            }
            Session.Clear();
#pragma warning disable CS0252 // 可能非有意的引用比较；左侧需要强制转换
            if (HttpContext.Application["GlobalVar"] != null)
            {
                TempData["Error"] = "Not signed in or session timeout";
                TempData.Remove("Success");
            }
#pragma warning restore CS0252 // 可能非有意的引用比较；左侧需要强制转换
            HttpContext.Application["GlobalVar"] = "";
            TempData.Keep();
            return RedirectToAction("SignIn", "PatientSets");
        }

        // GET: ProcessSets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessSet processSet = db.ProcessSet.Find(id);
            if (processSet == null)
            {
                return HttpNotFound();
            }
            return View(processSet);
        }

        private List<string> getBodyPartsOptions()
        {
            return new List<string>
            {
                "Head",
                "Chest",
                "Leg",
                "Arm",
            };
        }
        // GET: ProcessSets/Create
        public ActionResult Create()
        {
            ViewBag.BodyParts = getBodyPartsOptions();
            return View(new ProcessSet());
        }

        // POST: ProcessSets/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Patient,Doctor,Room,startTime,payment,bodyPart")] ProcessSet processSet)
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
                    Session["StartTime"] = currentTime;
                    if (processSet.startTime == null)
                    {
                        ModelState.AddModelError("startTime", "You need to select a time.");
                    }
                    if (processSet.bodyPart == null)
                    {
                        ModelState.AddModelError("bodyPart", "You need to choose a body part.");
                    }
                    processSet.Patient = existingPatient.name;
                    processSet.payment = "Not Paid";
                    var doctor_data = db.DoctorSet.FirstOrDefault(d => d.endTime < processSet.startTime);
                    var room_data = db.RoomSet.FirstOrDefault(d => d.endTime < processSet.startTime);
                    if (processSet.startTime < DateTime.Now.AddMinutes(-5))
                    {
                        ModelState.AddModelError("startTime", "You can't choose time before.");
                    }
                    if(doctor_data == null || room_data == null)
                    {
                        ModelState.AddModelError("startTime", "No valid room or doctor at this time.");
                    }
                    if (ModelState.IsValid)
                    {
                        processSet.Doctor = doctor_data.name;
                        processSet.Room = room_data.number;
                        db.ProcessSet.Add(processSet);

                        DateTime newDateTime = (DateTime)processSet.startTime;
                        // 更新RoomSet中的endTime
                        room_data.endTime = newDateTime.AddHours(1);
                        db.Entry(room_data).State = System.Data.Entity.EntityState.Modified;
                        
                        // 更新DoctorSet中的endTime
                        doctor_data.endTime = newDateTime.AddHours(1);
                        db.Entry(doctor_data).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    ViewBag.BodyParts = getBodyPartsOptions();
                    return View(processSet);
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            TempData.Remove("Success");
            TempData.Keep();
            return RedirectToAction("SignIn", "PatientSets");
        }

        // GET: ProcessSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessSet processSet = db.ProcessSet.Find(id);
            if (processSet == null)
            {
                return HttpNotFound();
            }
            return View(processSet);
        }

        // POST: ProcessSets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Patient,Doctor,Room,startTime,payment,bodyPart")] ProcessSet processSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(processSet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(processSet);
        }

        // GET: ProcessSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessSet processSet = db.ProcessSet.Find(id);
            if (processSet == null)
            {
                return HttpNotFound();
            }
            return View(processSet);
        }

        // POST: ProcessSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
                    Session["StartTime"] = currentTime;
                    ProcessSet processSet = db.ProcessSet.Find(id);
                    var doctorSets = db.DoctorSet.Where(p => p.name == processSet.Doctor).ToList();
                    var roomSets = db.RoomSet.Where(p => p.number == processSet.Room).ToList();
                    // Update the Patient field to newPatientSet.name for all fetched instances
                    foreach (var doctor in doctorSets)
                    {
                        doctor.endTime = DateTime.Now;
                    }
                    foreach (var room in roomSets)
                    {
                        room.endTime = DateTime.Now;
                    }
                    db.ProcessSet.Remove(processSet);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            TempData.Remove("Success");
            TempData.Keep();
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

        // GET: ProcessSets/Delete/5
        public ActionResult Email()
        {
            return View();
        }
        [HttpPost]
        public String CreateAppointment()
        {
            var vx = Request.Files["Attachment"].ContentLength;

            // Store the attachment in local storage.
            var Str1 = Request.Files[0].FileName.Split('.');
            var FileType = Str1[Str1.Length - 1];
            var FilePath =
                Server.MapPath("~/Uploads/") +
                string.Format(@"{0}", Guid.NewGuid()) +
                "." + FileType;
            Request.Files[0].SaveAs(FilePath);

            // Send confirmation email.
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(Request.Form["EmailAddress"]));
            mail.From = new MailAddress("jzy_monash_seu@outlook.com");
            
            mail.Subject = "Appointment Conformation";
            mail.Body =
                "You made an appointment:\n" +
                "Student ID: " + Request.Form["StudentID"] + "\n" +
                "Engineer ID: " + Request.Form["EngineerID"] + "\n" +
                "Date: " + Request.Form["AppointmentDate"];
            mail.IsBodyHtml = false;
            
            var attachment = new System.Net.Mail.Attachment(FilePath);
            mail.Attachments.Add(attachment);
            
            var smtp = new SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential
                ("jzy_monash_seu@outlook.com", "outlook.com");
            
            smtp.Send(mail);
            return "Success";
        }
    }
}
