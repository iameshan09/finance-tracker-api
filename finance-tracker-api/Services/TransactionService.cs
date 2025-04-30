using finance_tracker_api.Data;
using finance_tracker_api.Dtos;
using finance_tracker_api.Exceptions;
using finance_tracker_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace finance_tracker_api.Services
{
    public class TransactionService(AppDbContext appDbContext)
    {
        public async Task<Transaction> CreateTransactionAsync(TransactionDto dto)
        {
            var transaction = new Transaction
            {
                Name = dto.Name,
                Amount = dto.Amount,
                Type = dto.Type,
                Status = TransactionStatus.Active,
                Date = dto.Date,
            };
            appDbContext.Transactions.Add(transaction);
            await appDbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(UpdateTransactionDto dto)
        {
            var transaction = await appDbContext.Transactions.FindAsync(dto.Id) ?? throw new NotFoundException($"Transaction with Id {dto.Id} not found.");
            if (transaction.Status != TransactionStatus.Active)
            {
                throw new ForbiddenException($"Transaction with Id {dto.Id} is not active");
            }
            transaction.Name = dto.Name;
            transaction.Amount = dto.Amount;
            transaction.Type = dto.Type;
            transaction.Date = dto.Date;

            await appDbContext.SaveChangesAsync();
            return transaction;
        }


        public async Task<PaginatedTransactionsDto> GetTransactionsAsync(GetTransactionsQueryDto dto)
        {
            var query = appDbContext.Transactions.AsQueryable();

            query = query.Where(t => t.Status == TransactionStatus.Active);

            query = query.OrderByDescending(t => t.Id);

            if (dto.Type.HasValue)
            {
                query = query.Where(t => t.Type == dto.Type.Value);
            }

            

            var totalCount = await query.CountAsync();

            var transactions = await query
                .Skip(dto.Skip)
                .Take(dto.Limit)
                .ToListAsync();

            return new PaginatedTransactionsDto
            {
                TotalCount = totalCount,
                Transactions = transactions
            };
        }


        public async Task<TransactionSummary> GetSummaryAsync()
        {
            var incomes = await appDbContext.Transactions
                .Where(t => t.Type == TransactionType.Income & t.Status == TransactionStatus.Active)
                .SumAsync(t => t.Amount);

            var expenses = await appDbContext.Transactions
                .Where(t => t.Type == TransactionType.Expense & t.Status == TransactionStatus.Active)
                .SumAsync(t => t.Amount);

            var savings = incomes - expenses;

            int savingPercent = incomes != 0
       ? Math.Clamp((int)Math.Round((savings / incomes) * 100), -100, 100)
       : 0;

            return new TransactionSummary
            {
                savings = savings.ToString("F2"),
                incomes = incomes.ToString("F2"),
                expenses = expenses.ToString("F2"),
                savingsPercent = savingPercent.ToString()
            };
        }

        public async Task<Transaction> GetTransactionAsync(int id)
        {
            var transaction = await appDbContext.Transactions.FindAsync(id) ?? throw new NotFoundException($"Transaction with Id {id} not found.");
            return transaction;
        }

        public async Task<bool> DeactivateTransactionAsync(int id)
        {
            var transaction = await appDbContext.Transactions.FindAsync(id);

            if (transaction == null || transaction.Status == TransactionStatus.Inactive) throw new NotFoundException($"Transaction with Id {id} not found.");

            transaction.Status = TransactionStatus.Inactive;
            appDbContext.Transactions.Update(transaction);

            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearTx()
        {
            var transactions = await appDbContext.Transactions.ToListAsync();

            // Remove all records
            appDbContext.Transactions.RemoveRange(transactions);

            // Save changes to the database
            await appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
