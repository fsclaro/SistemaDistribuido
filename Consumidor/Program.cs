using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Consumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusClient = new SubscriptionClient(
                    "Endpoint=sb://aceleracaoavanade.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pa7LRhiDmi7ik4m812/M8CebQ6rBFJPCe4ZqduNhOm0=",
                    "pagamentofeito",
                    "PagamentoFeitoServicoB"
                );

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            Console.WriteLine("Consumidor - Recebendo a mensagem...");
            serviceBusClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

            while (true) { }
        }





        private static Task ProcessMessageAsync(Message message, CancellationToken arg2)
        {
            var pagamentoFeito = message.Body.ParseJson<PagamentoFeito>();

            Console.WriteLine(pagamentoFeito.ToString());

            return Task.CompletedTask;
        }





        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }
    }





    internal class PagamentoFeito
    {
        public string NumeroCartao { get; set; }
        public decimal Valor { get; set; }


        public override string ToString()
        {
            return $"Cartao {NumeroCartao}, Valor {Valor}";
            ;
        }
    }




    public static class Utils
    {
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };



        public static T ParseJson<T>(this byte[] json)
        {
            if (json == null || json.Length == 0) return default;

            var result = JsonConvert.DeserializeObject<T>(Utf8NoBom.GetString(json), JsonSettings);

            return result;
        }
    }
}
