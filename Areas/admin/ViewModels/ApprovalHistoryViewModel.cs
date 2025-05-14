using VerseCraft.Models;

namespace VerseCraft.Areas.admin.ViewModels
{
    public class ApprovalHistoryViewModel
    {
        public List<ApprovedItem> ApprovedItems { get; set; } = new();
        public List<Rejection> RejectedItems { get; set; } = new();
    }
}
