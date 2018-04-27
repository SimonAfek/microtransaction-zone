namespace DIHMT.Models
{
    public class BlockModel
    {
        public string BlockedIp { get; set; }
        public bool Explicit { get; set; }
        public string BlockType { get; set; }
        public int PendingId { get; set; }
        public string Reason { get; set; }
    }
}
