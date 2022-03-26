﻿using System.Text;
using OnlineShop.Common.Constants;

namespace OnlineShop.Models.Products.Peripherals
{
    public abstract class Peripheral : Product, IPeripheral
    {
        protected Peripheral(int id, string manufacturer, string model, decimal price, double overallPerformance, string connectionType) 
            : base(id, manufacturer, model, price, overallPerformance)
        {
            this.ConnectionType = connectionType;
        }

        public string ConnectionType { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(base.ToString() + string.Format(SuccessMessages.PeripheralToString, this.ConnectionType));

            return sb.ToString().TrimEnd();
        }
    }
}
