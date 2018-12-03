using System;
using System.Collections.Generic;

namespace BooktopiaApiService.Models
{
    public partial class Title
    {
        public Title()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int TitleId { get; set; }
        public string Title1 { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public DateTime PubDate { get; set; }
        public decimal? Price { get; set; }

        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
