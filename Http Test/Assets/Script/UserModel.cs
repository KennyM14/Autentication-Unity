using System.Collections.Generic;
using System; 

[Serializable]
public class UserModel
{
    public string _id;
    public string username;
    public int score;
    public bool estado;
}

[System.Serializable]
public class UserList
{
    public List<UserModel> usuarios;
}
