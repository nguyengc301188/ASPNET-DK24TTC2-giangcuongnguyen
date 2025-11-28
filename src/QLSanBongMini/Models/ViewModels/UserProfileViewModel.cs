using System.Collections.Generic;
using QLSanBongMini.Models;

namespace QLSanBongMini.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<BookingViewModel> BookingHistory { get; set; }
    }
}
