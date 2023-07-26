namespace Src.NetworkedMovement
{
    public interface IMobRelevanceProvider
    {
        /// <summary>
        /// Now it's dist. to closest player (is probably updated by some rate)
        /// </summary>
        float ClosestObserverDistance { get; }
        
        float Dbg_ClosestObserverDistance_Forced_DANGER { get; }
    }
}