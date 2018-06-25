[System.Serializable]
public class Reputation {

    [System.Serializable]    
    public enum Rank {
        LOVE = 10,
        LIKE = 6,
        NO_OPINION = 2,
        DISLIKE = -2,
        HATE = -6,
        ENEMIES = -10
    }

    private float reputation = 0;

    public float CurrentRep() {
        return reputation;
    }

    public Reputation(float reputation = 0) {
        this.reputation = reputation;
    }

    public void AffectReputation(float impact) {
        reputation += impact;
    }
}