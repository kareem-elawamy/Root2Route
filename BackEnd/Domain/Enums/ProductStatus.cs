namespace Domain.Enums
{
    public enum ProductStatus
    {
        Pending,   // في انتظار الموافقة (الحالة الافتراضية)
        Approved,  // تم الموافقة عليه (يظهر للناس)
        Rejected   // تم الرفض (ممكن نرجعله سبب الرفض)
    }
}