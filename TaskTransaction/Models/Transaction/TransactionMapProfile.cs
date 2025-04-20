using AutoMapper;
using TaskTransaction.Models.Transaction.Dto;

namespace TaskTransaction.Models.Transaction;

public class TransactionMapProfile : Profile
{
    public TransactionMapProfile()
    {
        CreateMap<Transaction, TransactionDto>().ReverseMap();
        CreateMap<CreateTransactionDto, Transaction>();
    }
}