/**
 * @file count_last_flip.c
 *
 *
 * A function is provided to count the number of fipped disc of the last move
 * for each square of the board. These functions are gathered into an array of
 * functions, so that a fast access to each function is allowed. The generic
 * form of the function take as input the player bitboard and return twice
 * the number of flipped disc of the last move.
 *
 * The basic principle is to read into an array a precomputed result. Doing
 * this is easy for a single line ; as we can use arrays of the form:
 *  - COUNT_FLIP[square where we play,8-bits disc pattern].
* The problem is thus to convert any line of a 64-bits disc pattern into an
 * 8-bits disc pattern. A fast way to do this is to select the right line,
 * with a bit-mask, to gather the masked-bits into a continuous set by a simple
 * multiplication and to right-shift the result to scale it into a number
 * between 0 and 255.
 * Once we get our 8-bits disc patterns, we directly get the number of
 * flipped discs from the precomputed array, and add them from each flipping
 * lines.
 * For optimization purpose, the value returned is twice the number of flipped
 * disc, to facilitate the computation of disc difference.
 *
 * With Modifications by Valéry ClaudePierre (merging diagonals).
 *
 * @date 1998 - 2010
 * @author Richard Delorme
 * @version 4.1
 * 
 */

namespace Edax.Lib
{
	using System;

	public static class CountLastFlip
	{
		/** precomputed count flip array */
		static byte[,] COUNT_FLIP = new byte[8, 256] {
			{
				 0,  0,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				 8,  8,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				10, 10,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				 8,  8,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				12, 12,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				 8,  8,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				10, 10,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
				 8,  8,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,  6,  6,  0,  0,  2,  2,  0,  0,  4,  4,  0,  0,  2,  2,  0,  0,
			},
			{
				 0,  0,  0,  0,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				 6,  6,  6,  6,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				 8,  8,  8,  8,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				 6,  6,  6,  6,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				10, 10, 10, 10,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				 6,  6,  6,  6,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				 8,  8,  8,  8,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
				 6,  6,  6,  6,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,  4,  4,  4,  4,  0,  0,  0,  0,  2,  2,  2,  2,  0,  0,  0,  0,
			},
			{
				 0,  2,  0,  0,  0,  2,  0,  0,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 4,  6,  4,  4,  4,  6,  4,  4,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 6,  8,  6,  6,  6,  8,  6,  6,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 4,  6,  4,  4,  4,  6,  4,  4,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 8, 10,  8,  8,  8, 10,  8,  8,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 4,  6,  4,  4,  4,  6,  4,  4,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 6,  8,  6,  6,  6,  8,  6,  6,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
				 4,  6,  4,  4,  4,  6,  4,  4,  0,  2,  0,  0,  0,  2,  0,  0,  2,  4,  2,  2,  2,  4,  2,  2,  0,  2,  0,  0,  0,  2,  0,  0,
			},
			{
				 0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 2,  6,  4,  4,  2,  2,  2,  2,  2,  6,  4,  4,  2,  2,  2,  2,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 4,  8,  6,  6,  4,  4,  4,  4,  4,  8,  6,  6,  4,  4,  4,  4,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 2,  6,  4,  4,  2,  2,  2,  2,  2,  6,  4,  4,  2,  2,  2,  2,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 6, 10,  8,  8,  6,  6,  6,  6,  6, 10,  8,  8,  6,  6,  6,  6,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 2,  6,  4,  4,  2,  2,  2,  2,  2,  6,  4,  4,  2,  2,  2,  2,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 4,  8,  6,  6,  4,  4,  4,  4,  4,  8,  6,  6,  4,  4,  4,  4,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
				 2,  6,  4,  4,  2,  2,  2,  2,  2,  6,  4,  4,  2,  2,  2,  2,  0,  4,  2,  2,  0,  0,  0,  0,  0,  4,  2,  2,  0,  0,  0,  0,
			},
			{
				 0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,
				 2,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,
				 4, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  4, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,
				 0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,
				 2,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  6,  4,  4,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,
			},
			{
				 0,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 2, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 2, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  8,  6,  6,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
			},
			{
				 0, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0, 10,  8,  8,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
			},
			{
				 0, 12, 10, 10,  8,  8,  8,  8,  6,  6,  6,  6,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
				 2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0, 12, 10, 10,  8,  8,  8,  8,  6,  6,  6,  6,  6,  6,  6,  6,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,
				 2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
				 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
			},
		};

