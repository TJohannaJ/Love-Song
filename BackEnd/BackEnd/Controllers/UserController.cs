using Microsoft.AspNetCore.Mvc;
namespace BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static byte[] ImageToBinary(string imagePath)
        {
            FileStream fS = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[fS.Length];
            fS.Read(b, 0, (int)fS.Length);
            fS.Close();
            return b;
        }

        private static readonly string[] SurName =
        [
            "Kiss", "Szabó", "Nagy", "Tóth", "Varga", "Kovács", "Horváth", "Molnár"
        ];

        private static readonly string[] FirstName =
        [
            "László", "István", "János", "József", "Zoltán", "Mária", "Erzsébet", "Katalin", "Ilona", "Éva"
        ];

        private static readonly string[] ImagePath =
        [
            "C:/Users/Tibor/Documents/Önlab/onlab_project/daisy.png",
            "C:/Users/Tibor/Documents/Önlab/onlab_project/luigi.png",
            "C:/Users/Tibor/Documents/Önlab/onlab_project/mario.png",
            "C:/Users/Tibor/Documents/Önlab/onlab_project/peach.png"
         ];

        [HttpGet(Name = "GetProfiles")]
        public IEnumerable<User> Get()
        {
            return Enumerable.Range(1, 12).Select(index => new User
            {
                Id = Random.Shared.Next(0, 999999),
                Name = SurName[Random.Shared.Next(SurName.Length)] + " " + FirstName[Random.Shared.Next(FirstName.Length)],
                Age = Random.Shared.Next(18, 50),
                Picture = ImageToBinary(ImagePath[Random.Shared.Next(ImagePath.Length)]),
                ShortIntro = "Szia, nekem fontos, hogy a párom szeresse a zenét, ezért itt keresek partnert. :D",
                LongIntro = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            })
            .ToArray();
        }
    }
}
