using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static System.Double;

namespace ValutaConverterConsole
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var currencyList = PopulateListFromWeb();

                Console.WriteLine("Welcome to this currency calculator");
                Console.WriteLine("Please enter the amount of Danish Kroner to convert: ");

                var valueOfDanishKroner = Console.ReadLine();

                TryParse(valueOfDanishKroner, out var valueConverted);

                if (valueConverted == 0)
                {
                    throw new Exception("Input was invalid - please try again");
                }

                Console.WriteLine("Currencies available: ");
                foreach (var currency in currencyList)
                {
                    Console.Write(currency.Code + " ");
                }
                Console.WriteLine("Please enter currency code to convert to: ");
                var currencyCode = Console.ReadLine()!.ToUpper();

                if (currencyList.All(x => x.Code != currencyCode!.ToUpper()))
                {
                    throw new Exception("Invalid country code entered - please try again");
                }

                var currencyToConvertTo = currencyList.First(x => x.Code == currencyCode);
                
                var currencyConverted = Math.Round((100 / currencyToConvertTo.Value * valueConverted),2); 

                Console.WriteLine($"Amount of Danish Kroner: {currencyConverted} DKK");
                Console.WriteLine($"Currency select: {currencyToConvertTo.Code} - {currencyToConvertTo.Description}");
                Console.WriteLine($"Currency converted: {currencyConverted} {currencyToConvertTo.Code}");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static List<Currency> PopulateListFromWeb()
        {
            try
            {
                var webClient = new WebClient();
                var data = webClient.DownloadString("https://www.nationalbanken.dk/_vti_bin/DN/DataService.svc/CurrencyRatesXML?lang=da");
                var filteredList = data.Split("\n").Where(x => x.Contains("currency code"));
                var currencyList = new List<Currency>();

                foreach (var currency in filteredList)
                {
                    currencyList.Add(new Currency
                    {
                        Code = currency.Substring(currency.IndexOf("code=") + 6, 3),
                        Value = Parse(currency[(currency.IndexOf("rate=") + 6)..(currency.IndexOf("/>") - 2)]),
                        Description = currency[(currency.IndexOf("desc") + 6)..(currency.IndexOf("rate") - 2)]
                    });
                }

                return currencyList;

            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong with downloading the currency list. :(");
                Console.WriteLine("Error: " + e);
            }

            return new List<Currency>();
        }
    }

    internal class Currency
    {
        public string Code { get; init; }
        public double Value { get; init; }
        public string Description { get; init; }
    }
}