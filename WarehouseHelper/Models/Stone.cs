using System;
using System.Collections.Generic;

#nullable disable

namespace WarehouseHelper
{
    [Serializable]
    public partial class Stone
    {
        public string StoneId { get; set; }
        public int CarId { get; set; }
        public decimal PricePerCube { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Type { get; set; }
        public bool IsSawn { get; set; }

        public Stone()
        { }
        // проверка 
        public virtual Car Car { get; set; }
    }
}
