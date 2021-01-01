using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Paramore.Brighter
{
    public class JsonMessageMapper<T> : BaseMessageMapper<T> where T : class, IRequest
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public JsonMessageMapper(IRequestContext requestContext, RoutingKey routingKey = null,
            Func<T, string> routingKeyFunc = null) : base(requestContext, routingKey, routingKeyFunc)
        {
        }

        protected override MessageBody CreateMessageBody(T request)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    _serializer.Serialize(jsonTextWriter, request, typeof(T));
                }

                return new MessageBody(memoryStream.ToArray(), "JSON");
            }
        }

        protected override T CreateType(Message message)
        {
            using (MemoryStream memoryStream = new MemoryStream(message.Body.Bytes))
            using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
            using (JsonReader reader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<T>(reader);
            }
        }
    }

    public class JsonMessageMapper2<T> : BaseMessageMapper<T> where T : class, IRequest
    {
        public JsonMessageMapper2(IRequestContext requestContext, RoutingKey routingKey = null,
            Func<T, string> routingKeyFunc = null) : base(requestContext, routingKey, routingKeyFunc)
        {
        }

        protected override MessageBody CreateMessageBody(T request)
        {
                return new MessageBody(JsonConvert.SerializeObject(request), "JSON");
        }

        protected override T CreateType(Message message)
        {
            return JsonConvert.DeserializeObject<T>(message.Body.Value);
        }
    }
}
