using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using VCCReportingTool.Models;
using WorkItem = VCCReportingTool.Models.WorkItem;


namespace VCCReportingTool.Controllers
{
    public class WorkItemsController : Controller
    {
        private DevOpsReportEntities db = new DevOpsReportEntities();

        // GET: WorkItems
        public ActionResult Index()
        {
            var workItems = db.WorkItems.Where(x => x.IsUpdated == false).ToList();  // .Include(w => w.Status1).Include(w => w.User);
            ViewBag.Assignee = db.Users.ToList();  //"UserID", "FullName");
            ViewBag.Status = db.Status.ToList();  // "StatusID", "Title");

            WorkItem objWorkItem = null;

            objWorkItem = new WorkItem();

            objWorkItem.UnUpdatedData = db.WorkItems.Where(x => x.IsUpdated == false).ToList();
            objWorkItem.DevInProgressdata = db.WorkItems.Where(x => x.Status == 2).ToList();
            objWorkItem.SelectedforDevelopment = db.WorkItems.Where(x => x.Status == 3).ToList();
            objWorkItem.BacklogData = db.WorkItems.Where(x => x.Status == 4).ToList();
            objWorkItem.CompletedItems = db.WorkItems.Where(x => x.Status == 1).ToList();
            objWorkItem.UpcomingData = db.UpcomingTasks.ToList();

            return View(objWorkItem);
        }
        public ActionResult UpdateRecord(int initialPriorityValue, int changedPriorityValue, string PriorityType, string PromptClick, string workitem)
        {
            // Update model to your db 
            WorkItemUpdateClass model = JsonConvert.DeserializeObject<WorkItemUpdateClass>(workitem);
            int diffAsc = changedPriorityValue - initialPriorityValue;
            int diffDesc = initialPriorityValue - changedPriorityValue;
            if (PriorityType == "Inc")
            {
                var getItemsonPriority = db.WorkItems.Where(x => x.Priority >= initialPriorityValue).ToList();
                if (getItemsonPriority.Count != 0)
                {
                    foreach (var item in getItemsonPriority)
                    {
                        item.Priority = item.Priority + diffAsc;
                        db.SaveChanges();
                    }
                }
            }
            else if (PriorityType == "Dec")
            {
                var getItemsonPriority = db.WorkItems.Where(x => x.Priority <= initialPriorityValue).ToList();
                if (getItemsonPriority.Count != 0)
                {
                    foreach (var item in getItemsonPriority)
                    {
                        item.Priority = item.Priority - diffDesc;
                        db.SaveChanges();
                    }
                }
            }
            else if ((PriorityType != "Inc" && PriorityType != "Dec") && PromptClick == "OK")
            {
                var getItemsonPriority1 = db.WorkItems.Where(x => x.Priority >= model.Priority).ToList();
                if (getItemsonPriority1.Count != 0)
                {
                    foreach (var item in getItemsonPriority1)
                    {
                        item.Priority = item.Priority - 1;
                        db.SaveChanges();
                    }
                }
            }
            else if ((PriorityType != "Inc" && PriorityType != "Dec") && PromptClick == "Cancel")
            {
                var getItemsonPriority1 = db.WorkItems.Where(x => x.Priority == model.Priority).ToList();
                if (getItemsonPriority1.Count != 0)
                {
                    foreach (var item in getItemsonPriority1)
                    {
                        item.Priority = null;
                        db.SaveChanges();
                    }
                }
            }

            var getWorkItem = db.WorkItems.Where(x => x.DevopsItemID == model.DevopsItemID).ToList();
            if (getWorkItem.Count != 0)
            {
                foreach (var item in getWorkItem)
                {
                    item.Summary = model.Summary;
                    item.Priority = model.Priority;
                    item.Type = "Task";
                    item.PendingWith = model.pendingWith;
                    item.ExpectedReleaseDate = Convert.ToDateTime(model.ExpectedReleaseDate);
                    item.Status = model.Status;
                    item.Assignee = model.Assignee;
                    item.IsUpdated = true;
                    db.SaveChanges();

                    if (item.Status != 1) { item.Priority = model.Priority; }
                    else { item.Priority = null; }
                    db.SaveChanges();
                }
            }
            return Json(new { success = true, responseText = "Record Updated Successfully" });
        }


        //Added New Note Against workitemid
        public ActionResult NewNoteUpdate(string UpdatedNote, string EditedBy, string EditedDate, int NoteId)
        {
            var NoteDetails = db.Notes.Where(x => x.NoteID == NoteId).ToList();
            foreach (var item in NoteDetails)
            {
                item.IsDeleted = true;
                db.SaveChanges();
            }
            Note objNote = new Note();
            objNote.WorkItemID = NoteDetails.FirstOrDefault().WorkItemID;
            objNote.RelatedNoteID = NoteId;
            objNote.EditedBy = EditedBy;
            var getediteduser = db.Users.Where(x => x.FullName == EditedBy).FirstOrDefault();
            objNote.EditedUser = getediteduser.UserID;
            objNote.EditedDate = Convert.ToDateTime(EditedDate);
            objNote.Note1 = UpdatedNote;
            objNote.IsDeleted = false;
            db.Notes.Add(objNote);
            db.SaveChanges();
            return Json(new { success = true, responseText = "Added New Note against WorkItem Successfully" });
        }

