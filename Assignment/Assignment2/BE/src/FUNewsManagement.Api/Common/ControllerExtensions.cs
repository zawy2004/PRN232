using System.Security.Claims;
using FUNewsManagement.BusinessLogic.Common;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Common;

public static class ControllerExtensions
{
    public static short CurrentAccountId(this ControllerBase controller) =>
        short.Parse(controller.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static bool IsAdmin(this ControllerBase controller) =>
        controller.User.IsInRole(RoleNames.Admin);
}
