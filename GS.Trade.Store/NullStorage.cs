namespace GS.Trade.TradeStorage
{
    public class NullStorage

{
        public string Name { get; set; }

        public NullStorage()
        {
        }
}
//    public abstract class NullStorage : IStorage 
//    {
//        public event EventHandler<IEventArgs> StorageChangedEvent;

//        protected virtual void OnStorageChangedEvent(IEventArgs e)
//        {
//            EventHandler<IEventArgs> handler = StorageChangedEvent;
//            if (handler != null) handler(this, e);
//        }

//        public void Add(IOrder3 ord)
//        {
            
//        }

//        public void Add(ITrade3 tr)
//        {
//            throw new NotImplementedException();
//        }

//        public IOrder GetOrderByKey(string key)
//        {
//            throw new NotImplementedException();
//        }

//        public IStrategy GetStrategyByKey(string key)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetStrategyKeyFromOrder(string orderKey)
//        {
//            throw new NotImplementedException();
//        }

//        public IStrategy Register(IStrategy s)
//        {
//            throw new NotImplementedException();
//        }

//        public IAccount Register(IAccount a)
//        {
//            throw new NotImplementedException();
//        }

//        public ITicker Register(ITicker t)
//        {
//            throw new NotImplementedException();
//        }

//        public int SaveChanges(StorageOperationEnum operation, IPosition2 p)
//        {
//            switch (operation)
//            {
//                case StorageOperationEnum.Update:
//                    OnStorageChangedEvent(new Events.EventArgs
//                    {
//                        Category = "UI.Positions",
//                        Entity = "Current",
//                        Operation = "Update",
//                        Object = p
//                    });
//                    break;
//            }
//            return 1;
//        }

//        public int SaveChanges(StorageOperationEnum operation, IPositionTotal2 p)
//        {
//            switch (operation)
//            {
//                case StorageOperationEnum.Update:
//                    OnStorageChangedEvent(new Events.EventArgs
//                    {
//                        Category = "UI.Positions",
//                        Entity = "Total",
//                        Operation = "Update",
//                        Object = p
//                    });
//                    break;
//            }
//            return 1;
//        }

//        public int SaveChanges(StorageOperationEnum operation, IDeal p)
//        {
//            OnStorageChangedEvent(new Events.EventArgs
//            {
//                Category = "Deals",
//                Entity = "Deal",
//                Operation = "Add",
//                Object = p
//            });
//            return 1;
//        }

//        public int SaveTotalChanges(StorageOperationEnum operation, IPosition2 p)
//        {
//            throw new NotImplementedException();
//        }

//        public int SaveDealChanges(StorageOperationEnum operation, IPosition2 p)
//        {
//            throw new NotImplementedException();
//        }

//        public int SaveChanges(StorageOperationEnum operation, IOrder3 ord)
//        {
//            throw new NotImplementedException();
//        }

//        public int SaveChanges(StorageOperationEnum operation, ITrade3 p)
//        {
//            throw new NotImplementedException();
//        }
//    }
}
