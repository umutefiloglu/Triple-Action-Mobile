public interface IResizingPlayer
{
    /// <summary>
    /// If size is inappropriate, then lose ball
    /// </summary>
    void HitObstacle(string tag, int loseAmount);
    /// <summary>
    /// Resize height with swipe; then change horizontal size according to height
    /// </summary>
    void Resize(float f);
}