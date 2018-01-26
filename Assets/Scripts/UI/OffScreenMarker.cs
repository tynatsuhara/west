using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OffScreenMarker : MonoBehaviour {

    public float markerDistFromEdge;

    public void Indicate(Vector3 viewPortPoint, Camera cam) {
        Vector3 p = viewPortPoint;
        Vector3 worldPoint = cam.ViewportToWorldPoint(ViewportIntersectPoint(p));
        Vector3 worldCenter = cam.ViewportToWorldPoint(new Vector3(.5f, .5f));
        // move it slightly towards the center
        worldPoint += (worldCenter - worldPoint).normalized * markerDistFromEdge * cam.orthographicSize;
        transform.position = worldPoint;

        Vector3 screenPoint = cam.ViewportToScreenPoint(p);
        Vector3 screenCenter = cam.ViewportToScreenPoint(new Vector2(.5f, .5f));
        float angle = Mathf.Atan2(screenPoint.y - screenCenter.y, screenPoint.x - screenCenter.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

	private Vector3 ViewportIntersectPoint(Vector3 outsidePoint) {
		Vector3 midpoint = new Vector2(.5f, .5f);
		List<Vector3?> pts = new List<Vector3?>();
		if (outsidePoint.y > .5f) {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.up, Vector2.one));
		} else {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.zero, Vector2.right));
		}
		if (outsidePoint.x < .5f) {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.zero, Vector2.up));
		} else {
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.right, Vector2.one));
		}
		return pts.Where(x => x.HasValue).OrderBy(x => (midpoint - x.Value).magnitude).First().Value;
	}

	// taken from https://gamedev.stackexchange.com/questions/111100/intersection-of-a-line-and-a-rectangle
	private Vector3? LineIntersectPoint(Vector3 ps1, Vector3 pe1, Vector3 ps2, Vector3 pe2) {
		// Get A,B of first line - points : ps1 to pe1
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;
		// Get A,B of second line - points : ps2 to pe2
		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;

		// Get delta and check if the lines are parallel
		float delta = A1*B2 - A2*B1;
		if(delta == 0) return null;

		// Get C of first and second lines
		float C2 = A2*ps2.x+B2*ps2.y;
		float C1 = A1*ps1.x+B1*ps1.y;
		//invert delta to make division cheaper
		float invdelta = 1/delta;
		// now return the Vector2 intersection point
		return new Vector3((B2*C1 - B1*C2)*invdelta, (A1*C2 - A2*C1)*invdelta);
	}
}