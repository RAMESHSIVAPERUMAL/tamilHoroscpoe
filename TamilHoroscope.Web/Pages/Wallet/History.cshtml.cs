using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TamilHoroscope.Web.Data.Entities;
using TamilHoroscope.Web.Services.Interfaces;

namespace TamilHoroscope.Web.Pages.Wallet;

[Authorize]
public class HistoryModel : PageModel
{
    private readonly IWalletService _walletService;
    private readonly ILogger<HistoryModel> _logger;

    private const int PageSize = 20;

    public HistoryModel(IWalletService walletService, ILogger<HistoryModel> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    public List<Transaction> Transactions { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public async Task OnGetAsync(int page = 1)
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return;
        }

        CurrentPage = page < 1 ? 1 : page;

        TotalCount = await _walletService.GetTransactionCountAsync(userId);
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        Transactions = await _walletService.GetTransactionHistoryAsync(userId, CurrentPage, PageSize);

        _logger.LogInformation("Retrieved {Count} transactions for user {UserId}, page {Page}",
            Transactions.Count, userId, CurrentPage);
    }
}
