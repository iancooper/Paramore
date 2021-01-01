using System;
using BenchmarkDotNet.Attributes;

namespace Paramore.Brighter.Perf
{
    public class Benchmark
    {
        private readonly JsonMessageMapper<TestCommand> _jsonMessageMapper;
        private readonly JsonMessageMapper2<TestCommand> _mapper2;
        private readonly Message _message;
        private readonly TestCommand _testCommand;

        public Benchmark()
        {
            RequestContext requestContext = new RequestContext();
            _jsonMessageMapper = new JsonMessageMapper<TestCommand>(requestContext);
            _mapper2 = new JsonMessageMapper2<TestCommand>(requestContext);

            DateTime dateTime = DateTime.UtcNow;
            _testCommand = new TestCommand
            {
                Message = "This is a message",
                Number = 999,
                DateNow = dateTime
            };

            string body =
                "{\"message\":\"This is a message\",\"number\":999,\"dateNow\":\"2019-04-09T15:06:56.7623017Z\",\"id\":\"7d9120b9-a18e-43ac-a63e-8201a43ea623\"}";
            Guid correlationId = Guid.NewGuid();
            _message = new Message(
                new MessageHeader(new Guid("7d9120b9-a18e-43ac-a63e-8201a43ea623"), "Blah", MessageType.MT_COMMAND,
                    correlationId), new MessageBody(body));
        }

        [Benchmark]
        public void MapToMessageJsonByte()
        {
            _jsonMessageMapper.MapToMessage(_testCommand);
        }

        [Benchmark]
        public void MapFromMessageJsonByte()
        {
            _jsonMessageMapper.MapToRequest(_message);
        }

        [Benchmark]
        public void MapToMessageJsonString()
        {
            _mapper2.MapToMessage(_testCommand);
        }

        [Benchmark]
        public void MapFromMessageJsonString()
        {
            _mapper2.MapToRequest(_message);
        }
    }
}
