using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Building {
    // TODO: these TDs should store relative positions to the center of the building?
    public List<Teleporter.TeleporterData> doors = new List<Teleporter.TeleporterData>();

    // predefined for each building type (later set with builder class)
    public int prefabIndex = 0;
    public int width = 4;
    public int height = 3;

    // set in TownLocation.cs
    public int bottomLeftTile;

    public Building() {
        doors.Add(new Teleporter.TeleporterData(System.Guid.Empty, new Vector3(), "interior teleporter"));
    }

    // private int doorOffsetX_ = 1;
    // private int doorOffsetY_ = -1;

    // public int angle {
    //     get { return rotation * 90; }
    // }
    // public int width {
    //     get {
    //         if (rotation % 2 == 1) {
    //             return height_;
    //         }
    //         return width_;
    //     }
    // }
    // public int height {
    //     get {
    //         if (rotation % 2 == 1) {
    //             return width_;
    //         }
    //         return height_;
    //     }
    // }
    // public int doorOffsetX {  // where to build the road to
    //     get {
    //         if (rotation == 0) {
    //             return doorOffsetX_;
    //         } else if (rotation == 1) {
    //             return doorOffsetY_;
    //         } else if (rotation == 2) {
    //             return -doorOffsetX_ + width_ - 1;
    //         } else {
    //             return -doorOffsetY_ + height_ - 1;
    //         }
    //     }
    // }
    // public int doorOffsetY {
    //     get {
    //         if (rotation == 0) {
    //             return doorOffsetY_;
    //         } else if (rotation == 1) {
    //             return width_ - doorOffsetX_ - 1;
    //         } else if (rotation == 2) {
    //             return height_ - doorOffsetY_ - 1;
    //         } else {
    //             return doorOffsetX_;
    //         }
    //     }
    // }

    // rotates 90 degrees clockwise
    public void Rotate(int times = 1) {
        for (int i = 0; i < times; i++) {
            // rotation = (rotation + 1) % 4;
            int temp = width;
            width = height;
            height = temp;

            foreach (Teleporter.TeleporterData td in doors) {
                // rotate 90 degrees
                td.position = new SerializableVector3(Quaternion.AngleAxis(90, Vector3.up) * td.position.val);
                Debug.Log(td.position.val);
            }
        }
    }
}