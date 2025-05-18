namespace ExpenseTracker.Interfaces;

using ExpenseTracker.DTO;
using ExpenseTracker.Models;

public interface ITransactionService
{
    void AddTransaction(CreateTransactionDTO transaction);
    // void UpdateTransaction(int transactionId, TransactionDTO transaction);
    // void DeleteTransaction(int transactionId);
    List<TransactionDTO> GetTransactions(int groupId);
    TransactionDTO GetTransaction(int transactionId);
    void SettleTransactionSplit(int splitId);
}