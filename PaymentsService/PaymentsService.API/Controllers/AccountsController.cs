using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(AccountService accountService) : ControllerBase
{
    /// <summary>
    /// Получить информацию о счете работодателя
    /// </summary>
    [HttpGet("employer")]
    public async Task<IActionResult> GetEmployerAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Не удалось определить пользователя.");
        }

        var accountInfo = await accountService.GetEmployerAccountAsync(Guid.Parse(userId));
        if (accountInfo == null)
        {
            return NotFound("У работодателя нет привязанного счета.");
        }

        return Ok(accountInfo);
    }

    /// <summary>
    /// Получить информацию о счете фрилансера
    /// </summary>
    [HttpGet("freelancer")]
    public async Task<IActionResult> GetFreelancerAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Не удалось определить пользователя.");
        }

        var accountInfo = await accountService.GetFreelancerAccountAsync(Guid.Parse(userId));
        if (accountInfo == null)
        {
            return NotFound("У фрилансера нет привязанного счета.");
        }

        return Ok(accountInfo);
    }
}