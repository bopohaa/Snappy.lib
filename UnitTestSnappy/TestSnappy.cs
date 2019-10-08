using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestSnappy
{
    [TestClass]
    public class TestSnappy
    {
        [TestMethod]
        public void TestCompress()
        {
            var source = "У попа была собака он её любил, она съела кусок мяса он её убил.";
            var sourceData = System.Text.Encoding.UTF8.GetBytes(source);
            var compressData = new byte[1024];
            var uncompressData = new byte[1024];

            var compressedSize = SnappyLib.Snappy.Compress(sourceData, 0, sourceData.Length, compressData, 0, compressData.Length);
            Assert.AreEqual(100, compressedSize);

            var uncompressedSize = SnappyLib.Snappy.Uncompress(compressData, 0, compressedSize, uncompressData, 0, uncompressData.Length);
            Assert.AreEqual(sourceData.Length, uncompressedSize);
            Assert.IsTrue(new Span<byte>(sourceData).SequenceEqual(new Span<byte>(uncompressData, 0, uncompressedSize)));

            var maxCompressedSize = SnappyLib.Snappy.MaxCompressedLength(sourceData.Length);
            Assert.IsTrue(maxCompressedSize >= compressedSize);

            var calculatedUncompressedSize = SnappyLib.Snappy.UncompressedLength(compressData, 0, compressedSize);
            Assert.AreEqual(sourceData.Length, calculatedUncompressedSize);

            var dcompressedDataValidationStatus = SnappyLib.Snappy.ValidateCompressedBuffer(compressData, 0, compressedSize);
            Assert.AreEqual(dcompressedDataValidationStatus, SnappyLib.SnappyStatusEnum.SNAPPY_OK);

        }
    }
}
