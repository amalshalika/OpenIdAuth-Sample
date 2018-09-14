using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Meegoda.Api.Authorization
{
    public class MustOwnTodoHandler : AuthorizationHandler<MustOwnTodoRequirment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustOwnTodoRequirment requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;

            if(filterContext==null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var todoId = int.Parse(filterContext.RouteData.Values["todoId"].ToString());

            var ownerId = context.User.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;

            if(!IsInUserTodo(todoId, ownerId)) {

                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private bool IsInUserTodo(int todoId, string imageOwnerId)
        {
            return SeedData.ListOFTodos.Any(todo => todo.TodoId == todoId && todo.OwnerId == imageOwnerId);
        }
    }
}
