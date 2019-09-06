using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCCReportingTool.Models
{
    public class WorkItemUpdateClass
    {
        public int DevopsItemID { get; set; }
        public string Summary { get; set; }
        public int Priority { get; set; }
        public string pendingWith { get; set; }
        public string ExpectedReleaseDate { get; set; }
        public int Status { get; set; }
        public int Assignee { get; set; }
    }

    public partial class WorkItem
    {
        public virtual ICollection<WorkItem> UnUpdatedData { get; set; } = null;
        public virtual ICollection<WorkItem> CompletedItems { get; set; } = null;//done
        public virtual ICollection<WorkItem> DevInProgressdata { get; set; } = null;//devinprogress    
        public virtual ICollection<WorkItem> SelectedforDevelopment { get; set; } = null;// selected for dev
        public virtual ICollection<WorkItem> BacklogData { get; set; } = null;//backlog
        public virtual ICollection<UpcomingTask> UpcomingData { get; set; } = null;//new
    }
}