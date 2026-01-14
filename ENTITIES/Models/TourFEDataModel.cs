using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.Models
{
    public class TourFEDataModel
    {
        /// <summary>
        /// Id của FlashSaleProduct
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id của TourProduct
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Tên tour
        /// </summary>
        public string TourName { get; set; }

        /// <summary>
        /// Giá hiện tại của tour
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Giá gốc trước FlashSale
        /// </summary>
        public decimal? OldPrice { get; set; }

        /// <summary>
        /// Số sao / rating
        /// </summary>
        public int? Star { get; set; }

        /// <summary>
        /// Ảnh đại diện tour
        /// </summary>
        public string Avatar { get; set; }
    }

}
