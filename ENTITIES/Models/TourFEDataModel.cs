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

        /// <summary>
        /// Vị trí hiển thị trong FlashSale (1 → 8)
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Loại tour (1: nội địa, 2: outbound, 3: quốc tế...)
        /// </summary>
        public int? TourType { get; set; }

        /// <summary>
        /// Tên loại tour (AllCode.Description)
        /// </summary>
        public string TourTypeName { get; set; }

        /// <summary>
        /// Danh sách điểm đến (TourType = 1)
        /// </summary>
        public string GroupEndPoint1 { get; set; }

        /// <summary>
        /// Danh sách điểm đến (TourType = 2)
        /// </summary>
        public string GroupEndPoint2 { get; set; }

        /// <summary>
        /// Danh sách điểm đến (TourType = 3)
        /// </summary>
        public string GroupEndPoint3 { get; set; }
    }
}
