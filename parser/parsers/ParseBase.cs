using System.Globalization;
using System.Net;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace parser
{
    class ParseBase
    {
        public async Task<string> htmlSource(Uri uri, string cityCode)
        {
            string response = null;

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding.GetEncoding("windows-1254");
                var client = new HttpClient();
                
                Cookie city = new Cookie("BITRIX_SM_city",cityCode);
                CookieContainer container = new CookieContainer();
                container.Add(uri, city);

                client.DefaultRequestHeaders.Add("cookie", container.GetCookieHeader(uri));

                var responseBody = await client.GetStringAsync(uri);
                responseBody = Encoding.UTF8.GetString(Encoding.Default.GetBytes(responseBody));

                response = responseBody;

            }
            catch(NullReferenceException e )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }

            return response;
        }
        
        public async Task csvWrite <P,PMap>(List<P> models, string name)
                                    where PMap : ClassMap<P>
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = " ;"
            };

            using(var writer = new StreamWriter(name+".csv"))
            using(var csvWriter = new CsvWriter(writer, csvConfig))
            {
                csvWriter.Context.RegisterClassMap<PMap>();
                await csvWriter.WriteRecordsAsync(models);
                await writer.FlushAsync();
            }

        }

        public async Task csvAppend<T, TMap>(string name, List<T> models)
                                    where TMap : ClassMap<T>
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = " ;"
            };

            var list =  new List<T>();

            using(var reader = new StreamReader(name+".csv"))
            using(var csvReader = new CsvReader(reader,csvConfig))
            {
                csvReader.Context.RegisterClassMap<TMap>();
                var rows = csvReader.GetRecords<T>();
                list = rows.ToList<T>();
            }
            list.AddRange(models);
            using(var writer = new StreamWriter(name+".csv"))
            using(var csvWriter = new CsvWriter(writer, csvConfig))
            {
                csvWriter.Context.RegisterClassMap<TMap>();
                await csvWriter.WriteRecordsAsync(list);
                await writer.FlushAsync();
            }
        }
    }
}
