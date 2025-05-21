using Microsoft.AspNetCore.Mvc.Rendering;

namespace FE.Helpers
{
    public static class FunctionHelper
    {
        /// <summary>
        /// Trả về thời gian đã trôi qua kể từ thời điểm hiện tại
        /// </summary>
        /// <param name="viewContext"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string GetTimeAgo(this ViewContext viewContext, DateTime time)
        {
            int? DurationHours = (int)((DateTime.Now - time).TotalHours);
            int? DurationMinutes = (int)((DateTime.Now - time).TotalMinutes);

            return (DurationHours, DurationMinutes) switch
            {
                // Lớn hơn 24 giờ
                (>= 24, _) => $"{DurationHours / 24} ngày",
                // Nhỏ hơn 24 giờ
                (< 24 and >= 1, _) => $"{DurationHours} giờ",
                // Nhỏ hơn 1 giờ và lớn hơn 1 phút
                ( < 1, >= 1) => $"{DurationMinutes} phút",
                // Nhỏ hơn 1 phút
                ( < 1, < 1) => "Vài giây"
            };
        }
    }
}
