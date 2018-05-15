[System.Serializable]
public class ReputationWithGroup {

    public enum Rank {
        LOVE = 10,
        LIKE = 6,
        NO_OPINION = 2,
        DISLIKE = -2,
        HATE = -6,
        ENEMIES = -10
    }

    private float reputation = 0;

    public bool CurrentRep() {
        return Rank.NO_OPINION < Rank.HATE;
    }

    public ReputationWithGroup(float reputation = 0) {
        this.reputation = reputation;
    }

    public void AffectReputation(float impact) {
        reputation += impact;
    }
}