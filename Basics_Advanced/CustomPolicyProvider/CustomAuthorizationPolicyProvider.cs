﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Basics_Advanced.CustomPolicyProvider
{

    public class SecurityLevelAttribute : AuthorizeAttribute
    {
        public SecurityLevelAttribute(int level) 
        {
            Policy = $"{DynamicPolicies.SecurityLevel}.{level}";
        }
    }


    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }

        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";
    }

    public static class DynamicAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts[0];
            var value = parts[1];

            switch (type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim("Rank", value)
                        .Build();

                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                       .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value)))
                        .Build();

                default:
                    return null;
            }
        }
    }

    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public SecurityLevelRequirement(int level)
        {
            Level = level;
        }

        public int Level { get; }
    }

    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            SecurityLevelRequirement requirement)
        {
            var claimsValue = Convert.ToInt32(context.User.Claims
                .FirstOrDefault(x => x.Type == DynamicPolicies.SecurityLevel)?
                .Value ?? "0");

            if(requirement.Level <= claimsValue)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }

    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            foreach(var customPolicy in DynamicPolicies.Get())
            {
                if (policyName.StartsWith(customPolicy))
                {
                    var policy = DynamicAuthorizationPolicyFactory.Create(policyName);

                    return Task.FromResult(policy);
                }
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
