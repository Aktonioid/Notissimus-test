using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using parser;
using System.Text.Json;
using System.Diagnostics;
// See https://aka.ms/new-console-template for more information


ParseByCity city= new();
ParseBase parseBase = new();


// Парсинг для москвы
var swM = new Stopwatch();
swM.Start();
var list = city.parseByPages("77000000000").GetAwaiter().GetResult();
swM.Stop();
Console.WriteLine(swM.Elapsed);
await parseBase.csvWrite<ProductModel, ProductMap>(list, "Moscow");
//

//парсинг для ростова
var swR = new Stopwatch();
swR.Start();
var listR = city.parseByPages("61000001000").GetAwaiter().GetResult();
swR.Stop();
await parseBase.csvWrite<ProductModel, ProductMap>(listR, "Rostow");

// // Console.WriteLine("Moscow parse time " + swM.Elapsed);
Console.WriteLine("Rostov parse time "+swR.Elapsed);
