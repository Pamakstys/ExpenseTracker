namespace ExpenseTracker.Services;

using ExpenseTracker.Interfaces;
using ExpenseTracker.Models;
using ExpenseTracker.DTO;
using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public void SettleTransactionSplit(int splitId)
    {
        var transactionSplit = _context.TransactionSplits
            .Include(ts => ts.Transaction)
            .FirstOrDefault(ts => ts.Id == splitId);

        if (transactionSplit == null)
        {
            throw new ArgumentException("Transaction split not found");
        }

        transactionSplit.IsSettled = true;
        _context.TransactionSplits.Update(transactionSplit);
        _context.SaveChanges();
    }

    public TransactionDTO GetTransaction(int transactionId)
    {
        var transaction = _context.Transactions
            .Include(t => t.User)
            .Include(t => t.Group)
            .FirstOrDefault(t => t.Id == transactionId);

        if (transaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        return new TransactionDTO
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Date = transaction.Date,
            UserId = transaction.UserId,
            UserName = transaction.User.Name,
            GroupId = transaction.GroupId
        };
    }

    public List<TransactionDTO> GetTransactions(int groupId)
    {
        var transactions = _context.Transactions
            .Include(t => t.User)
            .Include(t => t.Group)
            .Where(t => t.GroupId == groupId)
            .Select(t => new TransactionDTO
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                UserId = t.UserId,
                UserName = t.User.Name,
                GroupId = t.GroupId
            })
            .ToList();

        return transactions;
    }

    public void AddTransaction(CreateTransactionDTO transactionDTO)
    {
        var group = _context.Groups.FirstOrDefault(g => g.Id == transactionDTO.GroupId);
        if (group == null)
        {
            throw new ArgumentException("Group not found");
        }

        Transaction transaction = new Transaction();
        transaction.Amount = transactionDTO.Amount;
        transaction.Description = transactionDTO.Description;
        transaction.UserId = transactionDTO.UserId;
        transaction.GroupId = transactionDTO.GroupId;
        transaction.Group = group;
        User? user = _context.Users.FirstOrDefault(u => u.Id == transactionDTO.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        transaction.User = user;
        transaction.Date = DateTime.Now;
        transaction.SplitType = transactionDTO.SplitType;
        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        transaction.TransactionSplits = new List<TransactionSplit>();
        List<TransactionSplit> splits = new List<TransactionSplit>();
        switch (transactionDTO.SplitType)
        {
            case "Equally":
                int memberCount = group.Members.Count;
                decimal rawAmountPerUser = transactionDTO.Amount / memberCount;
                decimal amountPerUser = Math.Floor(rawAmountPerUser * 100) / 100;


                foreach (var member in group.Members)
                {
                    TransactionSplit split = new TransactionSplit
                    {
                        TransactionId = transaction.Id,
                        UserId = member.UserId,
                        Amount = amountPerUser,
                        IsSettled = false,
                        Transaction = transaction,
                        User = member.User
                    };
                    splits.Add(split);
                }
                _context.TransactionSplits.AddRange(splits);
                _context.SaveChanges();
                break;
            case "Percentage":
                double totalPercentage = 0;
                foreach (var split in transactionDTO.Splits)
                {
                    totalPercentage += (double)split.Amount;
                }
                if (totalPercentage != 100)
                {
                    throw new ArgumentException("Total percentage must be 100");
                }

                foreach (var split in transactionDTO.Splits)
                {
                    var splitUser = _context.Users.FirstOrDefault(u => u.Id == split.UserId);
                    if (splitUser == null)
                    {
                        throw new ArgumentException("User not found");
                    }
                    double percentage = (double)split.Amount;
                    if (percentage < 0 || percentage > 100)
                    {
                        throw new ArgumentException("Percentage must be between 0 and 100");
                    }

                    TransactionSplit newSplit = new TransactionSplit
                    {
                        UserId = split.UserId,
                        Amount = transactionDTO.Amount * (split.Amount / 100),
                        IsSettled = false,
                        Transaction = transaction,
                        User = user
                    };
                    splits.Add(newSplit);
                }
                _context.TransactionSplits.AddRange(transaction.TransactionSplits);
                _context.SaveChanges();
                break;
            case "Dynamic":
                decimal totalAmount = 0;
                foreach (var split in transactionDTO.Splits)
                {
                    totalAmount += split.Amount;
                }
                if (totalAmount != transactionDTO.Amount)
                {
                    throw new ArgumentException("Total amount must be equal to the transaction amount");
                }
                foreach (var split in transactionDTO.Splits)
                {
                    var splitUser = _context.Users.FirstOrDefault(u => u.Id == split.UserId);
                    if (splitUser == null)
                    {
                        throw new ArgumentException("User not found");
                    }

                    TransactionSplit newSplit = new TransactionSplit
                    {
                        UserId = split.UserId,
                        Amount = split.Amount,
                        IsSettled = false,
                        Transaction = transaction,
                        User = user
                    };
                    splits.Add(newSplit);
                }
                _context.TransactionSplits.AddRange(transaction.TransactionSplits);
                _context.SaveChanges();
                break;
            default:
                throw new ArgumentException("Invalid Split Type");
        }

        transaction.TransactionSplits = splits;
        _context.Transactions.Update(transaction);
        _context.SaveChanges();
    }
}