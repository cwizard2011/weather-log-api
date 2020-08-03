using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace weatherlogapi.Models
{
    public class WeatherPost
    {
        public string city { get; set; }

        public string current_temperature { get; set; }

        internal AppDb Db { get; set; }

        public WeatherPost() {

        }

         public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            // cmd.CommandText = @"call create_log(@city, @current_temperature)";
            cmd.CommandText = @"Insert Into data_log(city, current_temperature) VALUES(@city, @current_temperature)";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            // id = (int) cmd.LastInsertedId;
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@city",
                DbType = DbType.String,
                Value = city,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@current_temperature",
                DbType = DbType.String,
                Value = current_temperature,
            });
        }
    }
}