		/**
		 * Count last flipped discs when playing on square A1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, P & 0xff];
			n_flipped += COUNT_FLIP[0, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, P & 0xff];
			n_flipped += COUNT_FLIP[1, ((P & 0x0080402010080402UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, P & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x0000804020110A04UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, P & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0000008041221408UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, P & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0000000182442810UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, P & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0000010204885020UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, P & 0xff];
			n_flipped += COUNT_FLIP[6, ((P & 0x0001020408102040UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H1.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H1(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[0, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, P & 0xff];
			n_flipped += COUNT_FLIP[7, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[0, ((P & 0x4020100804020100UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[1, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x00804020110A0400UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0000804122140800UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0000018244281000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0001020488502000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[6, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H2.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H2(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[1, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, (P >> 8) & 0xff];
			n_flipped += COUNT_FLIP[7, ((P & 0x0204081020408000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[2, ((((P & 0x2010080402010204UL) + 0x6070787C7E7F7E7C) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[2, ((((P & 0x4020100804020408UL) + 0x406070787C7E7C78) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x0000000102040810UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[2, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0000010204081020UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[3, ((P & 0x0080402010080402UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0001020408102040UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[4, ((P & 0x0000804020100804UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[5, ((P & 0x0000008040201008UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[2, ((((P & 0x0204081020402010UL) + 0x7E7C787060406070) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H3.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H3(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[2, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, (P >> 16) & 0xff];
			n_flipped += COUNT_FLIP[2, ((((P & 0x0408102040804020UL) + 0x7C78706040004060) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[3, ((((P & 0x1008040201020408UL) + 0x70787C7E7F7E7C78) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[3, ((((P & 0x2010080402040810UL) + 0x6070787C7E7C7870) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x0000010204081020UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[2, ((P & 0x4020100804020100UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0001020408102040UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[3, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[4, ((P & 0x0080402010080402UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0204081020408000UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[5, ((P & 0x0000804020100804UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[3, ((((P & 0x0408102040201008UL) + 0x7C78706040607078) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H4.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H4(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[3, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, (P >> 24) & 0xff];
			n_flipped += COUNT_FLIP[3, ((((P & 0x0810204080402010UL) + 0x7870604000406070) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[4, ((((P & 0x0804020102040810UL) + 0x787C7E7F7E7C7870) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[4, ((((P & 0x1008040204081020UL) + 0x70787C7E7C787060) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x0001020408102040UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[2, ((P & 0x2010080402010000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[3, ((P & 0x4020100804020100UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0204081020408000UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[4, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0408102040800000UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[5, ((P & 0x0080402010080402UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[4, ((((P & 0x0810204020100804UL) + 0x787060406070787C) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H5.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H5(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[4, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, (P >> 32) & 0xff];
			n_flipped += COUNT_FLIP[4, ((((P & 0x1020408040201008UL) + 0x7060400040607078) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[5, ((((P & 0x0402010204081020UL) + 0x7C7E7F7E7C787060) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[5, ((((P & 0x0804020408102040UL) + 0x787C7E7C78706040) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[2, ((P & 0x1008040201000000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0204081020408000UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[3, ((P & 0x2010080402010000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0408102040800000UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[4, ((P & 0x4020100804020100UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0810204080000000UL) * 0x0101010101010101UL) >> 56];
			n_flipped += COUNT_FLIP[5, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[5, ((((P & 0x1020402010080402UL) + 0x7060406070787C7E) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H6.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H6(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[5, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, (P >> 40) & 0xff];
			n_flipped += COUNT_FLIP[5, ((((P & 0x2040804020100804UL) + 0x604000406070787C) & 0x8080808080808080) * 0x0002040810204081) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[0, ((P & 0x0001020408102040UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[1, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[2, ((P & 0x00040A1120408000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[3, ((P & 0x0008142241800000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[4, ((P & 0x0010284482010000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[5, ((P & 0x0020508804020100UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[6, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H7.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H7(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[6, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, (P >> 48) & 0xff];
			n_flipped += COUNT_FLIP[7, ((P & 0x0080402010080402UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square A8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_A8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x0101010101010101UL) * 0x0102040810204080UL) >> 56];
			n_flipped += COUNT_FLIP[0, P >> 56];
			n_flipped += COUNT_FLIP[0, ((P & 0x0102040810204080UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square B8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_B8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x0202020202020202UL) * 0x0081020408102040UL) >> 56];
			n_flipped += COUNT_FLIP[1, P >> 56];
			n_flipped += COUNT_FLIP[1, ((P & 0x0204081020408000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square C8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_C8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x0404040404040404UL) * 0x0040810204081020UL) >> 56];
			n_flipped += COUNT_FLIP[2, P >> 56];
			n_flipped += COUNT_FLIP[2, ((P & 0x040A112040800000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square D8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_D8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x0808080808080808UL) * 0x0020408102040810UL) >> 56];
			n_flipped += COUNT_FLIP[3, P >> 56];
			n_flipped += COUNT_FLIP[3, ((P & 0x0814224180000000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square E8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_E8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x1010101010101010UL) * 0x0010204081020408UL) >> 56];
			n_flipped += COUNT_FLIP[4, P >> 56];
			n_flipped += COUNT_FLIP[4, ((P & 0x1028448201000000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square F8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_F8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x2020202020202020UL) * 0x0008102040810204UL) >> 56];
			n_flipped += COUNT_FLIP[5, P >> 56];
			n_flipped += COUNT_FLIP[5, ((P & 0x2050880402010000UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square G8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_G8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x4040404040404040UL) * 0x0004081020408102UL) >> 56];
			n_flipped += COUNT_FLIP[6, P >> 56];
			n_flipped += COUNT_FLIP[6, ((P & 0x4020100804020100UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when playing on square H8.
		 *
		 * @param P player's disc pattern.
		 * @return flipped disc count.
		 */
		static int count_last_flip_H8(ulong P)
		{
			int n_flipped;

			n_flipped = COUNT_FLIP[7, ((P & 0x8080808080808080UL) * 0x0002040810204081UL) >> 56];
			n_flipped += COUNT_FLIP[7, P >> 56];
			n_flipped += COUNT_FLIP[7, ((P & 0x8040201008040201UL) * 0x0101010101010101UL) >> 56];

			return n_flipped;
		}

