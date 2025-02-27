using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCityTelegramBot.DAL.DTOs.WeatherDTOs
{
    public class WeatherDTO
    {
        public double Latitude {  get; set; }
        public double Longitude { get; set; }
        public string ReasolvedAddress { get; set; }
        public string Address { get; set; }
        public string Timezone { get; set; }
        public double Tzoffset { get; set; }
        public string Description { get; set; }
    }
}
