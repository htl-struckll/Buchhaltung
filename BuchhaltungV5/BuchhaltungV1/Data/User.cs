namespace BuchhaltungV1
{
    public class User
    {
        #region var´s
        public int  Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        #endregion

        public User(int id, string name, bool isAdmin)
        {
            Id = id;
            Name = name;
            IsAdmin = isAdmin;
        }
    }
}