        //Add New Fix Version
        public ActionResult AddNewFixVersion(string FixVersionName, string Comment, int WorkItemID, int FixVersionID)
        {
            var result = db.FixVersionRecords.Where(x => x.FixVersionID == FixVersionID).ToList();
            foreach (var item in result)
            {
                item.IsDeleted = true;
                db.SaveChanges();
            }
            FixVersionRecord obj = new FixVersionRecord();
            obj.FixVersionName = FixVersionName;
            obj.WorkItemID = WorkItemID;
            obj.RelatedFixVersionID = FixVersionID;
            obj.EditedBy = "User";
            obj.EditedDate = DateTime.Now;
            obj.Comments = Comment;
            obj.IsDeleted = false;
            db.FixVersionRecords.Add(obj);
            db.SaveChanges();
            return Json(new { success = true, responseText = "Added New FixVersion against WorkItem Successfully" });
        }

        public ActionResult AddNoteforWorkItem(string Comment, int WorkItemID, string UpdateBy)
        {
            Note objNote = new Note();
            objNote.WorkItemID = WorkItemID;
            objNote.EditedBy = UpdateBy;
            var getediteduser = db.Users.Where(x => x.FullName == UpdateBy).FirstOrDefault();
            objNote.EditedUser = getediteduser.UserID;
            objNote.EditedDate = DateTime.Now;
            objNote.Note1 = Comment;
            objNote.IsDeleted = false;
            db.Notes.Add(objNote);
            db.SaveChanges();
            return Json(new { success = true, responseText = "Added New Note against WorkItem Successfully" });
        }


        //Update Workitem on against Id:
        public ActionResult UpdateWorkitembyID(int devopsid, string summary, int priority, string fixversion, string date, string pendingwith, string notes, int status, int assignee)
        {
            var getWorkItem = db.WorkItems.Where(x => x.DevopsItemID == devopsid).ToList();
            if (getWorkItem.Count != 0)
            {
                foreach (var item in getWorkItem)
                {
                    item.Summary = summary;
                    item.Priority = priority;
                    item.Fixversion = fixversion;
                    item.PendingWith = pendingwith;
                    item.ExpectedReleaseDate = Convert.ToDateTime(date);
                    item.Status = status;
                    item.Assignee = assignee;
                    item.Notes = notes;
                    item.IsUpdated = true;

                    //code for add the note(workitemtable) to Notes table against workitemid
                    Note objnote = new Note();
                    objnote.WorkItemID = item.WorkItemID;
                    if (Session["LoggedinUser"] != null)
                    {
                        objnote.EditedBy = Session["LoggedinUser"].ToString();
                    }
                    var getediteduser = db.Users.Where(x => x.FullName == objnote.EditedBy).FirstOrDefault();
                    objnote.EditedUser = getediteduser.UserID;
                    objnote.EditedDate = DateTime.Now;
                    objnote.Note1 = item.Notes;
                    objnote.IsDeleted = false;
                    db.Notes.Add(objnote);
                    db.SaveChanges();

                    //code for add the FixVersion(workitemtable) to Fixversion table against workitemid
                    FixVersionRecord objFixVersionRecord = new FixVersionRecord();
                    objFixVersionRecord.FixVersionName = fixversion;
                    objFixVersionRecord.WorkItemID = item.WorkItemID;
                    if (Session["LoggedinUser"] != null)
                    {
                        objFixVersionRecord.EditedBy = Session["LoggedinUser"].ToString();
                    }
                    objFixVersionRecord.EditedDate = DateTime.Now;
                    objFixVersionRecord.Comments = "First FixVersion for the workitem";
                    objFixVersionRecord.IsDeleted = false;
                    db.FixVersionRecords.Add(objFixVersionRecord);
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
            return Json(new { success = true, responseText = "Updated the Selected WorkItem Successfully" });
        }


        //Delete functionality by NoteID
        public ActionResult DeleteNote(int id)
        {
            var resultset = db.Notes.Where(x => x.NoteID == id).ToList();
            if (resultset.Count != 0)
            {
                foreach (var item in resultset)
                {
                    item.IsDeleted = true;
                }
                db.SaveChanges();
            }
            return Json(new { success = true, responseText = "Selected Note deleted Successfully" });
        }

        public ActionResult ViewPreview()
        {
            WorkItem objWorkItem = new WorkItem();
            var UnUpdateddata = db.WorkItems.Where(x => x.IsUpdated == false).ToList();
            var DoneItemsdata = db.WorkItems.Where(x => x.Status == 1).ToList();
            var Devinprocessdata = db.WorkItems.Where(x => x.Status == 2).ToList();
            var SelectedforDevelopment = db.WorkItems.Where(x => x.Status == 3).ToList();
            var backlogdata = db.WorkItems.Where(x => x.Status == 4).ToList();
            var UpcomingTasks = db.UpcomingTasks.ToList();

            objWorkItem.UnUpdatedData = UnUpdateddata;
            objWorkItem.DevInProgressdata = Devinprocessdata;
            objWorkItem.SelectedforDevelopment = SelectedforDevelopment;
            objWorkItem.BacklogData = backlogdata;
            objWorkItem.CompletedItems = DoneItemsdata;
            objWorkItem.UpcomingData = UpcomingTasks;
            return View(objWorkItem);
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
