using finance_tracker_api.Dtos;
using finance_tracker_api.Models;
using finance_tracker_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace finance_tracker_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController(TransactionService service) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction(TransactionDto body)
        {
            var result = await service.CreateTransactionAsync(body);
            return Ok(result);
        }

        [HttpPatch]
        public async Task<ActionResult<Transaction>> UpdateTransaction(UpdateTransactionDto body)
        {
            var result = await service.UpdateTransactionAsync(body);
            return Ok(result);
        }


        [HttpGet]
        public async Task<ActionResult<PaginatedTransactionsDto>> GetTransactions([FromQuery] GetTransactionsQueryDto query)
        {
            var result = await service.GetTransactionsAsync(query);
            return Ok(result);
        }


        [HttpGet("summary")]
        public async Task<ActionResult<TransactionSummary>> GetSummary()
        {
            var result = await service.GetSummaryAsync();
            return Ok(result);
        }

        [HttpGet("single/{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var result = await service.GetTransactionAsync(id);
            return Ok(result);
        }

        [HttpPatch("deativate")]
        public async Task<ActionResult<Transaction>> DeativateTransaction(DeativateTransactionDto body)
        {
            var result = await service.DeactivateTransactionAsync(body.Id);
            return Ok(result);
        }


        [HttpDelete("clear")]
        public async Task<ActionResult<Boolean>> ClearTx()
        {
            var result = await service.ClearTx();
            return Ok(result);
        }
    }
}
