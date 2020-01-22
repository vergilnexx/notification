using Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Parser
{
    public class EmailParser : IParser
    {
        public async Task<IReadOnlyCollection<VacationData>> Parse(IReadOnlyCollection<IMessage> messages)
        {
            var data = new List<VacationData>();

            foreach (var message in messages)
            {
                var item = await Parse(message);
                data.Add(item);
            }

            return data;
        }

        private async Task<VacationData> Parse(IMessage message)
        {
            var data = new VacationData();

            data.From = message.From;
            
            data.Start = FindDate("DTSTART", message.Content);
            data.End = FindDate("DTEND", message.Content);

            return data;
        }

        private DateTime FindDate(string findKeyWord, string content)
        {
            var keyExpression = $"{findKeyWord};VALUE=DATE:";
            var dateStringIndex = content.IndexOf(keyExpression);
            var dateString = content.Substring(dateStringIndex + keyExpression.Length, 8);
            var date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
            return date;
        }
    }
}
