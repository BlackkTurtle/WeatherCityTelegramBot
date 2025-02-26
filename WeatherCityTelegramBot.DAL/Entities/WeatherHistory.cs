using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCityTelegramBot.DAL.Entities
{
    public class WeatherHistory
    {
        public int Id { get; set; }
        public string City { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserId { get; set; }
    }
}
