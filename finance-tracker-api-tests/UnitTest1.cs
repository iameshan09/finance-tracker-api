using finance_tracker_api.Data;
using finance_tracker_api.Dtos;
using finance_tracker_api.Services;
using finance_tracker_api.Models;
using Microsoft.EntityFrameworkCore;
using finance_tracker_api.Exceptions;

namespace finance_tracker_api_tests
{
    public class TransactionControllerTests
    {
        public readonly DbContextOptions<AppDbContext> dbContextOptions;

        public TransactionControllerTests()
        {
            // Build DbContextOptions
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }


        [Fact]
        public async Task GetTransactions_Returns_The_Correct_Response()
        {
            

            using var context = new AppDbContext(dbContextOptions);

            // Seed test data
            context.Transactions.AddRange(new List<Transaction>
            {
                new() { Id = 1, Type = TransactionType.Income, Status = TransactionStatus.Active },
                new() { Id = 2, Type = TransactionType.Expense, Status = TransactionStatus.Inactive },
                new() { Id = 3, Type = TransactionType.Income, Status = TransactionStatus.Active },
            });

            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var dto = new GetTransactionsQueryDto
            {
                Skip = 0,
                Limit = 10,
                Type = TransactionType.Income
            };

            // Act
            var result = await service.GetTransactionsAsync(dto);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Transactions, t => Assert.Equal(TransactionType.Income, t.Type));
        }

        [Fact]
        public async Task CreateTransactionAsync_Creates_Transaction()
        {
            using var context = new AppDbContext(dbContextOptions);
            var service = new TransactionService(context);

            var dto = new TransactionDto
            {
                Name = "Test Income",
                Amount = 1000,
                Type = TransactionType.Income,
                Date = DateTime.Today
            };

            var result = await service.CreateTransactionAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(TransactionStatus.Active, result.Status);
        }

        [Fact]
        public async Task UpdateTransactionAsync_Updates_Existing_Active_Transaction()
        {
            using var context = new AppDbContext(dbContextOptions);

            var existing = new Transaction
            {
                Name = "Old",
                Amount = 500,
                Type = TransactionType.Expense,
                Status = TransactionStatus.Active,
                Date = DateTime.Today.AddDays(-1)
            };

            context.Transactions.Add(existing);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var dto = new UpdateTransactionDto
            {
                Id = existing.Id,
                Name = "Updated",
                Amount = 800,
                Type = TransactionType.Income,
                Date = DateTime.Today
            };

            var result = await service.UpdateTransactionAsync(dto);

            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Amount, result.Amount);
            Assert.Equal(dto.Type, result.Type);
        }

        [Fact]
        public async Task UpdateTransactionAsync_Throws_For_Inactive_Transaction()
        {
            using var context = new AppDbContext(dbContextOptions);

            var existing = new Transaction
            {
                Name = "Old",
                Amount = 500,
                Type = TransactionType.Expense,
                Status = TransactionStatus.Inactive,
                Date = DateTime.Today
            };

            context.Transactions.Add(existing);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var dto = new UpdateTransactionDto
            {
                Id = existing.Id,
                Name = "Updated",
                Amount = 800,
                Type = TransactionType.Income,
                Date = DateTime.Today
            };

            await Assert.ThrowsAsync<ForbiddenException>(() => service.UpdateTransactionAsync(dto));
        }

        [Fact]
        public async Task GetSummaryAsync_Returns_Correct_Summary()
        {
            using var context = new AppDbContext(dbContextOptions);

            context.Transactions.AddRange(new List<Transaction>
            {
                new() { Type = TransactionType.Income, Amount = 1000, Status = TransactionStatus.Active },
                new() { Type = TransactionType.Expense, Amount = 200, Status = TransactionStatus.Active },
                new() { Type = TransactionType.Income, Amount = 500, Status = TransactionStatus.Inactive },
                new() { Type = TransactionType.Expense, Amount = 100, Status = TransactionStatus.Inactive },
            });

            await context.SaveChangesAsync();

            var service = new TransactionService(context);

            var summary = await service.GetSummaryAsync();

            Assert.Equal("800.00", summary.savings); // 1000 - 200
            Assert.Equal("1000.00", summary.incomes);
            Assert.Equal("200.00", summary.expenses);
            Assert.Equal("80", summary.savingsPercent); // 800/1000 = 80%
        }

        [Fact]
        public async Task DeactivateTransactionAsync_Deactivates_Transaction()
        {
            using var context = new AppDbContext(dbContextOptions);

            var transaction = new Transaction
            {
                Type = TransactionType.Expense,
                Amount = 300,
                Status = TransactionStatus.Active,
                Date = DateTime.Today
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);
            var result = await service.DeactivateTransactionAsync(transaction.Id);

            Assert.True(result);
            var updated = await context.Transactions.FindAsync(transaction.Id);
            Assert.Equal(TransactionStatus.Inactive, updated.Status);
        }

        [Fact]
        public async Task DeactivateTransactionAsync_Throws_If_Transaction_Not_Found()
        {
            using var context = new AppDbContext(dbContextOptions);
            var service = new TransactionService(context);

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeactivateTransactionAsync(999));
        }

        [Fact]
        public async Task DeactivateTransactionAsync_Throws_If_Already_Inactive()
        {
            using var context = new AppDbContext(dbContextOptions);

            var transaction = new Transaction
            {
                Type = TransactionType.Expense,
                Amount = 300,
                Status = TransactionStatus.Inactive,
                Date = DateTime.Today
            };

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var service = new TransactionService(context);
            await Assert.ThrowsAsync<NotFoundException>(() => service.DeactivateTransactionAsync(transaction.Id));
        }
    
}
}