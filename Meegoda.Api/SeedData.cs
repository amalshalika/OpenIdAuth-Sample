using Meegoda.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meegoda.Api
{
    public class SeedData
    {
        public static List<UserToDo> ListOFTodos = new List<UserToDo>() {
                new UserToDo() {TodoId=1, OwnerName="Amal Shalika",TodoTitle = "Asp.net core 2.0", Description= "Studing ASP.net core 2.0",
                                Status = "InProgress", StartDate = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(7), OwnerId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7"},
                new UserToDo() {TodoId=2, OwnerName = "Vidun Theujan" ,TodoTitle = "EF Core", Description= "Studing EF core 2.0", Status = "InProgress",
                                StartDate = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(7),OwnerId = Guid.NewGuid().ToString()},
                 new UserToDo() {TodoId= 3,OwnerName = "Ganul" ,TodoTitle = "Identity Server", Description= "Learning Identity Server4", Status = "InProgress",
                                StartDate = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(7), OwnerId = "d860efca-22d9-47fd-8249-791ba61b07c7"} };
    }
}
