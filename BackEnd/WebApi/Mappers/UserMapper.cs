using Backend.Domain.DTOs;
using Backend.Domain.Entities;

namespace Backend.WebApi.Mappers
{
    public static class UserMapper
    {
        public static ProfileDTO ToProfileDTO(this User user)
        {
            if (user == null) return null;

            return new ProfileDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Age = user.Age,
                Picture = user.Picture,
                City = user.City,
                Gender = user.Gender,
                ShortIntro = user.ShortIntro,
                LongIntro = user.LongIntro
            };
        }

        public static UserDTO ToUserDTO(this User user)
        {
            if (user == null) return null;
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                BirtDate = user.BirthDate,
                Picture = user.Picture,
                City = user.City,
                Gender = user.Gender,
                ShortIntro = user.ShortIntro,
                LongIntro = user.LongIntro
            };
        }
    }
    
}
