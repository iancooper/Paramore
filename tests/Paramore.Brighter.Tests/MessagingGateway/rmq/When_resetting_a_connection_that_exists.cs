﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using FakeItEasy;
using Paramore.Brighter.MessagingGateway.RMQ;
using RabbitMQ.Client;
using Xunit;

namespace Paramore.Brighter.Tests.MessagingGateway.RMQ
{
    [Collection("RMQ")]
    [Trait("Category", "RMQ")]
    public class RMQMessageGatewayConnectionPoolResetConnectionExists
    {
        private readonly RMQMessageGatewayConnectionPool _connectionPool;
        private readonly IEndpointResolver _endpointResolver;

        public RMQMessageGatewayConnectionPoolResetConnectionExists()
        {
            _connectionPool = new RMQMessageGatewayConnectionPool("MyConnectionName", 7);
            _endpointResolver = A.Fake<IEndpointResolver>();

            var connectionFactory = new ConnectionFactory { HostName = "localhost" };

            _connectionPool.GetConnection(connectionFactory);
        }

        [Fact]
        public void When_resetting_a_connection_that_exists()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                EndpointResolverFactory = _ => _endpointResolver
            };

            _connectionPool.ResetConnection(connectionFactory);

            A.CallTo(() => _endpointResolver.All()).MustNotHaveHappened();
        }
    }
}
