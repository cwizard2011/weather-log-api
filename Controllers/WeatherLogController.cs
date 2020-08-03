using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weatherlogapi.Models;
using System.Collections.Generic;


namespace weatherlogapi.controllers

{
   [Route("api/log")]
   public class WeatherLogController: ControllerBase
   {
     public WeatherLogController(AppDb db) {
       Db = db;
     }

     // GET api/log
     [HttpGet]
     public async Task<IActionResult> GetLatest() {
       await Db.Connection.OpenAsync();
       var query = new WeatherLogQuery(Db);
       var result = await query.LatestLogsAsync();
       return new OkObjectResult(result);
     }

     // GET api/log/1
     [HttpGet("{id}")]
     public async Task<IActionResult> GetOne(int id)
     {
       await Db.Connection.OpenAsync();
       var query = new WeatherLogQuery(Db);
       var result = await query.FindOneAsync(id);
       if (result is null)
        return new NotFoundResult();
      return new OkObjectResult(result);
     }


     // POST api/log
     [HttpPost]
     public async Task<IActionResult> Post([FromBody]WeatherPost body)
     {
       List<string> ls = new List<string>();
       if (body.city is null || body.city.Length < 3) ls.Add("city");
       if (body.current_temperature is null) ls.Add("current_temperature");
       if (body.city is null || body.city.Length < 3 || body.current_temperature is null) {
         var response = new { message = "One or more parameters are missing in the request body.", missing_params = ls, status = new BadRequestResult().StatusCode };
         return new BadRequestObjectResult(response);
       }
       await Db.Connection.OpenAsync();
       var query = new WeatherLogQuery(Db);
       var existing = await query.FindExistingNameAsync(body.city);
       if (existing is null) {
        body.Db = Db;
        await body.InsertAsync();
        return new OkObjectResult(body);
       } else {
         existing.read_count = existing.read_count + 1;
         existing.current_temperature = body.current_temperature;
         existing.city = body.city;
         await existing.UpdateAsync();
         return new OkObjectResult(existing);
       }
     }

     // PUT api/log/5
     [HttpPut("{id}")]
     public async Task<IActionResult> PutOne(int id, [FromBody]WeatherLogUpdateDTO body)
     {
       await Db.Connection.OpenAsync();
       var query = new WeatherLogQuery(Db);
       var result = await query.FindOneAsync(id);
       if (result is null) {
         return new NotFoundResult();
       }
       result.current_temperature = body.current_temperature;
       result.read_count = result.read_count + 1;
       await result.UpdateAsync();
       return new OkObjectResult(result);
     }

     // DELETE all logs with id less than id api/log/5
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteGroup(int id)
      {
          await Db.Connection.OpenAsync();
          var query = new WeatherLogQuery(Db);
          var result = await query.FindOneAsync(id);
          if (result is null)
              return new NotFoundResult();
          await result.DeleteAsync();
          return new OkResult();
      }

    public AppDb Db { get; }
   }

}