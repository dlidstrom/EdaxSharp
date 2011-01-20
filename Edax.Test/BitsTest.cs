namespace Edax.Test
{
	using System;
	using NUnit.Framework;
	using Edax.Lib;

	[TestFixture]
	public class BitsTest
	{
		[Test]
		public void FirstBit()
		{
			Assert.AreEqual(0, Bit.first_bit(1));
			Assert.AreEqual(1, Bit.first_bit(2));
			Assert.AreEqual(5, Bit.first_bit(32));
			Assert.AreEqual(8, Bit.first_bit(256));
			Assert.AreEqual(10, Bit.first_bit(1024));
		}
	}
}
