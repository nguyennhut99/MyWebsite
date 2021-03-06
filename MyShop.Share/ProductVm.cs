using System;

namespace MyShop.Share
{
    public class ProductVm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string CreateDate { get; set; }

        public string ModifyDate { get; set; }

        public decimal Rating { get; set; }

        public int RatingCount { get; set; }

        public string ThumbnailImageUrl { get; set; }

    }
}