using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy.Json;
using RestSharp;
using Skymey_main_lib.Models.Prices.Polygon;
using Skymey_stocks_polygon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Skymey_stocks_polygon.Actions.Prices
{
    public class GetPrices : ServiceBase
    {
        private RestClient _client;
        private RestRequest _request;
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        private string _apiKey;
        public GetPrices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            _apiKey = config.GetSection("ApiKeys:Polygon").Value;
            _client = new RestClient("https://api.polygon.io/v2/snapshot/locale/us/markets/stocks/tickers?apiKey=" + _apiKey);
            _request = new RestRequest("https://api.polygon.io/v2/snapshot/locale/us/markets/stocks/tickers?apiKey=" + _apiKey, Method.Get);
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }

        public void GetPricesFromPolygon()
        {
            try
            {
                #region DIA
                _request.AddHeader("Content-Type", "application/json");
                var r = _client.Execute(_request).Content;
                TickerPrices tp = new JavaScriptSerializer().Deserialize<TickerPrices>(r);
                #endregion
                var ticker_finds = (from i in _db.Ticker select i);
                //tp.tickers = (from i in tp.tickers select i);
                foreach (var ticker in tp.tickers)
                    {
                    //if(ticker.ticker == "MSFT") {Console.WriteLine(ticker.ticker);}
                    //Console.WriteLine(ticker.ticker);
                    var ticker_find = (from i in ticker_finds where i.ticker == ticker.ticker select i).FirstOrDefault();

                    if (ticker_find == null)
                        {
                            ticker._id = ObjectId.GenerateNewId();
                            ticker.Update = DateTime.UtcNow;
                            _db.Ticker.Add(ticker);
                        }
                        else
                        {
                            ticker_find.prevDay = ticker.prevDay;
                            ticker_find.day = ticker.day;
                            ticker_find.min = ticker.min;
                            ticker_find.Update = DateTime.UtcNow;
                            _db.Ticker.Update(ticker_find);
                        }

                    }
                    _db.SaveChanges();
                Console.WriteLine("Saved! " + DateTime.UtcNow);

            }
            catch (Exception ex)
            {
            }
        }
        public void Dispose()
        {
        }
        ~GetPrices()
        {

        }
    }
}
