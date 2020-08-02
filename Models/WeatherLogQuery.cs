using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace weatherlogapi
{
    public class WeatherLogQuery
    {
        public AppDb Db { get; }

        public WeatherLogQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<WeatherLog> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `id`, `city`, `current_temperature`, `read_count`, `created_date` FROM `data_log` WHERE `id` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<WeatherLog> FindExistingNameAsync(string city_name)
        {
          using var cmd = Db.Connection.CreateCommand();
          cmd.CommandText = @"SELECT `id`, `read_count` From `data_log` WHERE Lower(city) = Lower(@city_name) AND read_count < 10";
          cmd.Parameters.Add(new MySqlParameter {
            ParameterName = "@city_name",
            DbType = DbType.Int32,
            Value = city_name
          });
           Console.WriteLine(city_name);
            Console.WriteLine("I am here");
          var result = await ReadExisting(await cmd.ExecuteReaderAsync());
          return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<WeatherLog>> LatestLogsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `id`, `city`, `current_temperature`, `read_count`, `created_date` FROM `data_log` ORDER BY `id` DESC LIMIT 10;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task DeleteAllAsync(int id)
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `data_log` where id < @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }

        private async Task<List<WeatherLog>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<WeatherLog>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new WeatherLog(Db)
                    {
                        id = reader.GetInt32(0),
                        city = reader.GetString(1),
                        current_temperature = reader.GetString(2),
                        read_count = reader.GetInt32(3),
                        created_date = reader.GetDateTime(4)
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

        private async Task<List<WeatherLog>> ReadExisting(DbDataReader reader)
        {
          var posts = new List<WeatherLog>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new WeatherLog(Db)
                    {
                        id = reader.GetInt32(0),
                        read_count = reader.GetInt32(1),
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }
    }
}