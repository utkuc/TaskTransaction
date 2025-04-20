using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskTransaction.Models.Transaction;
using TaskTransaction.Models.Transaction.Dto;
using TaskTransaction.Models.User.Dto;
using TaskTransaction.Services;

namespace TaskTransaction.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController(
    ILogger<TransactionController> logger,
    TransactionService transactionService,
    UserService userService,
    IMapper autoMapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        var transactions = await transactionService.GetAllTransactionsAsync();
        logger.LogInformation("Fetched all transactions, Fetch Count: {TransactionCount}", transactions.Count());
        return Ok(autoMapper.Map<List<TransactionDto>>(transactions));
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransactionById(long transactionId)
    {
        var transaction = await transactionService.GetTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            logger.LogWarning("Transaction with id {Id} not found to fetch", transactionId);
            return StatusCode(404);
        }

        logger.LogInformation("Fetched transaction with ID: {Id}", transaction.TransactionID);
        return Ok(autoMapper.Map<TransactionDto>(transaction));
    }

    [HttpPost("createTransaction")]
    public async Task<IActionResult> CreateTransaction(CreateTransactionDto createTransactionDto)
    {
        if (createTransactionDto == null || string.IsNullOrWhiteSpace(createTransactionDto.UserID))
        {
            logger.LogWarning("CreateTransaction request body is not valid.");
            return StatusCode(400, "No user has been provided.");
        }

        var user = await userService.GetUserByIdAsync(createTransactionDto.UserID);
        if (user == null)
        {
            logger.LogWarning("User with ID {Id} not found.", createTransactionDto.UserID);
            return StatusCode(400, "User with given Id is not valid");
        }

        Transaction newTransaction;
        try
        {
            newTransaction = autoMapper.Map<Transaction>(createTransactionDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Automapper could not map createTransactionDto to transaction");
            return StatusCode(500, e.Message);
        }

        await transactionService.CreateTransactionAsync(newTransaction);
        logger.LogInformation("Transaction with Id {UserId} created", newTransaction.UserID);

        var transactionDto = autoMapper.Map<TransactionDto>(newTransaction);
        return StatusCode(201, transactionDto);
    }

    [HttpGet("getTransactionSumByType")]
    public async Task<IActionResult> GetTransactionSumByType()
    {
        var result = await transactionService.GetTotalAmountByTransactionTypeAsync();
        return StatusCode(200, result);
    }

    [HttpGet("identifyTransactionsAboveThreshold/{threshold}")]
    public async Task<IActionResult> GetTransactionSumByType(decimal threshold)
    {
        var result = await transactionService.GetTransactionsAboveThresholdAsync(threshold);
        var transactionDtos = autoMapper.Map<List<TransactionDto>>(result);
        return StatusCode(200, transactionDtos);
    }
}