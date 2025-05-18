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

    public void SettleTransactionSplit(SettleUserIdDTO settleUserIdDTO)
    {
        var transactionSplit = _context.TransactionSplits
            .Include(ts => ts.Transaction)
            .FirstOrDefault(ts => ts.Id == settleUserIdDTO.SplitId && ts.UserId == settleUserIdDTO.UserId);

        if (transactionSplit == null)
        {
            throw new ArgumentException("Transaction split not found");
        }
        var groupMember = _context.GroupMembers
            .FirstOrDefault(gm => gm.UserId == settleUserIdDTO.UserId && gm.GroupId == transactionSplit.Transaction.GroupId);
        if (groupMember == null)
        {
            throw new ArgumentException("Group member not found");
        }
        groupMember.Balance -= transactionSplit.Amount;
        if(groupMember.Balance == 0)
        {
            groupMember.IsSettled = true;
        }

        _context.GroupMembers.Update(groupMember);

        transactionSplit.IsSettled = true;
        _context.TransactionSplits.Update(transactionSplit);
        _context.SaveChanges();
    }
    public List<SettleDTO> GetSettles(UserGroupIdDTO dto)
    {
        var transactionSplits = _context.TransactionSplits
            .Include(ts => ts.User)
            .Include(ts => ts.Transaction)
            .Include(ts => ts.Transaction.User)
            .Where(ts => ts.Transaction.GroupId == dto.GroupId && ts.UserId == dto.UserId && !ts.IsSettled)
            .Select(ts => new SettleDTO
            {
                SplitId = ts.Id,
                UserName = ts.Transaction.User.Name,
                Amount = ts.Amount
            })
            .ToList();

        return transactionSplits;
    }

    public TransactionDTO GetTransaction(UserTransactionIdDTO userTransactionDTO)
    {
        var transaction = _context.Transactions
            .Include(t => t.User)
            .Include(t => t.Group)
            .FirstOrDefault(t => t.Id == userTransactionDTO.TransactionId);

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
            GroupId = transaction.GroupId,
        };
    }

    public void AddTransaction(CreateTransactionDTO transactionDTO)
    {
        var group = _context.Groups.Include(g => g.Members).FirstOrDefault(g => g.Id == transactionDTO.GroupId);
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
                    var groupMember = _context.GroupMembers
                        .FirstOrDefault(gm => gm.UserId == member.UserId && gm.GroupId == transactionDTO.GroupId);
                    if (groupMember == null)
                    {
                        throw new ArgumentException("Group member not found");
                    }
                    if (member.UserId == transactionDTO.UserId)
                    {
                        continue;
                    }
                    groupMember.Balance += amountPerUser;
                    groupMember.IsSettled = false;
                    _context.GroupMembers.Update(groupMember);

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
                    if(splitUser.Id == transactionDTO.UserId)
                    {
                        continue;
                    }

                    double percentage = (double)split.Amount;
                    if (percentage < 0 || percentage > 100)
                    {
                        throw new ArgumentException("Percentage must be between 0 and 100");
                    }

                    var groupMember = _context.GroupMembers
                        .FirstOrDefault(gm => gm.UserId == split.UserId && gm.GroupId == transactionDTO.GroupId);
                    if (groupMember == null)
                    {
                        throw new ArgumentException("Group member not found");
                    }
                    groupMember.Balance += transactionDTO.Amount * (decimal)(percentage / 100);
                    groupMember.IsSettled = false;
                    _context.GroupMembers.Update(groupMember);
                    TransactionSplit newSplit = new TransactionSplit
                    {
                        UserId = split.UserId,
                        Amount = transactionDTO.Amount * ((decimal)percentage / 100),
                        IsSettled = false,
                        TransactionId = transaction.Id,
                        Transaction = transaction,
                        User = splitUser
                    };
                    splits.Add(newSplit);
                }
                _context.TransactionSplits.AddRange(splits);
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
                    if (splitUser.Id == transactionDTO.UserId)
                    {
                        continue;
                    }
                    var groupMember = _context.GroupMembers
                        .FirstOrDefault(gm => gm.UserId == split.UserId && gm.GroupId == transactionDTO.GroupId);
                    if (groupMember == null)
                    {
                        throw new ArgumentException("Group member not found");
                    }
                    groupMember.Balance += split.Amount;
                    groupMember.IsSettled = false;
                    _context.GroupMembers.Update(groupMember);

                    TransactionSplit newSplit = new TransactionSplit
                    {
                        UserId = split.UserId,
                        Amount = split.Amount,
                        IsSettled = false,
                        Transaction = transaction,
                        TransactionId = transaction.Id,
                        User = splitUser
                    };
                    splits.Add(newSplit);
                }
                _context.TransactionSplits.AddRange(splits);
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