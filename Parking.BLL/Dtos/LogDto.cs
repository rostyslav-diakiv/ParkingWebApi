using System;

namespace Parking.BLL.Dtos
{
    public class LogDto
    {
        private DateTime _logDateTime;

        private int _moneyEarned;

        private string _earner;

        public LogDto()
        {
        }

        public LogDto(int money, DateTime time, string earner)
        {
            MoneyEarned = money;
            LogDateTime = time;
            Earner = earner;
        }

        public DateTime LogDateTime
        {
            get => _logDateTime;

            set
            {
                if (_logDateTime != value)
                {
                    _logDateTime = value;
                }
            }
        }

        public int MoneyEarned
        {
            get => _moneyEarned;

            set
            {
                if (_moneyEarned != value)
                {
                    _moneyEarned = value;
                }
            }
        }

        public string Earner
        {
            get => _earner;

            set
            {
                if (_earner != value)
                {
                    _earner = value;
                }
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return
                $"Log was created in {LogDateTime}, {MoneyEarned}$ money were earned by {Earner} in {LogDateTime}";
        }
    }
}
