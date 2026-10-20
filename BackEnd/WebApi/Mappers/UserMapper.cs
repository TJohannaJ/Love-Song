using Backend.Domain.DTOs;
using Backend.Domain.Entities;

namespace Backend.WebApi.Mappers
{
    public static class UserMapper
    {
        public static ProfileDTO ToProfileDTO(this User user)
        {
            return new ProfileDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Age = user.Age,
                Gender = user.Gender,
                City = user.City,
                ShortIntro = user.ShortIntro,
                LongIntro = user.LongIntro,
                Picture = user.PictureUrl,
                CommonSongs = user.CommonSongs.Select(s => new CommonSongDTO
                {
                    SpotifyTrackId = s.SpotifyTrackId,
                    Title = s.Title,
                    Artist = s.Artist,
                    PreviewUrl = s.PreviewUrl
                }).ToList()
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
                Picture = user.PictureUrl,
                City = user.City,
                Gender = user.Gender,
                ShortIntro = user.ShortIntro,
                LongIntro = user.LongIntro
            };
        }
    }
    
}
