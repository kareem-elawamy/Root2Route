namespace Domain.Enums
{
    public enum CropServiceResult
    {
        Success,
        FarmNotFound,
        PlantInfoNotFound,
        InvalidDates, // لو تاريخ الحصاد قبل تاريخ الزراعة
        NotFound,
        Failed
    }
}