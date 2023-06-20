using System.Text;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace parser
{
    class ParseByCity
    {
        private ParseBase parseBase = new ParseBase();
        public async Task<ProductModel> parseProductAsync(string htmlSocurceCode, string cityCode, IHtmlDocument document, string url)
        {
            
            string _url = url;
            string _region = "";
            string _breadcrumbs ="";
            string _name = "";
            int _price = 0;//Цена ноль, если товар на сайте болле не продаётся(как трактор из задания)
            int _oldPrice = 0; // Старая цена ноль, если нет скидок или товара нет в наличии/ более не продаётся 
            string _availability=""; // наличие товара
            string _pitcsUrls = ""; // урлы картинок разделёенные пробелом


            //
            //регион
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName =="A" 
                                                                                && x.HasAttribute("data-src")
                                                                                && x.GetAttribute("data-src").StartsWith("#reg"));

                foreach (var item in tagContent)
                {
                    _region = item.TextContent.Trim();
                }

            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Region tag is empty");
            }
            
            //
            //Изображения
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName == "A" 
                                                                                && x.HasAttribute("data-fancybox")
                                                                                && x.GetAttribute("data-fancybox").StartsWith("grou"));
                foreach(var item in tagContent)
                {
                    _pitcsUrls += item.GetAttribute("href")+" ";
                    _pitcsUrls.Trim();
                }
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Image tag(s) is/are empty");
            }
            
            //  
            //хлебыне крошки
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.ClassList.Contains("breadcrumb-item"));
                for(int i = 0; i < tagContent.Count()-1; i++ )
                {
                    _breadcrumbs += tagContent.ElementAt(i).TextContent +">";
                }
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Breadcrumb tags are null");
            }

            //
            //старая цена 
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName =="SPAN" 
                                                                                && x.ClassList.Contains("old-price"));
                // Console.WriteLine(tagContent.ElementAt(0).TextContent);
                foreach (var item in tagContent)
                {
                    _oldPrice = int.Parse(item.TextContent.Trim().Replace(" ","").Replace("руб.", ""));
                }
            }
            catch(FormatException e)
            {
                // Console.WriteLine(e.StackTrace);
                // Console.WriteLine("Нет скидок");
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Old price tag is null");
            }

            //
            //цена
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName =="SPAN" 
                                                                                && x.ClassList.Contains("price"));
                foreach (var item in tagContent)
                {
                    _price = int.Parse(item.TextContent.Trim().Replace(" ", "").Replace("руб.", ""));
                }
                
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Price tags is null");
            }
            
            
            //
            //Наличие товара
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.ClassList.Contains("ok") || 
                                                                                x.ClassList.Contains("net-v-nalichii") &&
                                                                                x.TagName == "DIV");
                foreach(var item in tagContent)
                {
                    _availability = item.TextContent.Trim();
                }

            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Product availability tag is null");
            }

            //
            //Наименование товара
            //
            try
            {
                var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName == "H1" 
                                                                                && x.ClassList.Contains("detail-name"));
                foreach (var item in tagContent)
                {
                    _name = item.TextContent.Trim();
                }
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(" tags is/are null");
            }

            // try
            // {
            //     var tagContent = document.QuerySelectorAll("");
            // }
            // catch(NullReferenceException e)
            // {
            //     Console.WriteLine(e.StackTrace);
            //     Console.WriteLine(" tags is/are null");
            // }
            //Console.WriteLine();
            
            return new ProductModel()
            {
              name =  Encoding.UTF8.GetString(Encoding.Default.GetBytes(_name)),
              region = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_region)),
              url = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_url)),
              pitcsUrls = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_pitcsUrls)),
              availability = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_availability)),
              breadcrumbs = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_breadcrumbs)),
              oldPrice = _oldPrice,
              price = _price  
            };
        }
        

        public async Task<List<ProductModel>> parseByPages(string cityCode)
        {
            // формат url 
            // https://www.toy.ru/catalog/boy_transport/?filterseccode%5B0%5D=transport&PAGEN_5=2
            
            List<ProductModel> models = new List<ProductModel>();

            int maxPage =1;
            int pageNumber = 1;
            string url = $"https://www.toy.ru/catalog/boy_transport/?filterseccode%5B0%5D=transport&PAGEN_5={pageNumber}";

            Uri uri = new Uri(url);
             
            string htmlSocurceCode = await parseBase.htmlSource(uri, cityCode);

            if(htmlSocurceCode == null || htmlSocurceCode == "")
            {
                Console.WriteLine("Source code is empty");
                return null;
            }

            var parser = new HtmlParser();
            IHtmlDocument document = await parser.ParseDocumentAsync(htmlSocurceCode);

            try
            {
                var pagesCountContent = document.QuerySelectorAll("*").Where(x => x.TagName == "A" && x.ClassList.Contains("page-link"));
                maxPage = int.Parse(pagesCountContent.ElementAt(pagesCountContent.Count()-2).TextContent);
            }
            catch(FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Не конвертируется из string  в int");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch(ArgumentOutOfRangeException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("PagesCountContent is empty");
                Console.ForegroundColor = ConsoleColor.White;

            }
            catch(NullReferenceException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if(maxPage == 1)
            {
                try
                {
                    var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName == "A" &&
                                                                                    x.ClassList.Contains("product-name"));

                    foreach (var item in tagContent)
                    {
                        string prodUrl = "https://www.toy.ru"+item.GetAttribute("href");
                        // Console.WriteLine(prodUrl);
                        models.Add(await parseProductAsync(htmlSocurceCode, cityCode, document, prodUrl));
                    }
                    return models;
                }
                catch(NullReferenceException e)
                {
                    Console.WriteLine("No tag a with product-name");
                }

                return null;
            }

            List<string> prodUrlList = new List<string>();
            Console.WriteLine(maxPage);
            for(int i = pageNumber; i <= maxPage; i++)
            {
                url = $"https://www.toy.ru/catalog/boy_transport/?filterseccode%5B0%5D=transport&PAGEN_5={i}";
                htmlSocurceCode = await parseBase.htmlSource(new Uri(url), cityCode);
                document = await parser.ParseDocumentAsync(htmlSocurceCode);
                
                try
                {
                    var tagContent = document.QuerySelectorAll("*").Where(x => x.TagName == "A" &&
                                                                                    x.ClassList.Contains("product-name"));

                    foreach (var item in tagContent)
                    {
                        string prodUrl = "https://www.toy.ru"+item.GetAttribute("href");
                        // Console.WriteLine(prodUrl);
                        prodUrlList.Add(prodUrl);
                    }
                }
                catch(NullReferenceException e)
                {
                    Console.WriteLine("No tag a with product-name");
                }
            }
            for(int i = 0; i < prodUrlList.Count; i++)
            {
                htmlSocurceCode = await parseBase.htmlSource(new Uri(prodUrlList[i]), cityCode);
                document = await parser.ParseDocumentAsync(htmlSocurceCode);
                
                models.Add(await parseProductAsync(htmlSocurceCode, cityCode, document, prodUrlList[i]));
            }
            return models;
        }
    }


}