using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountLogic : MonoBehaviour
{


    public User GetUser()
    {
        User user = new User();
        return user;
    }
}
public struct User
{
    public string Username;
    public string Password;
    public bool IsPremium;
    public User(string username, string password, bool isprem)
    {
        Username = username;
        Password = password;
        IsPremium = isprem;
    }
}
