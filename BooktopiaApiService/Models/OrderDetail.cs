using System;
using System.Collections.Generic;

namespace BooktopiaApiService.Models
{
    public partial class OrderDetail
    {
        public int ToId { get; set; }
        public int OrderId { get; set; }
        public int TitleId { get; set; }
        public int Count { get; set; }

        public Order Order { get; set; }
        public Title Title { get; set; }
    }
}
