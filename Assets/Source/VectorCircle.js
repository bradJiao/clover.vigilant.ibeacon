#pragma strict

import Vectrosity;

var dotSize = 2.0;
var numberOfDots = 100;
var numberOfRings = 1;
var dotColor = Color.cyan;
var radius = 2;

function Start () {
	var totalDots = numberOfDots * numberOfRings;
	var dotPoints = new Vector3[totalDots];
	var dotColors = new Color[totalDots];
	
	var reduceAmount = 1.0 - .75/totalDots;
	for (c in dotColors) {
		c = dotColor;
		//dotColor *= reduceAmount;
	}
	
	var dots = new VectorPoints("Dots", dotPoints, dotColors, null, dotSize);
	//var x = this.transform.position.x;
	//var y = this.transform.position.y;
	for (var i = 0; i < numberOfRings; i++) {
		dots.MakeCircle (this.transform.position, radius/(i+1), numberOfDots, numberOfDots*i);	
	}
	dots.Draw();
}