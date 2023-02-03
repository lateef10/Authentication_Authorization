using Microsoft.AspNetCore.Authorization;

namespace AuthenticationAuthorization.Custome_Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }

    }


    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);
            if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Now)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
