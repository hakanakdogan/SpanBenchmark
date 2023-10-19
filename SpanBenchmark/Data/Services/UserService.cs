using SpanBenchmark.Data.Model;
using System.Text.Json;
using System.Text;
using SpanBenchmark.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;

namespace SpanBenchmark.Data.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GetEmployees()
        {
            var employees = await _context.Users.Select(x => x.Employee).ToListAsync();
            var jsonEmp = JsonSerializer.Serialize(employees);

            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\akdog\\source\\repos\\hakanakdogan\\SpanBenchmark\\Test.txt");

                sw.WriteLine(jsonEmp);

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        public async Task BulkUpsertUserDefault()
        {
            string employeesString = "";
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("C:\\Users\\akdog\\source\\repos\\hakanakdogan\\SpanBenchmark\\SpanBenchmark\\Test.txt");
                //Read the first line of text
                employeesString = sr.ReadLine();
                //Continue to read until you reach end of file
                //var deneme = sr.ReadLine();
                //while (sr.ReadLine() != null)
                //{
                //    //write the line to console window
                //    Console.WriteLine(employeesString);
                //    //Read the next line
                //    employeesString += sr.ReadLine();
                //}
                //close the file
                sr.Close();
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

            var employeesList = JsonSerializer.Deserialize<List<JsonDocument>>(employeesString);

            List<User> newEmployees = new();
            List<User> quittedEmployees = new();
            List<UserDifferenceDto> modifiedUsers = new();
            foreach (var employee in employeesList)
            {
                var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(employee).ToJsonString());

                var user = await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserName == modifiedJson.RootElement.GetProperty("identityNumber").GetString());

                if (user is null)
                {
                   
                    newEmployees.Add(new User
                    {
                        UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
                        Employee = modifiedJson,
                        IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
                    });
                    continue;
                }
                if (!user.Employee.DeepEquals(modifiedJson))
                {

                    var diff = modifiedJson.Deserialize<JsonNode>().Diff(user.Employee.Deserialize<JsonNode>(), new JsonDiffOptions
                    {
                        PropertyFilter = (key, context) =>
                        {
                            if (key == "isActive" || key == "avatar")
                                return false;
                            return true;
                        }

                    });
                    if (diff is JsonNode)
                    {
                        modifiedUsers.Add(new UserDifferenceDto
                        {
                            User = user,
                            //Difference = FormatChangesToString(diff.Deserialize<Dictionary<string, string[]>>(new JsonSerializerOptions
                            //{
                            //    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString
                            //}))
                        });
                    }

                }

                if (employee.RootElement.GetProperty("isActive").GetBoolean() is true &&
                            user.Employee.RootElement.GetProperty("isActive").GetBoolean() is false)
                    newEmployees.Add(user);
                else if (employee.RootElement.GetProperty("isActive").GetBoolean() is false &&
                        user.Employee.RootElement.GetProperty("isActive").GetBoolean())
                    quittedEmployees.Add(user);
            }
            Console.WriteLine("no error haha");
        }




        public Task BulkUpsertUserSpan()
        {
            string employeesString = "";
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("C:\\Users\\akdog\\source\\repos\\hakanakdogan\\SpanBenchmark\\SpanBenchmark\\Test.txt");
                //Read the first line of text
                employeesString = sr.ReadLine();
                //Continue to read until you reach end of file
                //var deneme = sr.ReadLine();
                //while (sr.ReadLine() != null)
                //{
                //    //write the line to console window
                //    Console.WriteLine(employeesString);
                //    //Read the next line
                //    employeesString += sr.ReadLine();
                //}
                //close the file
                sr.Close();
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

            var employeesList = CollectionsMarshal.AsSpan(JsonSerializer.Deserialize<List<JsonDocument>>(employeesString));

            
            List<User> newEmployees = new();
            List<User> quittedEmployees = new();
            List<UserDifferenceDto> modifiedUsers = new();
            foreach (var employee in employeesList)
            {
                var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(employee).ToJsonString());

                var user =  _context.Users.AsNoTracking()
                    .FirstOrDefault(x => x.UserName == modifiedJson.RootElement.GetProperty("identityNumber").GetString());

                if (user is null)
                {

                    newEmployees.Add(new User
                    {
                        UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
                        Employee = modifiedJson,
                        IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
                    });
                    continue;
                }
                if (!user.Employee.DeepEquals(modifiedJson))
                {

                    var diff = modifiedJson.Deserialize<JsonNode>().Diff(user.Employee.Deserialize<JsonNode>(), new JsonDiffOptions
                    {
                        PropertyFilter = (key, context) =>
                        {
                            if (key == "isActive" || key == "avatar")
                                return false;
                            return true;
                        }

                    });
                    if (diff is JsonNode)
                    {
                        modifiedUsers.Add(new UserDifferenceDto
                        {
                            User = user,
                            //Difference = FormatChangesToString(diff.Deserialize<Dictionary<string, string[]>>(new JsonSerializerOptions
                            //{
                            //    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString
                            //}))
                        });
                    }

                }

                if (employee.RootElement.GetProperty("isActive").GetBoolean() is true &&
                            user.Employee.RootElement.GetProperty("isActive").GetBoolean() is false)
                    newEmployees.Add(user);
                else if (employee.RootElement.GetProperty("isActive").GetBoolean() is false &&
                        user.Employee.RootElement.GetProperty("isActive").GetBoolean())
                    quittedEmployees.Add(user);
            }

            return Task.CompletedTask;
        }




        private JsonNode SetServiceDataProperty(JsonDocument employee)
        {
            var rawJson = JsonNode.Parse(employee.RootElement.GetRawText());
            if (!employee.RootElement.TryGetProperty("isServiceData", out _))
                rawJson.AsObject().Add("isServiceData", true);
            else
                rawJson["isServiceData"] = true;
            return rawJson;
        }

        private string FormatChangesToString(Dictionary<string, string[]> diff)
        {
            StringBuilder sb = new();

            for (int i = 0; i < diff.Count; i++)
            {
                sb.Append($"{diff.ElementAt(i).Key} = {(diff.ElementAt(i).Value.Length > 1 ? diff.ElementAt(i).Value[1].Replace("0", string.Empty) : diff.ElementAt(i).Value[0])} " +
                    $"-> {(diff.ElementAt(i).Value.Length > 1 ? diff.ElementAt(i).Value[0] : string.Empty)}");
                if (diff.Count - 1 > i)
                    sb.Append("," + Environment.NewLine);
            }

            return sb.ToString();
        }

    }
}
