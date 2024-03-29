﻿using UnityEngine;
using System.Collections;

/*
 * TODO: REDO MENU SYSTEM COMPLETELY
 *  - have one listener function on each menu node that can operate as it wants from input, if it's selected
 *  - different types of MenuNodes for different purposes
 *  - interact with mouse as well as console-esque input?
 */

public class Menu : MonoBehaviour {

	public Transform cursor;
	public bool perPerson {
		get { return playerId != 0; }
	}
	public int playerId;

	public MenuNode selectedNode;

	public void Update() {
		GetInput();
		if (selectedNode != null)
			selectedNode.Select();
		SaveDPad();
	}

	private void GetInput() {
		if (selectedNode == null)
			return;

		selectedNode.Deselect();

		if (GetUp() && selectedNode.up != null) {
			selectedNode = selectedNode.up;
		} else if (GetDown() && selectedNode.down != null) {
			selectedNode = selectedNode.down;
		} else if (GetLeft()) {
			if (selectedNode.carousel) {
				Carousel(selectedNode, -1);
			} else if (selectedNode.left != null) {
				selectedNode = selectedNode.left;
			}
		} else if (GetRight()) {
			if (selectedNode.carousel) {
				Carousel(selectedNode, 1);
			} else if (selectedNode.right != null) {
				selectedNode = selectedNode.right;
			}
		}
		selectedNode.Select();
		if (GetEnter()) {
			Enter(selectedNode);
		}
		if (GetEsc()) {
			Back(selectedNode);
		}
		if (Input.GetKeyDown(KeyCode.X)) {
			X(selectedNode);
		}
	}

	protected void NewSelect(MenuNode node) {
		if (selectedNode != null) {
			selectedNode.Deselect();
		}
		node.Select();
		selectedNode = node;
	}

	private Vector2 lastDPad;
	private void SaveDPad() {
		if (perPerson) {
			lastDPad = new Vector2(Input.GetAxis("DPX" + playerId), Input.GetAxis("DPY" + playerId));
		} else {
			lastDPad = new Vector2(Input.GetAxis("DPX"), Input.GetAxis("DPY"));
		}
	}
	private bool GetUp() {
		return ((Input.GetKeyDown(KeyCode.W) && (!perPerson || playerId == 1)) || 
				(lastDPad.y == 0 && perPerson && Input.GetAxis("DPY" + playerId) > 0) || 
				(lastDPad.y == 0 && !perPerson && Input.GetAxis("DPY") > 0));
	}
	private bool GetDown() {
		return ((Input.GetKeyDown(KeyCode.S) && (!perPerson || playerId == 1)) || 
				(lastDPad.y == 0 && perPerson && Input.GetAxis("DPY" + playerId) < 0) || 
				(lastDPad.y == 0 && !perPerson && Input.GetAxis("DPY") < 0));
	}
	private bool GetLeft() {
		return ((Input.GetKeyDown(KeyCode.A) && (!perPerson || playerId == 1)) || 
				(lastDPad.x == 0 && perPerson && Input.GetAxis("DPX" + playerId) < 0) || 
				(lastDPad.x == 0 && !perPerson && Input.GetAxis("DPX") < 0));
	}
	private bool GetRight() {
		return ((Input.GetKeyDown(KeyCode.D) && (!perPerson || playerId == 1)) || 
				(lastDPad.x == 0 && perPerson && Input.GetAxis("DPX" + playerId) > 0) || 
				(lastDPad.x == 0 && !perPerson && Input.GetAxis("DPX") > 0));
	}
	private bool GetEnter() {
		return (Input.GetKeyDown(KeyCode.Return) && (!perPerson || playerId == 1) || 
			   (!perPerson && Input.GetKeyDown("joystick button 1")) || 
			   (perPerson && Input.GetKeyDown("joystick " + playerId + " button 1")));
	}
	private bool GetEsc() {
		return (Input.GetKeyDown(KeyCode.Escape) && (!perPerson || playerId == 1) || 
			   (!perPerson && Input.GetKeyDown("joystick button 2")) || 
			   (perPerson && Input.GetKeyDown("joystick " + playerId + " button 2")));
	}

	public virtual void Carousel(MenuNode node, int dir) {}
	public virtual void Enter(MenuNode node) {}
	public virtual void Back(MenuNode node) {}
	public virtual void X(MenuNode node) {}
}
