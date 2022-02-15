interface IGrowingPlayer
{
    /// <summary>
    /// Get same colored ball, then expand
    /// </summary>
    public void GetBall(int b);
    /// <summary>
    /// Get different ball, then shrink
    /// </summary>
    public void LoseBall(int b);
}