﻿using ego_auto.BFF.Domain.Common;
using ego_auto.BFF.Domain.Entities;
using ego_auto.BFF.Domain.Requests.Payment;

namespace ego_auto.BFF.Application.Contracts.Persistence;

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentByIdAsync(int id);
    Task<PaginatedResult<Payment>> GetPaymentsAsync(GetPaymentsRequest request);
    Task UpsertPayment(PaymentRequest request);
}
