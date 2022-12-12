using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace ExchangeRate.API
{
    internal class RequestService : IRequestRateService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly HttpClient client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan };

        public async Task<string> GetRateAsync(string url)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            var requestSend = client.SendAsync(request);

            var responce = new HttpResponseMessage();

            try
            {
                responce = await requestSend;
            }
            catch(Exception e)
            {
                log.Info("ERROR ==================================");
                log.Error($"Error during requesting remote server: {e.Message}");
                log.Info($"Source: {e.Source}");
                log.Info($"Date: {responce.Headers.Date}");
                
                //log.Info($"{e.StackTrace}");

                throw e;
            }

            var result = responce.Content.ReadAsStringAsync();

            switch (responce.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return await result;

                case System.Net.HttpStatusCode.InternalServerError:
                    return await Task.FromException<string>(new Exception($"Внутренняя ошибка удаленного сервера. Message: {await responce.Content.ReadAsStringAsync()}"));
                
                case System.Net.HttpStatusCode.NotFound:
                    return await Task.FromException<string>(new Exception($"Запрошенный курс не найден (возможны проблемы с сайтом НБРБ). Message: {await responce.Content.ReadAsStringAsync()}"));
                
                default:
                    return await Task.FromException<string>(new Exception($"Status: {responce.StatusCode}. Message: {await responce.Content.ReadAsStringAsync()}"));
            }
        }
    }
}
