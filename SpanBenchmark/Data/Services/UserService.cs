//using SpanBenchmark.Data.Model;
//using System.Text.Json;
//using System.Text;
//using SpanBenchmark.Data.Dtos;
//using Microsoft.EntityFrameworkCore;
//using System.Text.Json.JsonDiffPatch;
//using System.Text.Json.Nodes;
//using System.Runtime.InteropServices;

//namespace SpanBenchmark.Data.Services
//{
//    public class UserService
//    {
//        private readonly List<JsonDocument> _employeesList;

//        public UserService(List<JsonDocument> employeesList)
//        {
//            _employeesList = employeesList;
//        }

//        //public async Task GetEmployees()
//        //{
//        //    var employees = await _context.Users.Select(x => x.Employee).ToListAsync();
//        //    var jsonEmp = JsonSerializer.Serialize(employees);

//        //    try
//        //    {
//        //        StreamWriter sw = new StreamWriter("C:\\Users\\akdog\\source\\repos\\hakanakdogan\\SpanBenchmark\\Test.txt");

//        //        sw.WriteLine(jsonEmp);

//        //        sw.Close();
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        Console.WriteLine("Exception: " + e.Message);
//        //    }
//        //    finally
//        //    {
//        //        Console.WriteLine("Executing finally block.");
//        //    }
//        //}

//        public async Task BulkUpsertUserDefault()
//        {
//            List<User> newEmployees = new();
//            List<User> quittedEmployees = new();
//            List<UserDifferenceDto> modifiedUsers = new();
//            foreach (var employee in _employeesList)
//            {
//                var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(employee).ToJsonString());


//                if (employee.RootElement.GetProperty("identityNumber").GetString().StartsWith("1"))
//                {
                   
//                    newEmployees.Add(new User
//                    {
//                        UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
//                        Employee = modifiedJson,
//                        IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
//                    });
//                    continue;
//                }
//                if (employee.RootElement.GetProperty("identityNumber").GetString().StartsWith("2"))
//                {

//                    quittedEmployees.Add(new User
//                    {
//                        UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
//                        Employee = modifiedJson,
//                        IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
//                    });
//                    continue;

//                }
//                else
//                {
//                    modifiedUsers.Add(new UserDifferenceDto
//                    {
//                        User = new User
//                        {
//                            UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
//                            Employee = modifiedJson,
//                            IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
//                        }
//                    });
//                    continue;
//                }

//            }
//        }




//        public Task BulkUpsertUserSpan()
//        {
//            Span<User> newEmployees = new();
//            Span<User> quittedEmployees = new();
//            Span<UserDifferenceDto> modifiedUsers = new();
//            for (int i = 0; i< _employeesList.Count; i++)
//            {
                
//                var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(_employeesList[i]).ToJsonString());


//                if (_employeesList[i].RootElement.GetProperty("identityNumber").GetString().StartsWith("1"))
//                {

//                    newEmployees.Fill(new User
//                    {
//                        UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
//                        Employee = modifiedJson,
//                        IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
//                    });
//                    continue;
//                }
//                if (_employeesList[i].RootElement.GetProperty("identityNumber").GetString().StartsWith("2"))
//                {

//                    quittedEmployees.Fill(new User
//                    {
//                        UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
//                        Employee = modifiedJson,
//                        IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
//                    });
//                    continue;

//                }
//                else
//                {
//                    modifiedUsers.Fill(new UserDifferenceDto
//                    {
//                        User = new User
//                        {
//                            UserName = modifiedJson.RootElement.GetProperty("identityNumber").GetString(),
//                            Employee = modifiedJson,
//                            IsActive = modifiedJson.RootElement.GetProperty("isActive").GetBoolean(),
//                        }
//                    });
//                    continue;
//                }

//            }
//            return Task.CompletedTask;
//        }

//        public void SpanReplace()
//        {
//            foreach (var employee in CollectionsMarshal.AsSpan(_employeesList))
//            {
//                var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(employee).ToJsonString());
//                list.Fill(modifiedJson.RootElement.GetProperty("identityNumber").GetString());
                
//            }
//            foreach (var item in list)
//            {
//                ReplaceSpan(item, "1", "2");
//            }
            
//        }

//        public void ListReplace()
//        {
//            List<string> list = new();
//            foreach (var employee in _employeesList)
//            {
//                var modifiedJson = JsonDocument.Parse(SetServiceDataProperty(employee).ToJsonString());
//                list.Add(modifiedJson.RootElement.GetProperty("identityNumber").GetString());

//            }
//            foreach (var item in list)
//            {
//                item.Replace("1", "2");
//            }
//        }



//        private JsonNode SetServiceDataProperty(JsonDocument employee)
//        {
//            var rawJson = JsonNode.Parse(employee.RootElement.GetRawText());
//            if (!employee.RootElement.TryGetProperty("isServiceData", out _))
//                rawJson.AsObject().Add("isServiceData", true);
//            else
//                rawJson["isServiceData"] = true;
//            return rawJson;
//        }

//        private static void ReplaceSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
//        {
//            int indexOfSearch = source.IndexOf(oldValue);
//            int resultLength = source.Length + (newValue.Length - oldValue.Length);
//            Span<char> result = stackalloc char[resultLength];
//            source.Slice(0, indexOfSearch).CopyTo(result);
//            newValue.CopyTo(result.Slice(indexOfSearch));
//            source.Slice(indexOfSearch + oldValue.Length).CopyTo(result.Slice(indexOfSearch + newValue.Length));
//        }

//        private string FormatChangesToString(Dictionary<string, string[]> diff)
//        {
//            StringBuilder sb = new();

//            for (int i = 0; i < diff.Count; i++)
//            {
//                sb.Append($"{diff.ElementAt(i).Key} = {(diff.ElementAt(i).Value.Length > 1 ? diff.ElementAt(i).Value[1].Replace("0", string.Empty) : diff.ElementAt(i).Value[0])} " +
//                    $"-> {(diff.ElementAt(i).Value.Length > 1 ? diff.ElementAt(i).Value[0] : string.Empty)}");
//                if (diff.Count - 1 > i)
//                    sb.Append("," + Environment.NewLine);
//            }

//            return sb.ToString();
//        }

//    }
//}
