using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Meegoda.Api.Controllers
{

    [Produces("application/json")]
    [Route("api/usertodo")]
    [Authorize]
    public class TodoApiController : Controller
    {
        [HttpGet]
        public IActionResult GetUserToDos()
        {
            var listOfUserTodos = SeedData.ListOFTodos.Where(todo => todo.OwnerId == User.Claims.FirstOrDefault(c => c.Type == "sub").Value);
            return Ok(listOfUserTodos);
        }

        [Authorize("MustOwnTodo")]
        [HttpGet("{todoId}")]
        public IActionResult GetDotoById(int todoId)
        {
            var listOfUserTodos = SeedData.ListOFTodos.SingleOrDefault(todo => todo.TodoId == todoId);
            return Ok(listOfUserTodos);
        }

        [Authorize("MustOwnTodo")]
        [HttpPost("update/{todoId}")]
        public IActionResult UpdateTodo(int todoId, [FromBody]UserToDo userToDo)
        {

            var todoTobeUpdate = SeedData.ListOFTodos.Single(todo => todo.OwnerId == User.Claims.FirstOrDefault(c => c.Type == "sub").Value
            && todo.TodoId == todoId);

            todoTobeUpdate.TodoTitle = userToDo.TodoTitle;

            return Ok();

        }

        [HttpPost("create")]
        [Authorize(Roles = "payinguser")]
        public IActionResult Create([FromBody]UserToDo newTodo)
        {

            var nextTodoId = SeedData.ListOFTodos.Count + 1;
            newTodo.OwnerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            newTodo.TodoId = nextTodoId;

            SeedData.ListOFTodos.Add(newTodo);

            return Created($"api/usertodo/{nextTodoId}", newTodo);

        }

        [Authorize("MustOwnTodo")]
        [HttpDelete("{todoId}")]
        public IActionResult DeleteTodo(int todoId)
        {
            var todoToBeRemove = SeedData.ListOFTodos.Single(todo => todo.OwnerId == User.Claims.FirstOrDefault(c => c.Type == "sub").Value
           && todo.TodoId == todoId);

            SeedData.ListOFTodos.Remove(todoToBeRemove);

            return Ok();
        }
        private bool IsInUserTodo(int todoId)
        {
            return SeedData.ListOFTodos.Any(todo => todo.TodoId == todoId && todo.OwnerId == User.Claims.FirstOrDefault(c => c.Type == "sub").Value);
        }
    }
}