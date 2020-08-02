using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace weatherlogapi
{
    public class WeatherLog
    {
        public int id { get; set; }
        public string city { get; set; }

        public string current_temperature { get; set; }
        public int read_count { get; set; }
        public DateTime created_date { get; set; }

        internal AppDb Db { get; set; }

        public WeatherLog()
        {
        }

        internal WeatherLog(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            // cmd.CommandText = @"call create_log(@city, @current_temperature)";
            cmd.CommandText = @"Insert Into data_log(city, current_temperature) VALUES(@city, @current_temperature)";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            id = (int) cmd.LastInsertedId;
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `data_log` SET `read_count` = @read_count, `current_temperature` = @current_temperature WHERE `id` = @id;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `data_log` WHERE `id` < @id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

        private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
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
            cmd.Parameters.Add(new MySqlParameter {
              ParameterName = "@read_count",
              DbType = DbType.Int32,
              Value = read_count
            });
        }
    }
}