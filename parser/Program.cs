using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using parser;
using System.Text.Json;
using System.Diagnostics;
// See https://aka.ms/new-console-template for more information


ParseByCity city= new();
ParseBase parseBase = new();

// // // // // // // // await parseBase.csvWrite<ProductModel, ProductMap>(new List<ProductModel>(), "Result");

// Парсинг для москвы
// var swM = new Stopwatch();
// swM.Start();
// var list = await city.parseByPages("77000000000");
// swM.Stop();
// Console.WriteLine(swM.Elapsed);
// await parseBase.csvAppend<ProductModel, ProductMap>("Result", list);
// //

//парсинг для ростова
var swR = new Stopwatch();
swR.Start();
var listR = await city.parseByPages("61000001000");
swR.Stop();
Console.WriteLine("Rostov parse time "+swR.Elapsed);
await parseBase.csvAppend<ProductModel, ProductMap>("Result",listR);

// // Console.WriteLine("Moscow parse time " + swM.Elapsed);

