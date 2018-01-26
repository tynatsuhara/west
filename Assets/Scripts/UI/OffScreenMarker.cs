using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OffScreenMarker : MonoBehaviour {

    public float markerDistFromEdge;

    public void Indicate(Vector3 viewPortPoint, Camera cam) {
        Vector3 p = viewPortPoint;
        Vector3 intersect = ViewportIntersectPoint(p, cam);
        // move it slightly towards the center
        transform.position = cam.ViewportToWorldPoint(intersect);

        Vector3 screenPoint = cam.ViewportToScreenPoint(p);
        Vector3 screenCenter = cam.ViewportToScreenPoint(new Vector2(.5f, .5f));
        float angle = Mathf.Atan2(screenPoint.y - screenCenter.y, screenPoint.x - screenCenter.x) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

	private Vector3 ViewportIntersectPoint(Vector3 outsidePoint, Camera cam) {
		Vector3 midpoint = new Vector2(.5f, .5f);
        Vector2 tbShift = new Vector3(0, markerDistFromEdge * cam.aspect);
        Vector2 lrShift = new Vector3(markerDistFromEdge, 0);
		List<Vector3?> pts = new List<Vector3?>();
		if (outsidePoint.y > .5f) {  // top
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.up - tbShift, Vector2.one - tbShift));
		} else {  // bottom
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.zero + tbShift, Vector2.right + tbShift));
		}
		if (outsidePoint.x < .5f) {  // left
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.zero + lrShift, Vector2.up + lrShift));
		} else {  // right
			pts.Add(LineIntersectPoint(midpoint, outsidePoint, Vector2.right - lrShift, Vector2.one - lrShift));
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