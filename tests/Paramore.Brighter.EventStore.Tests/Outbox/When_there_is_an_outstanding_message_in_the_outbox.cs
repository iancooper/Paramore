#region Licence

/* The MIT License (MIT)
Copyright © 2019 Jonny Olliff-Lee <jonny.ollifflee@gmail.com>

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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Paramore.Brighter.Outbox.EventStore;
using Xunit;

namespace Paramore.Brighter.EventStore.Tests.Outbox
{
    [Trait("Category", "EventStore")]
    [Collection("EventStore")]
    public class OutStandingMessageTests : EventStoreFixture
    {
        [Fact]
        public void When_there_is_an_outstanding_message_in_the_outbox()
        {
            // arrange
            var eventStoreOutbox = new EventStoreOutbox(Connection);
            
            var args = new Dictionary<string, object> {{Globals.StreamArg, StreamName}};
            
            var outstandingMessage = CreateMessage(0, StreamName);
            var dispatchedMessage = CreateMessage(1, StreamName);
            var outstandingRecentMessage = CreateMessage(3, StreamName);
            
            eventStoreOutbox.Add(outstandingMessage);
            eventStoreOutbox.Add(dispatchedMessage);
            Task.Delay(1000).Wait();
            eventStoreOutbox.MarkDispatched(dispatchedMessage.Id, DateTime.UtcNow, args);
            eventStoreOutbox.Add(outstandingRecentMessage);

            // act
            var messages = eventStoreOutbox.OutstandingMessages(500, 100, 1, args);

            // assert
            messages.Should().BeEquivalentTo(outstandingMessage);
        }
        
        [Fact]
        public void When_null_args_are_supplied()
        {
            // arrange
            var eventStoreOutbox = new EventStoreOutbox(Connection);
            
            // act
            Action getWithoutArgs = () => eventStoreOutbox.OutstandingMessages(500, 100, 1);
            
            // assert
            getWithoutArgs.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void When_null_stream_arg_supplied()
        {
            // arrange
            var eventStoreOutbox = new EventStoreOutbox(Connection);
            var args = new Dictionary<string, object> {{Globals.StreamArg, null}};
            
            // act
            Action getWithoutArgs = () => eventStoreOutbox.OutstandingMessages(500, 100, 1, args);
            
            // assert
            getWithoutArgs.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_empty_args_are_supplied()
        {
            // arrange
            var eventStoreOutbox = new EventStoreOutbox(Connection);
            var args = new Dictionary<string, object>();
            
            // act
            Action getWithoutArgs = () => eventStoreOutbox.OutstandingMessages(500, 100, 1, args);
            
            // assert
            getWithoutArgs.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void When_wrong_args_are_supplied()
        {
            // arrange
            var eventStoreOutbox = new EventStoreOutbox(Connection);
            var args = new Dictionary<string, object> { { "Foo", "Bar" }};
            
            // act
            Action getWithoutArgs = () => eventStoreOutbox.OutstandingMessages(500, 100, 1, args);
            
            // assert
            getWithoutArgs.Should().Throw<ArgumentException>();
        }
    }
}
