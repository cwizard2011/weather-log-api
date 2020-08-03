using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace weatherlogapi.Models
{
    public class WeatherLogUpdateDTO
    {
        public string current_temperature { get; set; }

        public WeatherLogUpdateDTO()
        {
        }

    }
}