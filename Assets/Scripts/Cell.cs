using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

	public Piece curPiece;
	public bool isSelected = false;
	public bool isHighlighted = false;
	private string associatedMove;

	public void SetSelected(bool isSelected) {
		this.isSelected = isSelected;
		//GetComponent<MeshRenderer>().enabled = !isSelected;
	}

	public void SetHighlighted(bool isHighlighted, string move) {
		this.isHighlighted = isHighlighted;
		this.associatedMove = move;
		//GetComponent<MeshRenderer>().enabled = !isHighlighted;
	}

	public string GetAssociatedMove() {
		return this.associatedMove;
	}

}
