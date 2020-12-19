using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Produtor
{
    class Program
    {
        static void Main(string[] args)
        {
            // definindo a mensagem
            var pagamentofeito = new PagamentoFeito { NumeroCartao = "123456789012", Valor = 1502.32M };

            // definindo o service bus
            var serviceBusClient = new TopicClient(
                "Endpoint=sb://aceleracaoavanade.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pa7LRhiDmi7ik4m812/M8CebQ6rBFJPCe4ZqduNhOm0=",
                "pagamentofeito");

            // define uma mensagem
            var message = new Message(pagamentofeito.ToJsonBytes());
            message.ContentType = "application/json";
            message.UserProperties.Add("CorrelationId", Guid.NewGuid().ToString());

            while (true)
            {
                Console.ReadKey();

                Console.WriteLine("Produtor - Enviando a mensagem...");

                // envia a mensagem
                serviceBusClient.SendAsync(message);
            }

            // aguarda o usuário dar Enter
            Console.ReadKey();
            Console.WriteLine("Produtor - Encerrando.");
        }
    }


    internal class PagamentoFeito
    {
        public string NumeroCartao { get; set; }
        public decimal Valor { get; set; }
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



        /// <summary>
        /// Converts the object to json bytes.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns></returns>
        public static byte[] ToJsonBytes(this object source)
        {
            if (source == null)
                return null;

            var instring = JsonConvert.SerializeObject(source, Formatting.Indented, JsonSettings);
            return Utf8NoBom.GetBytes(instring);
        }

    }

}

