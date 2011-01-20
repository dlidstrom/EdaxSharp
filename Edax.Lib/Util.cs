namespace Edax.Lib
{
	using System;

	public class Random
	{
		public ulong x;
	}

	public static class Util
	{
		/// <summary>
		/// Pseudo-random number generator.
		/// 
		/// A good pseudo-random generator (derived from rand48 or Java's one) to set the
		/// hash codes.
		/// </summary>
		/// <param name="random">Pseudo-Random generator state.</param>
		/// <returns>a 64-bits pseudo-random unsigned int integer.</returns>
		public static ulong random_get(Random random)
		{
			ulong MASK48 = 0xFFFFFFFFFFFFul;
			ulong A = 0x5DEECE66Dul;
			ulong B = 0xBul;
			ulong r;

			random.x = ((A * random.x + B) & MASK48);
			r = random.x >> 16;
			random.x = ((A * random.x + B) & MASK48);
			return (r << 32) | (random.x >> 16);
		}
	}
}
