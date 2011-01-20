namespace Edax.Lib
{
	using System;
	using System.IO;
	using System.Text;

	public class Board
	{
		ulong player, opponent;     /**< bitboard representation */

		/** edge stability global data */
		static int[,] edge_stability;

		/**
		 * @brief Swap players.
		 *
		 * Swap players, i.e. change player's turn.
		 *
		 * @param board board
		 */
		public void swap_players()
		{
			ulong tmp = player;
			player = opponent;
			opponent = tmp;
		}

/**
 * @brief Set a board from a string description.
 *
 * Read a standardized string (See http://www.nada.kth.se/~gunnar/download2.html
 * for details) and translate it into our internal Board structure.
 *
 * @param board the board to set
 * @param string string describing the board
 */
		public Board(string @string)
{
	int i;
	int s = 0;

	player = opponent = 0;
	for (i = Const.A1; i <= Const.H8; ++i) {
		if (@string[s] == '\0') break;
		switch (@string[s].ToString().ToLower()) {
		case "b":
		case "x":
		case "*":
			player |= Bit.X_TO_BIT[i];
			break;
		case "o":
		case "w":
			opponent |= Bit.X_TO_BIT[i];
			break;
		case "-":
		case ".":
			break;
		default:
			i--;
			break;
		}
		++s;
	}
	check();

	for (;@string[s] != '\0'; ++s) {
		switch (@string[s].ToString().ToLower()) {
		case "b":
		case "x":
		case "*":
				break;
		case "o":
		case "w":
			swap_players();
				break;
		default:
			break;
		}
	}

	Global.warn("set: bad string input\n");
		}

/**
 * @brief Set a board to the starting position.
 *
 * @param board the board to initialize
 */
public Board()
{
	player   = 0x0000000810000000ul; // BLACK
	opponent = 0x0000001008000000ul; // WHITE
}

public Board(ulong player, ulong opponent)
{
	this.player = player;
	this.opponent = opponent;
	check();
}

/**
 * @brief Check board consistency
 *
 * @param board the board to initialize
 */
void check()
{
#if DEBUG
	if(player!=0 & opponent!=0) {
		error("Two discs on the same square?\n");
		print(board, BLACK, stderr);
		bitboard_write(board.player, stderr);
		bitboard_write(board.opponent, stderr);
		abort();
	}

	// empty center ?
	if (((board.player|board.opponent) & 0x0000001818000000ul) != 0x0000001818000000ul) {
		error("Empty center?\n");
		print(board, BLACK, stderr);
	}
#endif // DEBUG
}

/**
 * @brief Compare two board
 *
 * @param b1 first board
 * @param b2 second board
 * @return -1, 0, 1
 */
static int compare(Board b1, Board b2)
{
	if (b1.player > b2.player) return 1;
	else if (b1.player < b2.player) return -1;
	else if (b1.opponent > b2.opponent) return 1;
	else if (b1.opponent < b2.opponent) return -1;
	else return 0;
}

/**
 * @brief Compare two board for equality
 *
 * @param b1 first board
 * @param b2 second board
 * @return true if both board are equal
 */
static bool equal(Board b1, Board b2)
{
	return (b1.player == b2.player && b1.opponent == b2.opponent);
}

/**
 * @brief symetric board
 *
 * @param board input board
 * @param s symetry
 * @param sym symetric output board
 */
public Board symetry(int s)
{
	ulong player = this.player;
	ulong opponent = this.opponent;

	if ((s & 1)!=0) {
		player = Bit.horizontal_mirror(player);
		opponent = Bit.horizontal_mirror(opponent);
	}
	if ((s & 2)!=0) {
		player = Bit.vertical_mirror(player);
		opponent = Bit.vertical_mirror(opponent);
	}
	if ((s & 4)!=0) {
		player = Bit.transpose(player);
		opponent = Bit.transpose(opponent);
	}

	return new Board(player, opponent);
}

/**
 * @brief unique board
 *
 * Compute a board unique from all its possible symertries.
 *
 * @param board input board
 * @param unique output board
 */
public int unique(Board unique)
{
	Board sym;
	int i, s = 0;

	unique = new Board(player, opponent);
	for (i = 1; i < 8; ++i) {
		sym = symetry(i);
		if (compare(sym, unique) < 0) {
			unique = sym;
			s = i;
		}
	}

	return s;
}

/**
 * @brief Count flippable discs of the last move.
 *
 * Count how many discs can be flipped (without flipping them).
 *
 * @param board  board to test
 * @param x      square on which to move.
 * @return       the number of disc(s) flipped.
 */
int count_last_flips(int x)
{
	return CountLastFlip.count_last_flip[x](player);
}

/**
 * @brief Compute a move.
 *
 * Compute how the board will be modified by a move without playing it.
 *
 * @param board board
 * @param x     square on which to move.
 * @param move  a Move structure remembering the modification.
 * @return      the flipped discs.
 */
ulong get_move(int x, ref Move move)
{
	move.flipped = flip[x](player, opponent);
	move.x = x;
	return move.flipped;
}

/**
 * @brief Check if a move is legal.
 *
 * @param board board
 * @param move  a Move.
 * @return      true if the move is legal, false otherwise.
 */
bool check_move(Move move)
{
	if (move.x == PASS) return !can_move(board.player, board.opponent);
	else if ((X_TO_BIT[move.x] & ~(board.player|board.opponent)) == 0) return false;
	else if (move.flipped != flip[move.x](board.player, board.opponent)) return false;
	else return true;
}

/**
 * @brief Update a board.
 *
 * Update a board by flipping its discs and updating every other data,
 * according to the 'move' description.
 *
 * @param board the board to modify
 * @param move  A Move structure describing the modification.
 */
void update(Move move)
{
	player ^= (move.flipped |  Bit.X_TO_BIT[move.x]);
	board.opponent ^= move.flipped;
	swap_players(board);

	check(board);
}

/**
 * @brief Restore a board.
 *
 * Restore a board by un-flipping its discs and restoring every other data,
 * according to the 'move' description, in order to cancel a update_move.
 *
 * @param board board to restore.
 * @param move  a Move structure describing the modification.
 */
void restore(Board board, Move move)
{
	swap_players(board);
	board.player ^= (move.flipped |  Bit.X_TO_BIT[move.x]);
	board.opponent ^= move.flipped;

	check(board);
}

/**
 * @brief Passing move
 *
 * Modify a board by passing player's turn.
 *
 * @param board board to update.
 */
void pass()
{
	swap_players(board);

	check(board);
}

/**
 * @brief Compute a board resulting of a move played on a previous board.
 *
 * @param board board to play the move on.
 * @param x move to play.
 * @param next resulting board.
 * @return flipped discs.
 */
ulong next(int x, Board next)
{
	const ulong flipped = flip[x](board.player, board.opponent);
	const ulong player = board.opponent ^ flipped;

	next.opponent = board.player ^ (flipped |  Bit.X_TO_BIT[x]);
	next.player = player;

	return flipped;
}

/**
 * @brief Compute a board resulting of an opponent move played on a previous board.
 *
 * Compute the board after passing and playing a move.
 *
 * @param board board to play the move on.
 * @param x opponent move to play.
 * @param next resulting board.
 * @return flipped discs.
 */
ulong pass_next(int x, Board next)
{
	const ulong flipped = flip[x](board.opponent, board.player);

	next.opponent = board.opponent ^ (flipped |  Bit.X_TO_BIT[x]);
	next.player = board.player ^ flipped;

	return flipped;
}

/**
 * @brief Get a part of the moves.
 *
 * Partially compute a bitboard where each coordinate with a legal move is set to one.
 *
 * Two variants of the algorithm are provided, one based on Kogge-Stone parallel
 * prefix.
 *
 * @param P bitboard with player's discs.
 * @param mask bitboard with flippable opponent's discs.
 * @param dir flipping direction.
 * @return some legal moves in a 64-bit unsigned integer.
 */
static ulong get_some_moves(ulong P, ulong mask, int dir)
{
#if KOGGE_STONE_1

	// kogge-stone algorithm
 	// 6 << + 6 >> + 12 & + 7 |
	// + better instruction independency
	ulong flip_l, flip_r;
	ulong mask_l, mask_r;
	int d;

	flip_l = flip_r = P;
	mask_l = mask_r = mask;
	d = dir;

	flip_l |= mask_l & (flip_l << d);   flip_r |= mask_r & (flip_r >> d);
	mask_l &= (mask_l << d);            mask_r &= (mask_r >> d);
	d <<= 1;
	flip_l |= mask_l & (flip_l << d);   flip_r |= mask_r & (flip_r >> d);
	mask_l &= (mask_l << d);            mask_r &= (mask_r >> d);
	d <<= 1;
	flip_l |= mask_l & (flip_l << d);   flip_r |= mask_r & (flip_r >> d);

	return ((flip_l & mask) << dir) | ((flip_r & mask) >> dir);

#else

 	// sequential algorithm
 	// 7 << + 7 >> + 6 & + 12 |
	ulong flip;

	flip = (((P << dir) | (P >> dir)) & mask);
	flip |= (((flip << dir) | (flip >> dir)) & mask);
	flip |= (((flip << dir) | (flip >> dir)) & mask);
	flip |= (((flip << dir) | (flip >> dir)) & mask);
	flip |= (((flip << dir) | (flip >> dir)) & mask);
	flip |= (((flip << dir) | (flip >> dir)) & mask);
	return (flip << dir) | (flip >> dir);

#endif
}

/**
 * @brief Get legal moves.
 *
 * Compute a bitboard where each coordinate with a legal move is set to one.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return all legal moves in a 64-bit unsigned integer.
 */
ulong get_moves(ulong P, ulong O)
{
	return (get_some_moves(P, O & 0x7E7E7E7E7E7E7E7Eul, 1) // horizontal
		| get_some_moves(P, O & 0x00FFFFFFFFFFFF00ul, 8)   // vertical
		| get_some_moves(P, O & 0x007E7E7E7E7E7E00ul, 7)   // diagonals
		| get_some_moves(P, O & 0x007E7E7E7E7E7E00ul, 9))
		& ~(P|O); // mask with empties
}

/**
 * @brief Check if a player can move.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return true or false.
 */
bool can_move(ulong P, ulong O)
{
	ulong E = ~(P|O); // empties

	return (get_some_moves(P, O & 0x007E7E7E7E7E7E00ul, 7) & E)  // diagonals
		|| (get_some_moves(P, O & 0x007E7E7E7E7E7E00ul, 9) & E)
	    || (get_some_moves(P, O & 0x7E7E7E7E7E7E7E7Eul, 1) & E)  // horizontal
		|| (get_some_moves(P, O & 0x00FFFFFFFFFFFF00ul, 8) & E); // vertical
}

/**
 * @brief Count legal moves.
 *
 * Compute mobility, ie the number of legal moves.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return a count of all legal moves.
 */
int get_mobility(ulong P, ulong O)
{
	return bit_count(get_moves(P, O));
}

int get_weighted_mobility(ulong P, ulong O)
{
	return bit_weighted_count(get_moves(P, O));
}

/**
 * @brief Get some potential moves.
 *
 * @param P bitboard with player's discs.
 * @param dir flipping direction.
 * @return some potential moves in a 64-bit unsigned integer.
 */
static ulong get_some_potential_moves(ulong P, int dir)
{
	return (P << dir | P >> dir);
}

/**
 * @brief Get potential moves.
 *
 * Get the list of empty squares in contact of a player square.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return all potential moves in a 64-bit unsigned integer.
 */
static ulong get_potential_moves(ulong P, ulong O)
{
	return (get_some_potential_moves(O & 0x7E7E7E7E7E7E7E7Eul, 1) // horizontal
		| get_some_potential_moves(O & 0x00FFFFFFFFFFFF00ul, 8)   // vertical
		| get_some_potential_moves(O & 0x007E7E7E7E7E7E00ul, 7)   // diagonals
		| get_some_potential_moves(O & 0x007E7E7E7E7E7E00ul, 9))
		& ~(P|O); // mask with empties
}

/**
 * @brief Get potential mobility.
 *
 * Count the list of empty squares in contact of a player square.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return a count of potential moves.
 */
int get_potential_mobility(ulong P, ulong O)
{
	return bit_weighted_count(get_potential_moves(P, O));
}

/**
 * @brief search stable edge patterns.
 *
 * Compute a 8-bit bitboard where each stable square is set to one
 *
 * @param old_P previous player edge discs.
 * @param old_O previous opponent edge discs.
 * @param stable 8-bit bitboard with stable edge squares.
 */
static int find_edge_stable(int old_P, int old_O, int stable)
{
	int P, O, x, y;
	int E = ~(old_P | old_O); // empties

	stable &= old_P; // mask stable squares with remaining player squares.
	if (!stable || E == 0) return stable;

	for (x = 0; x < 8; ++x) {
		if (E &  Bit.X_TO_BIT[x]) { //is x an empty square ?
			O = old_O;
			P = old_P |  Bit.X_TO_BIT[x]; // player plays on it
			if (x > 1) { // flip left discs
				for (y = x - 1; y > 0 && (O &  Bit.X_TO_BIT[y]); --y) ;
				if (P &  Bit.X_TO_BIT[y]) {
					for (y = x - 1; y > 0 && (O &  Bit.X_TO_BIT[y]); --y) {
						O ^=  Bit.X_TO_BIT[y]; P ^=  Bit.X_TO_BIT[y];
					}
				}
			}
			if (x < 6) { // flip right discs
				for (y = x + 1; y < 8 && (O &  Bit.X_TO_BIT[y]); ++y) ;
				if (P &  Bit.X_TO_BIT[y]) {
					for (y = x + 1; y < 8 && (O &  Bit.X_TO_BIT[y]); ++y) {
						O ^=  Bit.X_TO_BIT[y]; P ^=  Bit.X_TO_BIT[y];
					}
				}
			}
			stable = find_edge_stable(P, O, stable); // next move
			if (!stable) return stable;

			P = old_P;
			O = old_O |  Bit.X_TO_BIT[x]; // opponent plays on it
			if (x > 1) {
				for (y = x - 1; y > 0 && (P &  Bit.X_TO_BIT[y]); --y) ;
				if (O &  Bit.X_TO_BIT[y]) {
					for (y = x - 1; y > 0 && (P &  Bit.X_TO_BIT[y]); --y) {
						O ^=  Bit.X_TO_BIT[y]; P ^=  Bit.X_TO_BIT[y];
					}
				}
			}
			if (x < 6) {
				for (y = x + 1; y < 8 && (P &  Bit.X_TO_BIT[y]); ++y) ;
				if (O &  Bit.X_TO_BIT[y]) {
					for (y = x + 1; y < 8 && (P &  Bit.X_TO_BIT[y]); ++y) {
						O ^=  Bit.X_TO_BIT[y]; P ^=  Bit.X_TO_BIT[y];
					}
				}
			}
			stable = find_edge_stable(P, O, stable); // next move
			if (!stable) return stable;
		}
	}

	return stable;
}

/**
 * @brief Initialize the edge stability tables.
 */
void edge_stability_init()
{
	int P, O;

	for (P = 0; P < 256; ++P)
	for (O = 0; O < 256; ++O) {
		if (P & O) { // illegal positions
			edge_stability[P][O] = 0;
		} else {
			edge_stability[P][O] = find_edge_stable(P, O, P);
		}
	}
}

/**
 * @brief Get ful lines.
 *
 * @param line all discs on a line.
 * @param dir tested direction
 * @return a bitboard with ful lines along the tested direction.
 */
static ulong get_ful_lines(ulong line, int dir)
{
#if KOGGE_STONE_2

	// kogge-stone algorithm
 	// 5 << + 5 >> + 8 & + 10 |
	// + better instruction indepency
	ulong l, r, p, q;
	int d;

	l = r = line;
	p = q = line & 0xff818181818181fful;
	d = dir;

	l &= p | (l >> d);      r &= q | (r << d);
	p |= p >> d;            q |= q << d;
	d <<= 1;

	l &= p | (l >> d);      r &= q | (r << d);
	p |= p >> d;            q |= q << d;
	d <<= 1;

	l &= p | (l >> d);  r &= q | (r << d);

	return r & l;

#else

	// sequential algorithm
 	// 6 << + 6 >> + 12 & + 5 |
	ulong ful;
	ulong edge = line & 0xff818181818181fful;

	ful = (line & (((line >> dir) & (line << dir)) | edge));
	ful &= (((ful >> dir) & (ful << dir)) | edge);
	ful &= (((ful >> dir) & (ful << dir)) | edge);
	ful &= (((ful >> dir) & (ful << dir)) | edge);
	ful &= (((ful >> dir) & (ful << dir)) | edge);

	return ((ful >> dir) & (ful << dir));

#endif
}

/**
 * @brief Get stable edge.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return a bitboard with (some of) player's stable discs.
 *
 */
static ulong get_stable_edge(ulong P, ulong O)
{
	// compute the exact stable edges (from precomputed tables)
	return edge_stability[P & 0xff][O & 0xff]
	    |  ((ulong)edge_stability[P >> 56][O >> 56]) << 56
	    |  A1A8[edge_stability[((P & 0x0101010101010101ul) * 0x0102040810204080ul) >> 56][((O & 0x0101010101010101ul) * 0x0102040810204080ul) >> 56]]
	    |  H1H8[edge_stability[((P & 0x8080808080808080ul) * 0x0002040810204081ul) >> 56][((O & 0x8080808080808080ul) * 0x0002040810204081ul) >> 56]];
}

/**
 * @brief Estimate the stability.
 *
 * Count the number (in fact a lower estimate) of stable discs.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return the number of stable discs.
 */
int get_stability(ulong P, ulong O)
{
	const ulong disc = (P | O);
	const ulong central_mask = (P & 0x007e7e7e7e7e7e00ul);
	ulong stable_h, stable_v, stable_d7, stable_d9, stable, new_stable;

	// compute the exact stable edges (from precomputed tables)
	new_stable = get_stable_edge(P, O);

	// discs on ful lines are stable
	new_stable |= (get_ful_lines(disc, 1) &
	              get_ful_lines(disc, 8) &
                  get_ful_lines(disc, 7) &
                  get_ful_lines(disc, 9) & central_mask);

	// now compute the other stable discs (ie discs touching another stable disc in each flipping direction).
	stable = 0;
	while (new_stable & ~stable) {
		stable |= new_stable;
		stable_h = ((stable >> 1) | (stable << 1));
		stable_v = ((stable >> 8) | (stable << 8));
		stable_d7 = ((stable >> 7) | (stable << 7));
		stable_d9 = ((stable >> 9) | (stable << 9));
	    new_stable = (stable_h & stable_v & stable_d7 & stable_d9 & central_mask);
	}

	return bit_count(stable);
}

/**
 * @brief Estimate the stability of edges.
 *
 * Count the number (in fact a lower estimate) of stable discs on the edges.
 *
 * @param P bitboard with player's discs.
 * @param O bitboard with opponent's discs.
 * @return the number of stable discs on the edges.
 */
int get_edge_stability(ulong P, ulong O)
{
	return bit_count(get_stable_edge(P, O));
}

/**
 * @brief Estimate corner stability.
 *
 * Count the number of stable discs around the corner. Limiting the count
 * to the corner keep the function fast but still get this information,
 * particularly important at Othello. Corner stability will be used for
 * move sorting.
 *
 * @param P bitboard with player's discs.
 * @return the number of stable discs around the corner.
 */
int get_corner_stability(ulong P)
{
	const ulong stable = ((((0x0100000000000001ul & P) << 1) | ((0x8000000000000080ul & P) >> 1) | ((0x0000000000000081ul & P) << 8) | ((0x8100000000000000ul & P) >> 8) | 0x8100000000000081ul) & P);
	return bit_count(stable);
}

/**
 * @brief Compute a hash code.
 *
 * @param board the board.
 * @return the hash code of the bitboard
 */
ulong get_hash_code()
{
	ulong h;
	unsafe
	{
		fixed (ulong* pl = &player)
		{
			byte* p = (byte*)pl;

			h = hash_rank[0][p[0]];
			h ^= hash_rank[1][p[1]];
			h ^= hash_rank[2][p[2]];
			h ^= hash_rank[3][p[3]];
			h ^= hash_rank[4][p[4]];
			h ^= hash_rank[5][p[5]];
			h ^= hash_rank[6][p[6]];
			h ^= hash_rank[7][p[7]];
		}

		fixed (ulong* o = &opponent)
		{
			byte* p = (byte*)o;
			h ^= hash_rank[8][p[8]];
			h ^= hash_rank[9][p[9]];
			h ^= hash_rank[10][p[10]];
			h ^= hash_rank[11][p[11]];
			h ^= hash_rank[12][p[12]];
			h ^= hash_rank[13][p[13]];
			h ^= hash_rank[14][p[14]];
			h ^= hash_rank[15][p[15]];
		}
	}

	return h;
/*	ATTENTION: The following algorithm does not work!
    ulong h;

	h = board.player ^ (2857720171ul * board.opponent);
	h ^= h >> 29;
	h += h << 16;
	h ^= h >> 21;
	h += h << 32;

	return h;
*/
}

/**
 * @brief Compute a hash code.
 *
 * @param board the board.
 * @return the hash code of the bitboard
 */
ulong get_move_excluded_hash_code(int excluded_move)
{
	ulong h;
	unsafe
	{
		const byte* p = (byte*)&player;

		h = hash_rank[0][p[0]];
		h ^= hash_rank[1][p[1]];
		h ^= hash_rank[2][p[2]];
		h ^= hash_rank[3][p[3]];
		h ^= hash_rank[4][p[4]];
		h ^= hash_rank[5][p[5]];
		h ^= hash_rank[6][p[6]];
		h ^= hash_rank[7][p[7]];
		p = (byte*)&opponent;
		h ^= hash_rank[8][p[8]];
		h ^= hash_rank[9][p[9]];
		h ^= hash_rank[10][p[10]];
		h ^= hash_rank[11][p[11]];
		h ^= hash_rank[12][p[12]];
		h ^= hash_rank[13][p[13]];
		h ^= hash_rank[14][p[14]];
		h ^= hash_rank[15][p[15]];
		h ^= hash_excluded[excluded_move];
	}

	return h;
}

/**
 * @brief Get square color.
 *
 * returned value: 0 = player, 1 = opponent, 2 = empty;
 *
 * @param board board.
 * @param x square coordinate.
 * @return square color.
 */
int get_square_color(int x)
{
	return 2 - 2 * ((board.player >> x) & 1) - ((board.opponent >> x) & 1);
}

/**
 * @brief Check if a square is occupied.
 *
 * @param board board.
 * @param x square coordinate.
 * @return true if a square is occupied.
 */
bool is_occupied(int x)
{
	return (board.player | board.opponent) &  Bit.X_TO_BIT[x];
}

/**
 * @brief Check if current player should pass.
 *
 * @param board board.
 * @return true if player is passing, false otherwise.
 */
bool is_pass()
{
	return can_move(player, opponent) == false &&
		can_move(opponent, player) == true;
}

/**
 * @brief Check if the game is over.
 *
 * @param board board.
 * @return true if game is over, false otherwise.
 */
bool is_game_over()
{
	return can_move(board.player, board.opponent) == false &&
		can_move(board.opponent, board.player) == false;
}


/**
 * @brief Check if the game is over.
 *
 * @param board board.
 * @return true if game is over, false otherwise.
 */
int count_empties()
{
	return bit_count(~(board.player | board.opponent));
}

/**
 * @brief Print out the board.
 *
 * Print an ASCII representation of the board to an output stream.
 *
 * @param board board to print.
 * @param player player's color.
 * @param f output stream.
 */
void print(int player, TextWriter f)
{
	int i, j, square, x;
	char *color = "?*O-." + 1;
	ulong moves = get_moves(board.player, board.opponent);

	fputs("  A B C D E F G H\n", f);
	for (i = 0; i < 8; ++i) {
		fputc(i + '1', f);
		fputc(' ', f);
		for (j = 0; j < 8; ++j) {
			x = i * 8 + j;
			if (player == BLACK) square = 2 - ((board.opponent >> x) & 1) - 2 * ((board.player >> x) & 1);
			else square = 2 - ((board.player >> x) & 1) - 2 * ((board.opponent >> x) & 1);
			if (square == EMPTY && (moves &  Bit.X_TO_BIT[x])) ++square;
			fputc(color[square], f);
			fputc(' ', f);
		}
		fputc(i + '1', f);
		if (i == 1)
			fprintf(f, " %c to move", color[player]);
		else if (i == 3)
			fprintf(f, " %c: discs = %2d    moves = %2d",
				color[player], bit_count(board.player), get_mobility(board.player, board.opponent));
		else if (i == 4)
			fprintf(f, " %c: discs = %2d    moves = %2d",
				color[!player], bit_count(board.opponent), get_mobility(board.opponent, board.player));
		else if (i == 5)
			fprintf(f, "  empties = %2d      ply = %2d",
				64 - bit_count(board.opponent|board.player), bit_count(board.opponent|board.player) - 3);
		fputc('\n', f);
	}
	fputs("  A B C D E F G H\n", f);
}

/**
 * @brief convert the to a compact string.
 *
 * @param board board to convert.
 * @param player player's color.
 * @param s output string.
 */
public string ToString(Color player)
{
	int square, x;
	string color = "XO-?";
	var s = new StringBuilder();

	for (x = 0; x < 64; ++x) {
		if (player == Color.BLACK) square = 2 - ((opponent >> x) & 1) - 2 * ((player >> x) & 1);
		else square = 2 - ((board.player >> x) & 1) - 2 * ((opponent >> x) & 1);
		s.Append(color[square]);
	}
	s.Append(' ');
	s.Append(color[player]);
	return s.ToString();
}

	}
}
