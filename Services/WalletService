using MicroServiceAspMvcProject.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServiceAspMvcProject.Services
{
    public class WalletService
    {


        // برای دیتابیس درون‌حافظه‌ای
        private readonly ConcurrentDictionary<Guid, Wallet> _wallets;
        private readonly List<TransactionLog> _transactionLogs;
        private readonly object _transferLock = new object();
        //ctor
        public WalletService()
        {
            _wallets = new ConcurrentDictionary<Guid, Wallet>();
            _transactionLogs = new List<TransactionLog>();
        }
        

     // ایجاد کیف پول جدید
     public Wallet CreateWallet(Guid userId, decimal initialBalance = 0){
            var wallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                UserId = userId,
                Balance = initialBalance
                };

        _wallets.TryAdd(wallet.WalletId, wallet);
        return wallet;
     }


    // دریافت کیف پول
    public Wallet GetWallet(Guid walletId)
    {
        _wallets.TryGetValue(walletId, out var wallet);
        return wallet;
    }


    // متد انتقال وجه
    public bool Transfer(Guid fromWalletId, Guid toWalletId, decimal amount){
        if (amount <= 0){
            //مبلغ باید بیشتر از صفر باشد
            return false;
        }

        // چک کردن وجود کیف‌ها
        if (!_wallets.TryGetValue(fromWalletId, out var fromWallet)){
            return false;
        }
        if (!_wallets.TryGetValue(toWalletId, out var toWallet))
        {
            return false;
        }

            //GetHashCode برای تبدیل یوآیدی به مقداری عددی تا بتوان آیدی دو کیف را باهم مقایسه کرد چرا که میخواهیم در قفل کردن آیدیها ترتیبی وجود داشته باشد 
            // برای جلوگیری از بن‌بست یعنی زمانی که یک نخ، آبجکت مورد نیاز نخی دیگر را گرفته و قفل کرده وهمزمان آن نخ آبجکت موردنیاز نخ اول را گرفته و قفل کرده (Deadlock)، 
            var firstLock = fromWalletId.GetHashCode() < toWalletId.GetHashCode() ? fromWallet.LockObject
            : toWallet.LockObject;

            var secondLock = fromWalletId.GetHashCode() < toWalletId.GetHashCode() ? toWallet.LockObject
            : fromWallet.LockObject;

        lock (firstLock)
        {
            lock (secondLock)
            {
                // بررسی موجودی ناکافی
                if (fromWallet.Balance < amount)
                {
                    //LogTransaction(null, fromWalletId, toWalletId, amount, false, $"موجودی کافی نیست. موجودی: {fromWallet.Balance}, مبلغ درخواستی: {amount}");
                    return false;
                }

                // انجام عملیات انتقال (اتمی)
                try
                {
                    fromWallet.Balance -= amount;
                    toWallet.Balance += amount;

                    // تراکنش موفق
                    LogTransaction(Guid.NewGuid(), fromWalletId, toWalletId, amount, true);

                    return true;
                }
                catch(Exception ex){
                    // تراکنش ناموفق
                    //LogTransaction(null, fromWalletId, toWalletId, amount, false, ex.Message);
                    return false;
                }
            }
        }
    }


// ثبت تراکنش‌
private void LogTransaction(Guid? transactionId, Guid fromWalletId, Guid toWalletId, decimal amount,
    bool success, string failureReason = null){
    var log = new TransactionLog
    {
        TransactionId = transactionId ?? Guid.Empty,    //مشابه با  variable1 != null ? variable1.Value : variable2
        FromWalletId = fromWalletId,
        ToWalletId = toWalletId,
        Amount = amount,
        Success = success,
        FailureReason = failureReason,
        Date = DateTime.Now
    };

    _transactionLogs.Add(log);
}


// گرفتن موجودی کیف
public decimal GetBalance(Guid walletId)
{
    if (_wallets.TryGetValue(walletId, out var wallet))
        return wallet.Balance;

    throw new KeyNotFoundException("کیف پول یافت نشد");
}


} }
