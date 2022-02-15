public interface IMazePlayer
{
    /// <summary>
    /// Move ball with swipe
    /// </summary>
    void MoveBall(ref float timer, float moveAmount);
    /// <summary>
    /// Lose ball by crossing each square
    /// </summary>
    void ConsumeBall(int b);
}