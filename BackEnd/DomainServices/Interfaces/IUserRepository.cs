using Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface IUserRepository
    {
        User GetUserById(string userId);        
    }
}
