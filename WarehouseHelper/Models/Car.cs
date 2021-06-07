using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WarehouseHelper
{
    public partial class Car
    {
        public Car()
        {
            Stones = new HashSet<Stone>();
        }
        [Key]
        public int CarId { get; set; }
        public DateTime Date { get; set; }
        public decimal Cost { get; set; }

        public virtual ICollection<Stone> Stones { get; set; }
    }
}
