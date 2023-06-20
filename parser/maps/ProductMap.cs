using CsvHelper.Configuration;

namespace parser
{
    class ProductMap : ClassMap<ProductModel>
    {
        public ProductMap()
        {
            Map(m => m.url).Index(0).Name("Url");
            Map(m => m.name).Index(1).Name("Name");
            Map(m => m.region).Index(2).Name("Region");
            Map(m => m.breadcrumbs).Index(3).Name("Breadcrumbs");
            Map(m => m.price).Index(4).Name("Price");
            Map(m => m.oldPrice).Index(5).Name("OldPrice");
            Map(m => m.availability).Index(6).Name("Availability");
            Map(m => m.pitcsUrls).Index(7).Name("PictsUrls");
            //Map(m => m.).Index().Name("");
        }
    }
}