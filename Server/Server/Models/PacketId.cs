namespace Server.Models
{
    /// <summary>
    /// Przechowuje enuma identyfikatorow pakietow.
    /// </summary>
    public partial class Client
    {
        private enum PacketId
        {
            MSG,
            REGISTER,
            REGISTER_REFUSED,
            LOGIN,
            LOGIN_REFUSED,
            ADD_AD,
            ADDED_AD,
            BROWSE_ADS,
            SHOW_AD,
            EDIT_AD,
            ACCEPT_EDIT_AD,
            EDITED_AD,
            DELETE_AD
        }
    }
}