		/**
		 * Count last flipped discs when plassing.
		 *
		 * @param P player's disc pattern (unused).
		 * @return zero.
		 */
		static int count_last_flip_pass(ulong P)
		{
			return 0;
		}

		/** Array of functions to count flipped discs of the last move */
		public static Func<ulong, int>[] count_last_flip = new Func<ulong, int>[65] {
			new Func<ulong, int>(count_last_flip_A1), new Func<ulong, int>(count_last_flip_B1), new Func<ulong, int>(count_last_flip_C1), new Func<ulong, int>(count_last_flip_D1),
			new Func<ulong, int>(count_last_flip_E1), new Func<ulong, int>(count_last_flip_F1), new Func<ulong, int>(count_last_flip_G1), new Func<ulong, int>(count_last_flip_H1),
			new Func<ulong, int>(count_last_flip_A2), new Func<ulong, int>(count_last_flip_B2), new Func<ulong, int>(count_last_flip_C2), new Func<ulong, int>(count_last_flip_D2),
			new Func<ulong, int>(count_last_flip_E2), new Func<ulong, int>(count_last_flip_F2), new Func<ulong, int>(count_last_flip_G2), new Func<ulong, int>(count_last_flip_H2),
			new Func<ulong, int>(count_last_flip_A3), new Func<ulong, int>(count_last_flip_B3), new Func<ulong, int>(count_last_flip_C3), new Func<ulong, int>(count_last_flip_D3),
			new Func<ulong, int>(count_last_flip_E3), new Func<ulong, int>(count_last_flip_F3), new Func<ulong, int>(count_last_flip_G3), new Func<ulong, int>(count_last_flip_H3),
			new Func<ulong, int>(count_last_flip_A4), new Func<ulong, int>(count_last_flip_B4), new Func<ulong, int>(count_last_flip_C4), new Func<ulong, int>(count_last_flip_D4),
			new Func<ulong, int>(count_last_flip_E4), new Func<ulong, int>(count_last_flip_F4), new Func<ulong, int>(count_last_flip_G4), new Func<ulong, int>(count_last_flip_H4),
			new Func<ulong, int>(count_last_flip_A5), new Func<ulong, int>(count_last_flip_B5), new Func<ulong, int>(count_last_flip_C5), new Func<ulong, int>(count_last_flip_D5),
			new Func<ulong, int>(count_last_flip_E5), new Func<ulong, int>(count_last_flip_F5), new Func<ulong, int>(count_last_flip_G5), new Func<ulong, int>(count_last_flip_H5),
			new Func<ulong, int>(count_last_flip_A6), new Func<ulong, int>(count_last_flip_B6), new Func<ulong, int>(count_last_flip_C6), new Func<ulong, int>(count_last_flip_D6),
			new Func<ulong, int>(count_last_flip_E6), new Func<ulong, int>(count_last_flip_F6), new Func<ulong, int>(count_last_flip_G6), new Func<ulong, int>(count_last_flip_H6),
			new Func<ulong, int>(count_last_flip_A7), new Func<ulong, int>(count_last_flip_B7), new Func<ulong, int>(count_last_flip_C7), new Func<ulong, int>(count_last_flip_D7),
			new Func<ulong, int>(count_last_flip_E7), new Func<ulong, int>(count_last_flip_F7), new Func<ulong, int>(count_last_flip_G7), new Func<ulong, int>(count_last_flip_H7),
			new Func<ulong, int>(count_last_flip_A8), new Func<ulong, int>(count_last_flip_B8), new Func<ulong, int>(count_last_flip_C8), new Func<ulong, int>(count_last_flip_D8),
			new Func<ulong, int>(count_last_flip_E8), new Func<ulong, int>(count_last_flip_F8), new Func<ulong, int>(count_last_flip_G8), new Func<ulong, int>(count_last_flip_H8),
			new Func<ulong, int>(count_last_flip_pass)
		};
	}
}
