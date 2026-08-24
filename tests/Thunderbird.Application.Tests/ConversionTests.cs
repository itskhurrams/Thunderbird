using Thunderbird.Infrastructure.Common;

namespace Thunderbird.Application.Tests {
    public class ConversionTests {
        [Fact]
        public void ToInt_ReturnsZero_ForNull() {
            Assert.Equal(0, Conversion.ToInt(null));
        }

        [Fact]
        public void ToInt_ReturnsZero_ForDBNull() {
            Assert.Equal(0, Conversion.ToInt(DBNull.Value));
        }

        [Fact]
        public void ToInt_ParsesValidValue() {
            Assert.Equal(42, Conversion.ToInt("42"));
        }

        [Fact]
        public void ToInt_ReturnsZero_ForUnparsableValue() {
            Assert.Equal(0, Conversion.ToInt("not-a-number"));
        }

        [Fact]
        public void ToBool_ParsesTrueFalseStrings() {
            Assert.True(Conversion.ToBool("true"));
            Assert.False(Conversion.ToBool("false"));
        }

        [Fact]
        public void ToBool_ParsesIntegerFallback() {
            Assert.True(Conversion.ToBool(1));
            Assert.False(Conversion.ToBool(0));
        }

        [Fact]
        public void ToDateTime_ReturnsSentinel_ForNull() {
            Assert.Equal(new DateTime(1900, 1, 1), Conversion.ToDateTime(null));
        }

        [Fact]
        public void ToGuid_ReturnsEmpty_ForInvalidValue() {
            Assert.Equal(Guid.Empty, Conversion.ToGuid("not-a-guid"));
        }

        [Fact]
        public void ToString_ReturnsEmpty_ForNull() {
            Assert.Equal(string.Empty, Conversion.ToString(null));
        }
    }
}
