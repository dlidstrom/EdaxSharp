namespace Edax.Lib
{
	using System;
	using System.IO;

	public static class Bit
	{
		/// <summary>
		/// coordinate to bit table converter
		/// </summary>
		public static ulong[] X_TO_BIT = new ulong[66] {
			0x0000000000000001UL, 0x0000000000000002UL, 0x0000000000000004UL, 0x0000000000000008UL,
			0x0000000000000010UL, 0x0000000000000020UL, 0x0000000000000040UL, 0x0000000000000080UL,
			0x0000000000000100UL, 0x0000000000000200UL, 0x0000000000000400UL, 0x0000000000000800UL,
			0x0000000000001000UL, 0x0000000000002000UL, 0x0000000000004000UL, 0x0000000000008000UL,
			0x0000000000010000UL, 0x0000000000020000UL, 0x0000000000040000UL, 0x0000000000080000UL,
			0x0000000000100000UL, 0x0000000000200000UL, 0x0000000000400000UL, 0x0000000000800000UL,
			0x0000000001000000UL, 0x0000000002000000UL, 0x0000000004000000UL, 0x0000000008000000UL,
			0x0000000010000000UL, 0x0000000020000000UL, 0x0000000040000000UL, 0x0000000080000000UL,
			0x0000000100000000UL, 0x0000000200000000UL, 0x0000000400000000UL, 0x0000000800000000UL,
			0x0000001000000000UL, 0x0000002000000000UL, 0x0000004000000000UL, 0x0000008000000000UL,
			0x0000010000000000UL, 0x0000020000000000UL, 0x0000040000000000UL, 0x0000080000000000UL,
			0x0000100000000000UL, 0x0000200000000000UL, 0x0000400000000000UL, 0x0000800000000000UL,
			0x0001000000000000UL, 0x0002000000000000UL, 0x0004000000000000UL, 0x0008000000000000UL,
			0x0010000000000000UL, 0x0020000000000000UL, 0x0040000000000000UL, 0x0080000000000000UL,
			0x0100000000000000UL, 0x0200000000000000UL, 0x0400000000000000UL, 0x0800000000000000UL,
			0x1000000000000000UL, 0x2000000000000000UL, 0x4000000000000000UL, 0x8000000000000000UL,
			0, 0 // <- hack for passing move & nomove
		};

		/// <summary>
		/// Conversion array: neighbour bits
		/// </summary>
		static ulong[] NEIGHBOUR = new ulong[66] {
			0x0000000000000302UL, 0x0000000000000705UL, 0x0000000000000e0aUL, 0x0000000000001c14UL,
			0x0000000000003828UL, 0x0000000000007050UL, 0x000000000000e0a0UL, 0x000000000000c040UL,
			0x0000000000030203UL, 0x0000000000070507UL, 0x00000000000e0a0eUL, 0x00000000001c141cUL,
			0x0000000000382838UL, 0x0000000000705070UL, 0x0000000000e0a0e0UL, 0x0000000000c040c0UL,
			0x0000000003020300UL, 0x0000000007050700UL, 0x000000000e0a0e00UL, 0x000000001c141c00UL,
			0x0000000038283800UL, 0x0000000070507000UL, 0x00000000e0a0e000UL, 0x00000000c040c000UL,
			0x0000000302030000UL, 0x0000000705070000UL, 0x0000000e0a0e0000UL, 0x0000001c141c0000UL,
			0x0000003828380000UL, 0x0000007050700000UL, 0x000000e0a0e00000UL, 0x000000c040c00000UL,
			0x0000030203000000UL, 0x0000070507000000UL, 0x00000e0a0e000000UL, 0x00001c141c000000UL,
			0x0000382838000000UL, 0x0000705070000000UL, 0x0000e0a0e0000000UL, 0x0000c040c0000000UL,
			0x0003020300000000UL, 0x0007050700000000UL, 0x000e0a0e00000000UL, 0x001c141c00000000UL,
			0x0038283800000000UL, 0x0070507000000000UL, 0x00e0a0e000000000UL, 0x00c040c000000000UL,
			0x0302030000000000UL, 0x0705070000000000UL, 0x0e0a0e0000000000UL, 0x1c141c0000000000UL,
			0x3828380000000000UL, 0x7050700000000000UL, 0xe0a0e00000000000UL, 0xc040c00000000000UL,
			0x0203000000000000UL, 0x0507000000000000UL, 0x0a0e000000000000UL, 0x141c000000000000UL,
			0x2838000000000000UL, 0x5070000000000000UL, 0xa0e0000000000000UL, 0x40c0000000000000UL,
			0, 0 // <- hack for passing move & nomove
		};

		/// <summary>
		/// Count the number of bits set to one in an ulong.
		/// This is the classical popcount function.
		/// Since 2007, it is part of the instruction set of some modern CPU,
		/// (>= barcelona for AMD or >= nelhacem for Intel). Alternatively,
		/// a fast SWAR algorithm, adding bits in parallel is provided here.
		/// This function is massively used to count discs on the board,
		/// the mobility, or the stability.
		/// </summary>
		/// <param name="b">64-bit integer to count bits of.</param>
		/// <returns>the number of bits set.</returns>
		static int bit_count(ulong b)
		{
			ulong c = b
				- ((b >> 1) & 0x7777777777777777L)
				- ((b >> 2) & 0x3333333333333333L)
				- ((b >> 3) & 0x1111111111111111L);
			c = ((c + (c >> 4)) & 0x0F0F0F0F0F0F0F0FL) * 0x0101010101010101L;

			return (int)(c >> 56);
		}

		/// <summary>
		/// count the number of discs, counting the corners twice.
		/// 
		/// This is a variation of the above algorithm to count the mobility and favour
		/// the corners. This function is usefUL for move sorting.
		/// </summary>
		/// <param name="v">64-bit integer to count bits of.</param>
		/// <returns>the number of bit set, counting the corners twice.</returns>
		static int bit_weighted_count(ulong v)
		{
			ulong b;
			b = v - ((v >> 1) & 0x1555555555555515UL) + (v & 0x0100000000000001UL);
			b = ((b >> 2) & 0x3333333333333333UL) + (b & 0x3333333333333333UL);
			b = ((b >> 4) + b) & 0x0f0f0f0f0f0f0f0fUL;
			b *= 0x0101010101010101UL;

			return (int)(b >> 56);
		}

		static int[] magic = new int[64] {
			63, 0, 58, 1, 59, 47, 53, 2,
			60, 39, 48, 27, 54, 33, 42, 3,
			61, 51, 37, 40, 49, 18, 28, 20,
			55, 30, 34, 11, 43, 14, 22, 4,
			62, 57, 46, 52, 38, 26, 32, 41,
			50, 36, 17, 19, 29, 10, 13, 21,
			56, 45, 25, 31, 35, 16, 9, 12,
			44, 24, 15, 8, 23, 7, 6, 5
		};

		/// <summary>
		/// Search the first bit set.
		/// 
		/// On CPU with AMD64 or EMT64 instructions, a fast asm
		/// code is provided. Alternatively, a fast algorithm based on
		/// magic numbers is provided.
		/// </summary>
		/// <param name="b">64-bit integer.</param>
		/// <returns>the index of the first bit set.</returns>
		public static int first_bit(ulong b)
		{
			return magic[((b & (~b + 1)) * 0x07EDD5E59A4E28C2UL) >> 58];
		}

		/// <summary>
		/// Search the next bit set.
		/// 
		/// In practice, clear the first bit set and search the next one.
		/// </summary>
		/// <param name="b">64-bit integer.</param>
		/// <returns>the index of the next bit set.</returns>
		public static int next_bit(ref ulong b)
		{
			b &= b - 1;
			return first_bit(b);
		}

		/// <summary>
		/// Search the last bit set (same as log2()).
		/// 
		/// On CPU with AMD64 or EMT64 instructions, a fast asm
		/// code is provided. Alternatively, a fast algorithm based on
		/// magic numbers is provided.
		/// </summary>
		/// <param name="b">64-bit integer.</param>
		/// <returns>the index of the last bit set.</returns>
		public static int last_bit(ulong b)
		{
			b |= b >> 1;
			b |= b >> 2;
			b |= b >> 4;
			b |= b >> 8;
			b |= b >> 16;
			b |= b >> 32;
			b = (b >> 1) + 1;

			return magic[(b * 0x07EDD5E59A4E28C2UL) >> 58];
		}

		/// <summary>
		/// Transpose the ulong (symetry % A1-H8 diagonal).
		/// </summary>
		/// <param name="b">An ulong</param>
		/// <returns>The transposed ulong.</returns>
		public static ulong transpose(ulong b)
		{
			ulong t;

			t = (b ^ (b >> 7)) & 0x00aa00aa00aa00aaUL;
			b = b ^ t ^ (t << 7);
			t = (b ^ (b >> 14)) & 0x0000cccc0000ccccUL;
			b = b ^ t ^ (t << 14);
			t = (b ^ (b >> 28)) & 0x00000000f0f0f0f0UL;
			b = b ^ t ^ (t << 28);

			return b;
		}

		/// <summary>
		/// Mirror the ulong (exchange the lines A - H, B - G, C - F & D - E.).
		/// </summary>
		/// <param name="b">An ulong</param>
		/// <returns>The mirrored ulong.</returns>
		public static ulong vertical_mirror(ulong b)
		{
			b = ((b >> 8) & 0x00FF00FF00FF00FFUL) | ((b << 8) & 0xFF00FF00FF00FF00UL);
			b = ((b >> 16) & 0x0000FFFF0000FFFFUL) | ((b << 16) & 0xFFFF0000FFFF0000UL);
			b = ((b >> 32) & 0x00000000FFFFFFFFUL) | ((b << 32) & 0xFFFFFFFF00000000UL);
			return b;
		}

		/// <summary>
		/// Mirror the ulong (exchange the line 1 - 8, 2 - 7, 3 - 6 & 4 - 5).
		/// </summary>
		/// <param name="b">An ulong.</param>
		/// <returns>The mirrored ulong.</returns>
		public static ulong horizontal_mirror(ulong b)
		{
			b = ((b >> 1) & 0x5555555555555555UL) | ((b << 1) & 0xAAAAAAAAAAAAAAAAUL);
			b = ((b >> 2) & 0x3333333333333333UL) | ((b << 2) & 0xCCCCCCCCCCCCCCCCUL);
			b = ((b >> 4) & 0x0F0F0F0F0F0F0F0FUL) | ((b << 4) & 0xF0F0F0F0F0F0F0F0UL);

			return b;
		}

		/// <summary>
		/// Swap bytes of a short (little <-> big endian).
		/// </summary>
		/// <param name="s">An unsigned short.</param>
		/// <returns>The mirrored short.</returns>
		public static ushort bswap_short(ushort s)
		{
			return (ushort)(((s >> 8) & 0x00FF) | ((s << 8) & 0xFF00));
		}

		/// <summary>
		/// Mirror the unsigned int (little <-> big endian).
		/// </summary>
		/// <param name="i">An unsigned int.</param>
		/// <returns>The mirrored int.</returns>
		public static uint bswap_int(uint i)
		{
			i = ((i >> 8) & 0x00FF00FFU) | ((i << 8) & 0xFF00FF00U);
			i = ((i >> 16) & 0x0000FFFFU) | ((i << 16) & 0xFFFF0000U);
			return i;
		}

		/// <summary>
		/// Get a random set bit index.
		/// </summary>
		/// <param name="b">The ulong.</param>
		/// <param name="r">The pseudo-number generator.</param>
		/// <returns>a random bit index, or -1 if b value is zero.</returns>
		public static int get_rand_bit(ulong b, Random r)
		{
			int n = bit_count(b), x;

			if (n == 0)
				return -1;

			n = (int)Util.random_get(r) % n;
			for (x = first_bit(b); b != 0; x = next_bit(ref b))
				if (n-- == 0)
					return x;

			return -2; // impossible.
		}

		/// <summary>
		/// Print an ulong as a board.
		/// 
		/// Write a 64-bit long number as an Othello board.
		/// </summary>
		/// <param name="b">The ulong.</param>
		/// <param name="f">Output stream.</param>
		public static void bitboard_write(ulong b, TextWriter f)
		{
			int i, j, x;
			string color = ".X";

			f.Write("  A B C D E F G H\n");
			for (i = 0; i < 8; ++i)
			{
				f.Write((char)(i + '1'));
				f.Write(' ');
				for (j = 0; j < 8; ++j)
				{
					x = i * 8 + j;
					f.Write(color[(int)((b >> x) & 1)]);
					f.Write(' ');
				}
				f.Write((char)(i + '1'));
				f.Write('\n');
			}
			f.Write("  A B C D E F G H\n");
		}
	}
}
