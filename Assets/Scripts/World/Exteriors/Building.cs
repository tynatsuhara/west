[System.Serializable]
public class Building {
    public int prefabIndex = 0;
    public int bottomLeftTile;
    private int width_ = 4;
    private int height_ = 3;
    private int doorOffsetX_ = 1;
    private int doorOffsetY_ = -1;

    public int angle {
        get { return rotation * 90; }
    }
    public int width {
        get {
            if (rotation % 2 == 1) {
                return height_;
            }
            return width_;
        }
    }
    public int height {
        get {
            if (rotation % 2 == 1) {
                return width_;
            }
            return height_;
        }
    }
    public int doorOffsetX {  // where to build the road to
        get {
            if (rotation == 0) {
                return doorOffsetX_;
            } else if (rotation == 1) {
                return doorOffsetY_;
            } else if (rotation == 2) {
                return -doorOffsetX_ + width_ - 1;
            } else {
                return -doorOffsetY_ + height_ - 1;
            }
        }
    }
    public int doorOffsetY {
        get {
            if (rotation == 0) {
                return doorOffsetY_;
            } else if (rotation == 1) {
                return width - doorOffsetX_ - 1;
            } else if (rotation == 2) {
                return height - doorOffsetY_ - 1;
            } else {
                return doorOffsetX_;
            }
        }
    }

    private int rotation;
    // rotates 90 degrees clockwise
    public void Rotate(int times = 1) {
        for (int i = 0; i < times; i++) {
            rotation = (rotation + 1) % 4;
        }
    }
}