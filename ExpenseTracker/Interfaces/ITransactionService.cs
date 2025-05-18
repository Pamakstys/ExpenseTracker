namespace ExpenseTracker.Interfaces;

using ExpenseTracker.DTO;
using ExpenseTracker.Models;

public interface ITransactionService
{
    void AddTransaction(CreateTransactionDTO transaction);
    TransactionDTO GetTransaction(UserTransactionIdDTO userTransactionDTO);
    void SettleTransactionSplit(SettleUserIdDTO settleUserIdDto);
    List<SettleDTO> GetSettles(UserGroupIdDTO userGroupIdDto);
}