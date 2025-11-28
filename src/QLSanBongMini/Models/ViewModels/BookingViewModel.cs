using System;

namespace QLSanBongMini.Models.ViewModels
{
    public class BookingViewModel
    {
        public int BookingID { get; set; }
        public string FieldName { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }
        public bool IsPaid { get; set; }
        public decimal AmountPaid { get; set; }
        public string UserFullName { get; set; }  // Họ tên người đặt
    }
}
