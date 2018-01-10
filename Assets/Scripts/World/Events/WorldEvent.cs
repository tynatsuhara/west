public interface WorldEvent {
    // hasLoadedLocation is true if we are executing the event when the player
    // is currently in a location. If they are not in a location (ie traveling
    // between towns), it will be false
    void Execute(bool hasLoadedLocation);
}