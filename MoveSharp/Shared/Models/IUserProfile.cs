namespace MoveSharp.Models
{
    public interface IUserProfile
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Whether the user is authenticated
        /// </summary>
        bool IsAuthenticated { get; }
        /// <summary>
        /// URL of the user's profile picture
        /// null when no picture is available
        /// </summary>
        string ProfileImageUrl { get; }
    }
}
