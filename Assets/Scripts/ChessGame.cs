using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ChessGame : MonoBehaviour {

	public bool playerCanInteract = true;
	public bool playerCanAct = false;
	public int millisOfThought = 5000;

	private string aiPath = "C:\\demo\\stockfish-6-64.exe";
	
	private System.Diagnostics.Process aiProcess;
	private Thread aiListenerThread;
	private int selectedRow = 0; // 1-headed
	private int selectedCol = 0; // 1-headed
	private HashSet<Cell> highlightedCells = new HashSet<Cell>();
	private HashSet<string> highlightedMoves = new HashSet<string>();
	private string previousMove;
	private string history = "";
	private string ponder = "";
	private string curAiMove;
	public GameObject textbox;
	public TalkScript textScript;

	// Use this for initialization
	void Start () {

		//textbox = GameObject.Find("Grandpa Text");
		//textbox = Camera.main.transform.GetChild(0).GetComponent<TalkScript>().scriptNumber
		textScript = Camera.main.transform.Find ("Grandpa Text").GetComponent<TalkScript>();
		aiProcess = new System.Diagnostics.Process();
		aiProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		aiProcess.StartInfo.CreateNoWindow = true;
		aiProcess.StartInfo.UseShellExecute = false;
		aiProcess.StartInfo.RedirectStandardInput = true;
		aiProcess.StartInfo.RedirectStandardOutput = true;
		aiProcess.StartInfo.FileName = aiPath;
		aiProcess.StartInfo.Arguments = "";

		aiProcess.Start();
		aiListenerThread = new Thread(new ThreadStart(getAIResponse));
		aiListenerThread.Start();
		ToAI("");
		ToAI("uci");
		ToAI("ucinewgame");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			CheckSelect();
		}
		if (curAiMove != null) {
			DoMove(curAiMove);
			playerCanAct = true;
			previousMove = curAiMove;
			curAiMove = null;
		}
	}

	private void startAi() {
		aiProcess.Start();
	}

	private void getAIResponse() {
		while (!aiProcess.StandardOutput.EndOfStream) {
			string output = aiProcess.StandardOutput.ReadLine();
			if (output != null) {
				//Debug.Log("AI=" + output);
				if (output.StartsWith("bestmove")) {
					Debug.Log("AI=" + output);
					string[] words = output.Split(' ');
					if (words[0] == "bestmove" && words[2] == "ponder") {
						curAiMove = words[1];
						ponder = words[3];
					}
				}
			}
		}
	}

	private void DoMove(string move) {
		string src = Piece.GetSource(move);
		string dest = Piece.GetDest(move);
		Cell srcCell = GetCell(Piece.GetRow(src), Piece.GetCol(src));
		Cell destCell = GetCell(Piece.GetRow(dest), Piece.GetCol(dest));
		Piece piece = GetPiece(Piece.GetRow(src), Piece.GetCol(src));

		srcCell.curPiece = null;
		if (destCell.curPiece != null) {
			destCell.curPiece.InvokeDeath();
		}
		destCell.curPiece = piece;

		piece.transform.position = new Vector3(destCell.transform.position.x, piece.transform.position.y, destCell.transform.position.z);

		history += " " + move;

		Debug.Log("MOVING " + piece + " TO=" + destCell);
	}

	void OnDestroy () {
		if (aiProcess != null) {
			aiProcess.Kill();
		}
		aiListenerThread.Abort();
	}

	private void CheckSelect() {
		RaycastHit hit;
		if (IsClickOnCell(out hit)) {
			// Rows and columns are 1-headed
			Transform cellTransform = hit.collider.gameObject.transform;

			int row = (int)((cellTransform.position.z / cellTransform.localScale.z) + 2);
			int col = (int)((cellTransform.position.x / cellTransform.localScale.x) + 1);
			if (playerCanInteract) {

				UnityEngine.Debug.Log("Target row=" + row + ";col=" + col);
				Cell targetCell = GetCell(row, col);


				UnityEngine.Debug.Log("TargetCell=" + targetCell);
				// Debugging
				string logOut = "";
				foreach (Cell c in highlightedCells) {
					logOut += c + ";";
				}
				Debug.Log("highlightedCells=" + logOut);

				if (targetCell != null && highlightedCells.Contains(targetCell)) {
					Cell selectedCell = GetCell(selectedRow, selectedCol);
					if (targetCell != selectedCell) {
						Move(row, col);
					}
				} else {
					Select(row, col);
				}
			}
		} else {
			Unselect();
		}
	}

	private bool IsClickOnCell(out RaycastHit hit) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		return Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8) && hit.collider.gameObject.tag == "Cell";
	}

	private void Move(int row, int col) {
		if (playerCanAct) {

			Cell moveCell = GetCell(row, col);
			string move = moveCell.GetAssociatedMove();
			DoMove(move);

			//AFHALIEGHTALIEGHLJAHFLJDKAHGLAKDGHTL

			//textbox.GetComponent<TalkScript>
			//textbox.
			//TalkScript otherScript = GetComponent<TalkScript>();
			//otherScript.DoSomething();
			//textScript = GetComponent<TalkScript>();

			textScript.incrementText();

			if (ponder == move) {
				ToAI("ponderhit");
			}
			ToAI("position startpos moves" + history);

			playerCanAct = false;

			ToAI("go movetime " + millisOfThought);
			Unselect();
		}
	}

	private void Select(int row, int col) {
		Unselect();

		UnityEngine.Debug.Log("Selecting row=" + row + ";col=" + col);
		selectedRow = row;
		selectedCol = col;

		Cell selectedCell = GetCell(row, col);
		selectedCell.SetSelected(true);
		highlightedCells.Add(selectedCell);

		Piece cellPiece = GetPiece(row, col);
		if (cellPiece != null) {
			highlightedMoves = cellPiece.GetPossibleMoves(Piece.GetPos(row, col));

			foreach (string move in highlightedMoves) {
				string dest = Piece.GetDest(move);
				Cell destCell = GetCell(Piece.GetRow(dest), Piece.GetCol(dest));
				destCell.SetHighlighted(true, move);
				highlightedCells.Add(destCell);
			}

			// Debugging
			//string logOut = "";
			//foreach (string str in highlightedMoves) {
			//	logOut += str + ";";
			//}
			//Debug.Log("MOVES=" + logOut);


		}

	}

	private void Unselect() {
		selectedRow = 0;
		selectedCol = 0;
		highlightedMoves.Clear();
		foreach (Cell cell in highlightedCells) {
			cell.SetSelected(false);
			cell.SetHighlighted(false, null);
		}
		highlightedCells.Clear();
	}

	public Piece GetPiece(int row, int col) {
		return GetCell(row, col).curPiece;
	}

	public bool IsEnPassantValidFor(string dest) {
		if (previousMove == null) {
			// No moves yet
			return false;
		}

		if (!Char.IsLower(previousMove[0])) {
			// Not a pawn move
			return false;
		}

		string prev = Piece.GetDest(previousMove);
		if (Piece.GetCol(prev) != Piece.GetCol(dest)) {
			// Needs to jump to same column
			return false;
		}

		if (Piece.GetRow(prev) + 1 != Piece.GetRow(dest)) {
			// Needs to jump one "above" the black piece
		}

		Piece pieceAtLoc = GetPiece(Piece.GetRow(dest), Piece.GetCol(dest));
		if (pieceAtLoc == null) {
			// No piece exists
			return false;
		}

		if (!pieceAtLoc.IsPawn()) {
			// Piece is not a pawn
			return false;
		}

		if (pieceAtLoc.IsWhite()) {
			// Piece is player's pawn
			return false;
		}

		return true;
	}

	Cell GetCell(int row, int col) {
		return GetCell(row, Piece.IntToCol(col));
	}

	Cell GetCell(int row, string col) {
		return transform.Find("Board/" + col + row).GetComponent<Cell>();
	}

	private void ToAI(string line) {
		Debug.Log("TO AI=" + line);
		aiProcess.StandardInput.WriteLine(line);
	}
}
