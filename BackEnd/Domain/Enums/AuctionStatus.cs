namespace Domain.Enums
{
    public enum AuctionStatus
    {
        Upcoming, // لم يبدأ بعد
        Ongoing, // شغال حالياً
        Completed, // انتهى وتم البيع
        Cancelled, // اتلغى
        NoBids, // انتهى بدون مزايدات
        Pending,
        Active,
    }
}
