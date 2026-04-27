using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MicroServiceAspMvcProject.Models{


    public class Wallet
    {
        public Guid WalletId { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }

        // برای قفل کردن روی هر کیف پول به صورت مجزا
        public readonly object LockObject = new object();
    }


}
