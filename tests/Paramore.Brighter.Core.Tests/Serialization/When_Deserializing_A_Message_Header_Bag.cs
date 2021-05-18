﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Paramore.Brighter.Core.Tests.Serialization
{
    public class BagHelper 
    {
        //We use a Dictionary<string, object> for a header. System.Json.Text will serialize this as expected
        //but when deserializing it will return a a Dictionary<string, object> where the object is a JsonElement
        //not a primitive/reference type. 
        //The goal here is to convert back
        [Fact]
         public void When_deserializing_a_message_header_bag()
         {
             //Arrange
             var header = new MessageHeader(
                 messageId: Guid.NewGuid(),
                 topic: "MyTopic",
                 messageType: MessageType.MT_EVENT,
                 timeStamp: DateTime.UtcNow,
                 correlationId: Guid.NewGuid());
             
             var expectedBag = new Dictionary<string, object>
             {
                 {"MyStringKey", "A string value"},
                 {"MyDateTimeKey", DateTime.UtcNow},
                 {"MyIntegerKey", 123},
                 {"MyDecimalKey", 123.56},
                 {"MyBooleanKeyTrue", true},
                 {"MyBooleanKey", false},
                 {"MyArrayKey", new int[]{1,2,3,4,}}
             };

             foreach (var key in expectedBag.Keys)
             {
                header.Bag.Add(key, expectedBag[key]);
             }

             var json = JsonSerializer.Serialize(header);
             
             //Act
             MessageHeader deserializedHeader = JsonSerializer.Deserialize<MessageHeader>(json);
             //fix the headers to pass
             
             //Assert
             foreach (var key in expectedBag.Keys)
             {
                 if (key != "MyArrayKey") deserializedHeader.Bag[key].Should().Be(expectedBag[key]);
                 if (key == "MyArrayKey")
                 {
                     var expectedVals = (int[])expectedBag[key];
                     var providedVals = (List<Object>)deserializedHeader.Bag[key];
                     
                     for (int i = 0; i < 4; i++)
                     {
                         expectedVals[i].Should().Be((int)providedVals[i]);
                     }
                 }
             }

         }
    }
}
