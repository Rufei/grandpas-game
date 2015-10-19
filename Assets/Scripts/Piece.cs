using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

	private ChessGame chessGame;

	void Start () {
		chessGame = FindObjectOfType(typeof(ChessGame)) as ChessGame;
	}

	public void InvokeDeath() {
		GetComponent<MeshRenderer>().enabled = false;
	}

	public HashSet<string> GetPossibleMoves(string pos) {
		HashSet<string> moves = new HashSet<string>();

		int row = Piece.GetRow(pos);
		int col = Piece.GetCol(pos);

		if (IsPawn()) {
			string normalMove = GetPos(row, col, 1, 0);
			bool isBlocked = AddValidAndReturnShouldBreak(moves, pos, normalMove);
			if (!isBlocked && row == (IsWhite() ? 2 : 7)) {
				string twoSquareFirstMove = GetPos(row, col, 2, 0);
				AddValidAndReturnShouldBreak(moves, pos, twoSquareFirstMove);
			}
			// En Passant
			for (int i = -1; i <= 1; i += 2) {
				string enPassantMove = GetPos(row, col, 1, i);
				if (enPassantMove != null && chessGame.IsEnPassantValidFor(enPassantMove)) {
					AddValidAndReturnShouldBreak(moves, pos, enPassantMove);
				}
			}
		} else if (IsRook()) {
			AddRookMoves(moves, pos);
		} else if (IsBishop()) {
			AddBishopMoves(moves, pos);
		} else if (IsKnight()) {
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 2, 1));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 1, 2));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -2, 1));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -1, 2));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 2, -1));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 1, -2));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -2, -1));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -1, -2));
		} else if (IsQueen()) {
			AddRookMoves(moves, pos);
			AddBishopMoves(moves, pos);
		} else if (IsKing()) {
			for (int drow = -1; drow < 1; drow++) {
				AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, drow, 1));
				AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, drow, -1));
			}
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -1, 0));
			AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 1, 0));
		}

		return moves;
	}

	public bool IsPawn() {
		return gameObject.name.Contains("Pawn");
	}

	public bool IsRook() {
		return gameObject.name.Contains("Rook");
	}

	public bool IsBishop() {
		return gameObject.name.Contains("Bishop");
	}

	public bool IsKnight() {
		return gameObject.name.Contains("Knight");
	}

	public bool IsQueen() {
		return gameObject.name.Contains("Queen");
	}

	public bool IsKing() {
		return gameObject.name.Contains("King");
	}

	public bool IsWhite() {
		return gameObject.name[0] == 'W';
	}

	private string GetPos(int curRow, int curCol, int deltaRow, int deltaCol) {
		if (!IsWhite()) {
			deltaRow *= -1;
		}
		int row = curRow + deltaRow;
		int col = curCol + deltaCol;

		if (row < 1 || row > 8 || col < 1 || col > 8) {
			// Out of bounds
			return null;
		}
		return "" + Piece.IntToCol(col) + row;
	}

	private void AddRookMoves(HashSet<string> moves, string pos) {
		int row = Piece.GetRow(pos);
		int col = Piece.GetCol(pos);
		string newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, i, 0))) {
				break;
			}
		}
		newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -i, 0))) {
				break;
			}
		}
		newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 0, i))) {
				break;
			}
		}
		newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, 0, -i))) {
				break;
			}
		}
	}

	private void AddBishopMoves(HashSet<string> moves, string pos) {
		int row = Piece.GetRow(pos);
		int col = Piece.GetCol(pos);
		string newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, i, i))) {
				break;
			}
		}
		newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -i, i))) {
				break;
			}
		}
		newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, i, -i))) {
				break;
			}
		}
		newPos = "";
		for (int i = 1; newPos != null; i++) {
			if (AddValidAndReturnShouldBreak(moves, pos, GetPos(row, col, -i, -i))) {
				break;
			}
		}
	}

	private bool AddValidAndReturnShouldBreak(HashSet<string> moves, string loc, string dest) {
		if (dest == null) {
			// Invalid move
			return true;
		}

		Piece posPiece = chessGame.GetPiece(Piece.GetRow(dest), Piece.GetCol(dest));
		if (posPiece == null) {
			// Empty space
			moves.Add(loc + dest);
			return false;
		}

		if (posPiece.IsWhite() != this.IsWhite()) {
			// Can take enemy
			moves.Add(loc + "x" + dest);
		}
		// Can't move beyond
		return true;
	}

	public static int ColToInt(string col) {
		int num = 0;
		if (col == "a") {
			num = 1;
		} else if (col == "b") {
			num = 2;
		} else if (col == "c") {
			num = 3;
		} else if (col == "d") {
			num = 4;
		} else if (col == "e") {
			num = 5;
		} else if (col == "f") {
			num = 6;
		} else if (col == "g") {
			num = 7;
		} else if (col == "h") {
			num = 8;
		}
		return num;
	}

	public static string IntToCol(int col) {
		switch (col) {
			case 1:
				return "a";
			case 2:
				return "b";
			case 3:
				return "c";
			case 4:
				return "d";
			case 5:
				return "e";
			case 6:
				return "f";
			case 7:
				return "g";
			case 8:
				return "h";
			default:
				return null;
		}
	}

	public static string GetPos(int row, int col) {
		return "" + Piece.IntToCol(col).ToString() + row;
	}

	public static int GetRow(string pos) {
		return (int)Char.GetNumericValue(pos[1]);
	}

	public static int GetCol(string pos) {
		return ColToInt(pos.Substring(0, 1));
	}

	public static string GetSource(string move) {
		int mod = Char.IsUpper(move[0]) ? 1 : 0; // Adjust for piece declaration
		return move.Substring(mod, 2);
	}

	public static string GetDest(string move) {
		int mod = Char.IsUpper(move[0]) ? 1 : 0; // Adjust for piece declaration
		mod += 2; // Source
		if (move.Length > 3 && move[mod] == 'x') {
			// Capture notation
			mod += 1;
		}
		return move.Substring(mod, 2);
	}
}
