using System;
using AutoFixture;
using FluentAssertions;
using Marketplace.Infrastructure.JsonNet;
using Xunit;

namespace Marketplace.Tests.Infrastructure
{
    public class JsonNetSerializerTests
    {
        public JsonNetSerializerTests() => AutoFixture = new Fixture();

        private Fixture AutoFixture { get; }

        private class GameOver
        {
            public Guid GameId { get; set; }
            public Guid PlayerId { get; set; }
            public long Highscore { get; set; }
        }

        [Fact]
        public void can_serialize_and_deserialize()
        {
            // arrange
            var sut = new JsonNetSerializer();
            var expectedResult = AutoFixture.Create<GameOver>();

            // act
            var result = (GameOver) sut.Deserialize(sut.Serialize(expectedResult), typeof(GameOver));

            //assert 
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
